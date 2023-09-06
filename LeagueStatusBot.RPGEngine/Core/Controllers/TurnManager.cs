using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Messaging;

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
        public TurnMessage ProcessTurn(List<string> player1actions, List<string> player2actions, Player player1, Player player2)
        {
            return animationManager.CreateBattleAnimation(player1, player2, player1actions, player2actions);
        }
    }
}