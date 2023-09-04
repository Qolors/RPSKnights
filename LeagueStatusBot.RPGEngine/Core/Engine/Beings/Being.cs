
namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings
{
    public abstract class Being
    {
        public Image<Rgba32> CurrentSprite { get; set; }

        public Being(Image<Rgba32> idleSprite)
        {
            CurrentSprite = idleSprite;
        }
    }
}
