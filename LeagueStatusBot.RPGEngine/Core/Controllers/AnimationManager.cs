using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Metadata;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class AnimationManager
    {
        private AnimationHandler animationHandler;
        public AnimationManager()
        {
            animationHandler = new();
        }
        public bool CreateAnimation(Player player, Image<Rgba32> target)
        {
            var resizeTarget = animationHandler.ResizeImage(target, 56, 56);

            var frames = animationHandler.CreateAnimation(player.CurrentSprite, resizeTarget, 35, 8, 56, 56);

            using (Image<Rgba32> output = new Image<Rgba32>(150, 150))
            {
                var gifMeta = output.Metadata.GetGifMetadata();

                gifMeta.RepeatCount = 0;

                foreach (var frame in frames)
                {
                    output.Frames.AddFrame(frame.Frames.RootFrame);
                }

                output.Frames.RemoveFrame(0);

                for (int i = 0; i < output.Frames.Count; i++)
                {
                    var frame = output.Frames[i];
                    var metadata = frame.Metadata;
                    var frameMetadata = metadata.GetGifMetadata();
                    frameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;
                }

                output.SaveAsGif("animation.gif");

                return true;
            }
        }
    }
}
