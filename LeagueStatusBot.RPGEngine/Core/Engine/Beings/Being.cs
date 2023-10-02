
namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings;
/// <summary>
/// Base Abstract Class
/// </summary>
public abstract class Being
{
    public Image<Rgba32> CurrentSprite { get; set; }

    public Being(Image<Rgba32> idleSprite)
    {
        CurrentSprite = idleSprite;
    }
}
