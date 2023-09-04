using LeagueStatusBot.RPGEngine.Core.Engine.Beings;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class GameManager
    {
        private AssetManager assetManager;
        private AnimationManager animationManager;

        public GameManager()
        {
            assetManager = new AssetManager();
            animationManager = new AnimationManager();
        }

        public bool ExecuteTurn(Image<Rgba32> targetImage)
        {
            var image = assetManager.GetEntitySprite("tile001");

            Player player = new Player(image);

            if (animationManager.CreateAnimation(player, targetImage))
            {
                return true;
            }

            return false;
        }

    }

    public enum AttackType
    {
        Attack,
        Burn,
        
    }
}
