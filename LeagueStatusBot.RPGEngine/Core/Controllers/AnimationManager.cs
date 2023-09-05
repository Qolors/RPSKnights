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
        private readonly AssetManager assetManager;

        public AnimationManager(AnimationHandler animationHandler, AssetManager assetManager)
        {
            this.animationHandler = animationHandler;
            this.assetManager = assetManager;
        }

        public bool CreateBattleAnimation(Player player1, Player player2, List<string> player1actions, List<string> player2actions)
        {
            var allFrames = new List<Image<Rgba32>>();

            for (int i = 0; i < player1actions.Count; i++)
            {
                var player1Action = player1actions[i];
                var player2Action = player2actions[i];

                player1.CurrentSprite = assetManager.GetEntitySprite(player1Action);
                player2.CurrentSprite = assetManager.GetEntitySprite(player2Action);

                List<Image<Rgba32>> framesPlayer1;
                List<Image<Rgba32>> framesPlayer2;

                // Handle player1's actions
                if (player1Action == "Attack")
                {
                    player1.CurrentSprite = assetManager.GetEntitySprite("Attack");
                    framesPlayer1 = animationHandler.CreateAttackAnimation(player1.CurrentSprite, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, false);
                }
                else if (player1Action == "Defend")
                {
                    player1.CurrentSprite = assetManager.GetEntitySprite("Defend");
                    framesPlayer1 = animationHandler.CreateDefendAnimation(player1.CurrentSprite, 3, SPRITE_DIMENSION, SPRITE_DIMENSION, false);
                }
                else // ability
                {
                    player1.CurrentSprite = assetManager.GetEntitySprite("Ability");
                    framesPlayer1 = animationHandler.CreateAbilityAnimation(player1.CurrentSprite, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, false);
                }

                // Handle player2's actions
                if (player2Action == "Attack")
                {
                    player2.CurrentSprite = assetManager.GetEntitySprite("Attack");
                    framesPlayer2 = animationHandler.CreateAttackAnimation(player2.CurrentSprite, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, true);
                }
                else if (player2Action == "Defend")
                {
                    player2.CurrentSprite = assetManager.GetEntitySprite("Defend");
                    framesPlayer2 = animationHandler.CreateDefendAnimation(player2.CurrentSprite, 3, SPRITE_DIMENSION, SPRITE_DIMENSION, true);
                }
                else // ability
                {
                    player2.CurrentSprite = assetManager.GetEntitySprite("Ability");
                    framesPlayer2 = animationHandler.CreateAbilityAnimation(player2.CurrentSprite, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, true);
                }
                
                string winner = DetermineWinner(player1Action, player2Action);
                int hitFrameIndex = 0;
                var hitFrames = animationHandler.CreateHitAnimation(assetManager.GetEntitySprite("Hit"), 3, SPRITE_DIMENSION, SPRITE_DIMENSION, winner == "player2");
                // Combine frames from both players
                for (int j = 0; j < Math.Max(framesPlayer1.Count, framesPlayer2.Count); j++)
                {
                    if (winner == "player2" && j >= 4)
                    {
                        allFrames.Add(hitFrames[hitFrameIndex]);
                        hitFrameIndex++;
                    }
                    else if (j < framesPlayer1.Count)
                    {
                        allFrames.Add(framesPlayer1[j]);
                    }
                    if (winner == "player1" && j >= 4)
                    {
                        allFrames.Add(hitFrames[hitFrameIndex]);
                        hitFrameIndex++;
                    }
                    else if (j < framesPlayer2.Count)
                    {
                        allFrames.Add(framesPlayer2[j]);
                    }
                }
            }

            using (Image<Rgba32> output = new Image<Rgba32>(CANVAS_WIDTH, CANVAS_HEIGHT))
                {
                    var gifMeta = output.Metadata.GetGifMetadata();
                    gifMeta.RepeatCount = 0;

                    foreach (var frame in allFrames)
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

                    output.SaveAsGif(BATTLE_FILE);
                }

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

        private string DetermineWinner(string player1action, string player2action)
        {
            if (player1action == "Attack" && player2action == "Defend")
            {
                return "player2";
            }
            else if (player1action == "Defend" && player2action == "Ability")
            {
                return "player2";
            }
            else if (player1action == "Ability" && player2action == "Attack")
            {
                return "player2";
            }
            else
            {
                return "player1";
            }
            
        }
    }
}

