using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Helpers;

namespace LeagueStatusBot.RPGEngine.Core.Controllers;


/// <summary>
/// Every game is created in this class. GameManager manages the states of the overall gameplay from StartGame to EndGame
/// </summary>
public class GameManager
{
    public bool IsOff { get; set; } = true;
    public bool Player1Won { get; set; }
    public bool Forfeit { get; set; } = false;
    public string CurrentWinner { get; set; } = string.Empty;
    public string FinalWinnerName { get; set; } = string.Empty;
    public string MostRecentFile { get; set; }
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
        this.turnManager = turnManager;
        this.assetManager = assetManager;
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

        return await animationManager.CreateInitialAnimation(player1, player2, fileManager.GetBasePath);
    }

    public async Task<string> ProcessDecisions()
    {
        fileManager.DeleteInitialFile("initial.gif");

        return await animationManager.CreateInitialAnimation(player1!, player2!, fileManager.GetBasePath);
    }

    public async Task<bool> ProcessTurn(List<string> player1actions, List<string> player2actions)
    {
        var turnMessage = await turnManager.ProcessTurn(player1actions, player2actions, player1!, player2!, fileManager.GetBasePath, animationManager);

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
            int damage;
            if (player2actions[0] == "Ability" && player1actions[0] == "Defend")
            {
                damage = 2;
            }
            else
            {
                damage = 1;
            }
            player1!.Health -= damage;
            CurrentWinner = $"*{player2!.Name} won  last round*\n";
            FinalWinnerName = player2!.Name;
            return player1.IsAlive;
        }
        else
        {
            int damage;
            if (player1actions[0] == "Ability" && player2actions[0] == "Defend")
            {
                damage = 2;
            }
            else
            {
                damage = 1;
            }
            player2!.Health -= damage;
            CurrentWinner = $"*{player1!.Name} won last round*\n";
            FinalWinnerName = player1.Name;
            return player2.IsAlive;
        }
    }

    public async void ProcessDeathScene()
    {
        var turnMessage = await animationManager.CreateDeathAnimation(player1!, player2!, fileManager.GetBasePath);
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
                player.Energy -= 1;
                break;
            case "Defend":
                break;
            case "Ability":
                player.Energy -= 3;
                break;
        }

        player.Energy = Math.Min(player.Energy + 1, 5);
    }

    public void EndGame()
    {
        IsOff = true;

        var gifs = fileManager.LoadAllGifs();

        Console.WriteLine("Loaded Gifs for FinalBattle Gif");

        if (!animationManager.CreateGifFromGifs(gifs, fileManager.GetBasePath + "FinalBattle.gif"))
        {
            //TODO --> PROBABLY WON'T NEED THIS
            Console.WriteLine("");
        }

        MostRecentFile = fileManager.GetBasePath + "FinalBattle.gif";

        fileManager.AddToCache("initial.gif");
        fileManager.AddToCache("FinalBattle.gif");

        Player1Won = player1!.IsAlive;
    }

    public void PlayerForfeit(ulong forfeitPlayer)
    {
        if (forfeitPlayer == player1!.GetUserId)
        {
            player1.Health = 0;
        }
        else if (forfeitPlayer == player2!.GetUserId)
        {
            player2.Health = 0;
        }

        Forfeit = true;
    }

    public void Dispose()
    {
        Task.Run(() => fileManager.DeleteAllFiles());
        OnGameEnded?.Invoke(this, EventArgs.Empty);
    }

}

