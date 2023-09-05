using LeagueStatusBot.RPGEngine.Core.Engine.Beings;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class TurnManager
    {
        private AnimationManager animationManager;
        private AssetManager assetManager;
        public TurnManager(AnimationManager animationManager, AssetManager assetManager)
        {
            this.animationManager = animationManager;
            this.assetManager = assetManager;
        }
        public bool ProcessTurn(List<string> player1actions, List<string> player2actions, Player player1, Player player2)
        {
            try
            {
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}