using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Helpers;

namespace LeagueStatusBot.RPGEngine.Core.Controllers 
{
    public class GameManager 
    {
        public bool IsOff {get; set;} = true;
        public string CurrentWinner {get;set;} = string.Empty;
        public string MostRecentFile {get; set;}
        private const string DEFAULT_TILE = "Idle";
        private TurnManager turnManager;
        private AssetManager assetManager;
        private AnimationManager animationManager;
        private FileManager fileManager;
        private Player? player1;
        private Player? player2;
        public GameManager(TurnManager turnManager, AssetManager assetManager, AnimationManager animationManager)
        {
            fileManager = new();
            this.assetManager = assetManager;
            this.turnManager = turnManager;
            this.animationManager = animationManager;
        }

        public bool StartGame(ulong player1Id, ulong player2Id, string player1name, string player2name)
        {
            if (!IsOff) return false;

            player1 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE, false), player1Id, player1name);
            player2 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE, true), player2Id, player2name);

            IsOff = true;

            return animationManager.CreateInitialAnimation(player1, player2);
        }

        public bool ProcessTurn(List<string> player1actions, List<string> player2actions)
        {
            var turnMessage = turnManager.ProcessTurn(player1actions, player2actions, player1!, player2!);

            fileManager.AddToCache(turnMessage.FileName);
            MostRecentFile = turnMessage.FileName;

            //TODO --> CHANGE RECORD TO BE PLAYER1HITS & PLAYER2HITS
            if (turnMessage.Player1Health == turnMessage.Player2Health)
            {
                CurrentWinner = $"*Tied last round*\n";
                return player1!.IsAlive;
            }
            else if (turnMessage.Player1Health < turnMessage.Player2Health)
            {
                player1!.Health--;
                CurrentWinner = $"*{player2!.Name} won {turnMessage.Player2Health} hits to {turnMessage.Player1Health} hits last round*\n";
                return player1.IsAlive;
            }
            else
            {
                player2!.Health--;
                CurrentWinner = $"*{player1!.Name} won *{turnMessage.Player1Health} hits* to *{turnMessage.Player2Health} hits* last round*\n";
                return player2.IsAlive;
            }
        }

        public int[] GetCurrentHitPoints()
        {
            return new int[] { player1!.Health, player2!.Health };
        }

        public void EndGame()
        {
            fileManager.AddToCache("initial.gif");
            IsOff = true;
            Task.Run(() => fileManager.DeleteAllFiles());
        }

    }
}

