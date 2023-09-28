using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Helpers;

namespace LeagueStatusBot.RPGEngine.Core.Controllers 
{
    public class GameManager 
    {
        public bool IsOff {get; set;} = true;
        public bool Player1Won {get; set;}
        public string CurrentWinner {get;set;} = string.Empty;
        public string FinalWinnerName { get; set; } = string.Empty;
        public string MostRecentFile {get; set;}
        public EventHandler OnGameEnded;
        private const string DEFAULT_TILE = "Idle";
        private TurnManager turnManager;
        private AssetManager assetManager;
        private AnimationManager animationManager;
        private FileManager fileManager;
        private Player? player1;
        private Player? player2;
        public GameManager(TurnManager turnManager, AssetManager assetManager, AnimationManager animationManager, ulong gameKey)
        {
            fileManager = new(gameKey);
            this.assetManager = assetManager;
            this.turnManager = turnManager;
            this.animationManager = animationManager;
        }

        public async Task<string> StartGame(ulong player1Id, ulong player2Id, string player1name, string player2name, string player1url, string player2url)
        {
            if (!IsOff) return string.Empty;

            player1 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE, false), player1Id, player1name, player1url);
            player2 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE, true), player2Id, player2name, player2url);

            await assetManager.LoadPlayer1Avatar(player1url);
            await assetManager.LoadPlayer2Avatar(player2url);

            animationManager.SetAssetManager(assetManager);

            Console.WriteLine("Loaded Avatars");
            IsOff = true;

            if (animationManager == null)
            {
                throw new ArgumentNullException(nameof(animationManager), "AnimationManager null");
            }

            return animationManager.CreateInitialAnimation(player1, player2, fileManager.GetBasePath);
        }

        public string ProcessDecisions()
        {
            fileManager.DeleteInitialFile("initial.gif");

            return animationManager.CreateInitialAnimation(player1!, player2!, fileManager.GetBasePath);
        }

        public bool ProcessTurn(List<string> player1actions, List<string> player2actions)
        {
            var turnMessage = turnManager.ProcessTurn(player1actions, player2actions, player1!, player2!, fileManager.GetBasePath, animationManager);

            ProcessEnergy(player1actions[0], player1!);
            ProcessEnergy(player2actions[0], player2!);


            fileManager.AddToCache(turnMessage.FileName);

            MostRecentFile = turnMessage.FileName;

            if (turnMessage.Player1Health == turnMessage.Player2Health)
            {
                CurrentWinner = $"*Tied last round*\n";
                return true;
            }
            else if (turnMessage.Player1Health < turnMessage.Player2Health)
            {
                ProcessSpecialEffects(player2actions[0], player2!);
                int damage;
                if (player2actions[0] == "Ability" && player1actions[0] == "Defend")
                {
                    damage = 2;
                    player1!.Energy -= 1;
                }
                else if (player2actions[0] == "Attack" && player1actions[0] == "Ability")
                {
                    damage = 1;
                    player1!.Energy -= 1;
                }
                else
                {
                    damage = 1;
                }
                player1!.Health -= damage;
                CurrentWinner = $"*{player2!.Name} won {turnMessage.Player2Health} hits to {turnMessage.Player1Health} hits last round*\n";
                FinalWinnerName = player2!.Name;
                return player1.IsAlive;
            }
            else
            {
                ProcessSpecialEffects(player1actions[0], player1!);
                int damage;
                if (player1actions[0] == "Ability" && player2actions[0] == "Defend")
                {
                    damage = 2;
                    player2!.Energy -= 1;
                }
                else if (player1actions[0] == "Attack" && player2actions[0] == "Ability")
                {
                    damage = 1;
                    player2!.Energy -= 1;
                }
                else
                {
                    damage = 1;
                }
                player2!.Health -= damage;
                CurrentWinner = $"*{player1!.Name} won {turnMessage.Player1Health} hits to {turnMessage.Player2Health} hits last round*\n";
                FinalWinnerName = player1.Name;
                return player2.IsAlive;
            }
        }

        public void ProcessDeathScene()
        {
            var turnMessage = animationManager.CreateDeathAnimation(player1!, player2!, fileManager.GetBasePath);
            MostRecentFile = turnMessage.FileName;
            fileManager.AddToCache(turnMessage.FileName);
        }

        public int[] GetCurrentHitPoints()
        {
            return new int[] { player1!.Health, player2!.Health };
        }

        public int GetPlayer1Energy => player1!.Energy;
        public int GetPlayer2Energy => player2!.Energy;

        private void ProcessEnergy(string action, Player player)
        {
            switch (action)
            {
                case "Attack":
                    player.Energy -= 2;
                    break;
                case "Defend":
                    player.Energy -= 1;
                    break;
                case "Ability":
                    player.Energy -= 3;
                    break;
                case "Overcharge":
                    player.Energy = Math.Min(player.Energy + 3, 5);
                    break;
            }

            player.Energy = Math.Min(player.Energy + 1, 5);
        }

        private void ProcessSpecialEffects(string winningAction, Player winner)
        {
            switch (winningAction)
            {
                case "Attack":
                    winner.Energy = Math.Min(winner.Energy + 1, 5);
                    break;
                case "Defend":
                    winner.Energy = Math.Min(winner.Energy + 2, 5);
                    break;
            }
        }

        public void EndGame()
        {
            IsOff = true;

            var gifs = fileManager.LoadAllGifs();

            Console.WriteLine("Loaded Gifs for FinalBatlle Gif");

            if (!animationManager.CreateGifFromGifs(gifs, fileManager.GetBasePath + "FinalBattle.gif"))
            {
                //TODO --> PROBABLY WON'T NEED THIS
                Console.WriteLine("");
            }

            MostRecentFile = fileManager.GetBasePath + "FinalBattle.gif";

            fileManager.AddToCache("initial.gif");
            fileManager.AddToCache("FinalBattle.gif");

            Console.WriteLine("Finished Ending Game");

            Player1Won = player1!.IsAlive;
        }

        public void Dispose()
        {
            Task.Run(() => fileManager.DeleteAllFiles());
            OnGameEnded?.Invoke(this, EventArgs.Empty);
        }

    }
}

