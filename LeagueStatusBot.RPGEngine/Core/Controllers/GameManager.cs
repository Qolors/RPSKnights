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

        public bool ExecuteTurn(ulong gameId)
        {
            var image = assetManager.GetEntitySprite("Dwarf_Idle");
            Player player = new Player(image);

            if (animationManager.CreateAnimation(player))
            {
                return true;
            }

            return false;
        }

    }
}
