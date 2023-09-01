using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class AnimationManager
    {
        private AnimationHandler animationHandler;
        public AnimationManager()
        {
            animationHandler = new();
        }
        public bool CreateAnimation(Player player)
        {
            // Create a list to hold your frames
            var frames = animationHandler.CreateIdleAnimation(player.CurrentSprite);

            // Use the GifEncoder to save the animation
            using (Image<Rgba32> output = new Image<Rgba32>(100, 100))
            {
                var gifMeta = output.Metadata.GetGifMetadata();
                gifMeta.RepeatCount = 5;

                foreach (var frame in frames)
                {
                    output.Frames.AddFrame(frame.Frames.RootFrame);
                }
                output.Save("animation.gif", new GifEncoder());

                return true;
            }
        }
    }
}
