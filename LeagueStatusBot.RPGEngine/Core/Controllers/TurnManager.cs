using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Messaging;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class TurnManager
    {
        private AnimationManager animationManager;
        public TurnManager(AnimationManager animationManager)
        {
            this.animationManager = animationManager;
        }
        public TurnMessage ProcessTurn(List<string> player1actions, List<string> player2actions, Player player1, Player player2, string basePath, AnimationManager animationManager)
        {
            this.animationManager = animationManager;   
            return this.animationManager.CreateBattleAnimation(player1, player2, player1actions, player2actions, basePath);
        }
    }
}