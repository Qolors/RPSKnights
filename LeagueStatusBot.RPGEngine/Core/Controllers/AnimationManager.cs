using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Controllers 
{
    public class AnimationManager 
    {
        private const int SPRITE_DIMENSION = 56;
        private const int CANVAS_WIDTH = 150;
        private const int CANVAS_HEIGHT = 150;
        private const int FRAME_COUNT = 6;
        private const int ANIMATION_OFFSET = 35;
        private const string INITIAL_FILE = "initial.gif";
        private const string BATTLE_FILE = "battle.gif";
        private readonly AnimationHandler animationHandler;

        public AnimationManager(AnimationHandler animationHandler, SpriteHandler spriteHandler) 
        {
            this.animationHandler = animationHandler;
        }

        public bool CreateAnimation(Player player1, Player player2)
        {
            return true;
        }

        public bool CreateInitialAnimation(Player player1, Player player2)
        {
            try
            {
                var frames = animationHandler.CreateInitialAnimation(player1.CurrentSprite, player2.CurrentSprite, ANIMATION_OFFSET, FRAME_COUNT, SPRITE_DIMENSION, SPRITE_DIMENSION);

                using (Image<Rgba32> output = new Image<Rgba32>(CANVAS_WIDTH, CANVAS_HEIGHT)) 
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

                    output.SaveAsGif(INITIAL_FILE);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool SimulateBattleAnimation(List<string> player1Actions, List<string> player2Actions)
        {
            return false;
        }
    }
}

