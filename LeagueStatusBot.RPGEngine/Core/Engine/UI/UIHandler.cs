using LeagueStatusBot.RPGEngine.Core.Controllers;

namespace LeagueStatusBot.RPGEngine.Core.Engine.UI;

/// <summary>
/// Handles all things UI like the Healthbar, player portraits.
/// </summary>
public class UIHandler
{
    private AssetManager assetManager;
    private const int WIDTH = 48;
    private const int HEIGHT = 16;
    public UIHandler(AssetManager assetManager)
    {
        this.assetManager = assetManager;
    }

    public Image<Rgba32> GenerateHealthbar(int playerHealth, bool isPlayer1)
    {
        var healthFrame = assetManager.GetInterfaceSprite("healbarframe");

        var healthBar = playerHealth == 0
        ? new Image<Rgba32>(WIDTH, HEIGHT)
        : assetManager.GetInterfaceSprite($"healthbarat{playerHealth}");

        using var frame = new Image<Rgba32>(WIDTH, HEIGHT);
        var healthBarPosition = new Point(1, 1);
        frame.Mutate(ctx => ctx.DrawImage(healthFrame, healthBarPosition, 1));
        frame.Mutate(ctx => ctx.DrawImage(healthBar, healthBarPosition, 1));

        return frame.Clone();
    }

}
