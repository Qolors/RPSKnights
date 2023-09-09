using LeagueStatusBot.RPGEngine.Core.Controllers;

namespace LeagueStatusBot.RPGEngine.Core.Engine.UI
{
    public class UIHandler
    {
        private AssetManager assetManager;
        public UIHandler(AssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        public Image<Rgba32> GenerateHealthbar(int playerHealth, bool isPlayer1)
        {
            var healthFrame = assetManager.GetInterfaceSprite("healbarframe");

            var healthBar = playerHealth == 0 
            ? new Image<Rgba32>(48, 16) 
            : assetManager.GetInterfaceSprite($"healthbarat{playerHealth}");

            using var frame = new Image<Rgba32>(48, 16);
            var healthBarPosition = new Point(1, 1);
            frame.Mutate(ctx => ctx.DrawImage(healthFrame, healthBarPosition, 1));
            frame.Mutate(ctx => ctx.DrawImage(healthBar, healthBarPosition, 1));
            
            return frame.Clone();
        }

    }
}
