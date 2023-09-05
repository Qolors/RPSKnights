using LeagueStatusBot.RPGEngine.Core.Engine.Beings;

namespace LeagueStatusBot.RPGEngine.Core.Controllers 
{
    public class GameManager 
    {
        private const string DEFAULT_TILE = "Idle";
        private TurnManager turnManager;
        private AssetManager assetManager;
        private AnimationManager animationManager;
        private Player player1;
        private Player player2;
        public GameManager(TurnManager turnManager, AssetManager assetManager, AnimationManager animationManager)
        {
            this.assetManager = assetManager;
            this.turnManager = turnManager;
            this.animationManager = animationManager;
        }

        public bool StartGame(ulong player1Id, ulong player2Id)
        {
            player1 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE), player1Id);
            player2 = new Player(assetManager.GetEntitySprite(DEFAULT_TILE), player2Id);

            if (animationManager.CreateInitialAnimation(player1, player2))
            {
                return true;
            }

            return false;
        }

        public bool ProcessTurn(List<string> player1actions, List<string> player2actions)
        {
            return turnManager.ProcessTurn(player1actions, player2actions, player1, player2);
        }

        public void EndGame()
        {

        }

    }
}

