using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Messaging;
using LeagueStatusBot.RPGEngine.Helpers;
using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class AnimationManager
    {
        private const int SPRITE_DIMENSION = 56;
        private const int FRAME_COUNT = 6;
        private const int ANIMATION_OFFSET = 15;
        private const string INITIAL_FILE = "initial.gif";
        private readonly AnimationHandler animationHandler;
        private readonly AssetManager assetManager;

        public AnimationManager(AnimationHandler animationHandler, AssetManager assetManager)
        {
            this.animationHandler = animationHandler;
            this.assetManager = assetManager;
        }

        public TurnMessage CreateBattleAnimation(Player player1, Player player2, List<string> player1actions, List<string> player2actions)
        {
            Dictionary<string, List<int>> actionHitFrames = new Dictionary<string, List<int>>
            {
                { "Attack", new List<int> { 4, 7 } },
                { "Ability", new List<int> { 4 } },
                { "Defend", new List<int> { 3 } }
            };
            var allFrames = new List<Image<Rgba32>>();

            int player1Strikes = 0;
            int player2Strikes = 0;

            string newFileName = string.Empty;
            
            for (int i = 0; i < 3; i++)
            {
                var player1Action = player1actions[i];
                var player2Action = player2actions[i];

                // Fetch corresponding frames for each player action
                var framesPlayer1 = GetActionFrames(player1Action, false);
                var framesPlayer2 = GetActionFrames(player2Action, true);

                string winner = DetermineWinner(player1Action, player2Action);

                int maxFrames = 12;
                int hitFrameCount = 3;
                int idleFrameCount = 6;

                var hitFrames1 = animationHandler.CreateHitAnimation(assetManager.GetEntitySprite("Hit", false), hitFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, false);
                var hitFrames2 = animationHandler.CreateHitAnimation(assetManager.GetEntitySprite("Hit", true), hitFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, true);
                var idleFrames1 = animationHandler.CreateIdleAnimation(assetManager.GetEntitySprite("Idle", false), idleFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, false);
                var idleFrames2 = animationHandler.CreateIdleAnimation(assetManager.GetEntitySprite("Idle", true), idleFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, true);

                int player1HitCounter = 0;
                int player2HitCounter = 0;

                for (int j = 0; j < maxFrames; j++)
                {
                    //TODO --> REMOVE MAGIC STRINGS FOR ENUM WINNER
                    if (winner == "player1" && actionHitFrames[player1Action].Contains(j))
                    {
                        player2HitCounter = hitFrameCount;
                        player1Strikes++;
                    }

                    if (winner == "player2" && actionHitFrames[player2Action].Contains(j))
                    {
                        player1HitCounter = hitFrameCount;
                        player2Strikes++;
                    }

                    var framePlayer1 = player1HitCounter > 0 
                    ? hitFrames1[hitFrameCount - player1HitCounter] 
                    : (j < framesPlayer1.Count 
                        ? framesPlayer1[j] 
                        : idleFrames1[j % idleFrames1.Count]);

                    var framePlayer2 = player2HitCounter > 0 
                    ? hitFrames2[hitFrameCount - player2HitCounter] 
                    : (j < framesPlayer2.Count 
                        ? framesPlayer2[j] 
                        : idleFrames2[j % idleFrames2.Count]);

                    if (player1HitCounter > 0) player1HitCounter--;
                    if (player2HitCounter > 0) player2HitCounter--;

                    var combinedFrame = animationHandler.CombinePlayerSprites(framePlayer1, framePlayer2);

                    allFrames.Add(combinedFrame);
                }
            }

            List<Image<Rgba32>> GetActionFrames(string action, bool isPlayer2)
            {
                var spriteName = assetManager.GetEntitySprite(action, isPlayer2);
                return action switch
                {
                    "Attack" => animationHandler.CreateAttackAnimation(spriteName, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, isPlayer2),
                    "Defend" => animationHandler.CreateDefendAnimation(spriteName, 3, SPRITE_DIMENSION, SPRITE_DIMENSION, isPlayer2),
                    "Ability" => animationHandler.CreateAbilityAnimation(spriteName, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, isPlayer2),
                    _ => animationHandler.CreateHitAnimation(spriteName, 8, SPRITE_DIMENSION, SPRITE_DIMENSION, isPlayer2),
                };
            }

            try
            {   
                using (Image<Rgba32> output = new(250, 200))
                {
                    var gifMeta = output.Metadata.GetGifMetadata();
                    gifMeta.RepeatCount = 0;

                    foreach (var frame in allFrames)
                    {
                        var resizedFrame = frame.Clone(ctx => ctx.Resize(250, 200));
                        output.Frames.AddFrame(resizedFrame.Frames.RootFrame);
                    }

                    output.Frames.RemoveFrame(0);

                    for (int i = 0; i < output.Frames.Count; i++)
                    {
                        var frame = output.Frames[i];
                        var metadata = frame.Metadata;
                        var frameMetadata = metadata.GetGifMetadata();
                        frameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;
                    }

                    newFileName = Path.GetRandomFileName() + ".gif";
                    output.SaveAsGif(newFileName);
                    Console.WriteLine("Saved");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return new TurnMessage(player1Strikes, player2Strikes, newFileName);            
        }

        public bool CreateInitialAnimation(Player player1, Player player2)
        {
            try
            {
                var frames = animationHandler.CreateInitialAnimation(player1.CurrentSprite, player2.CurrentSprite, ANIMATION_OFFSET, FRAME_COUNT, SPRITE_DIMENSION, SPRITE_DIMENSION);

                using (Image<Rgba32> output = new(250, 200))
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
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private string DetermineWinner(string player1action, string player2action)
        {
            // Check if both players have the same action
            if (player1action == player2action)
            {
                return "tie";
            }
            else if (player1action == "Attack" && player2action == "Defend")
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

