using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.Beings;
using LeagueStatusBot.RPGEngine.Messaging;
using LeagueStatusBot.RPGEngine.Helpers;
using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Controllers;

/// <summary>
/// AnimationManager manages the creation of a gif file after each round.
/// It runs in order like so:
/// - CreateInitialAnimation
/// - CreateBattleAnimation (until someone wins)
/// - CreateDeathAnimation
/// - CreateGifFromGifs (Does a full replay of the entire fight)
/// </summary>
public class AnimationManager
{
    private const int SPRITE_DIMENSION = 56;
    private const int FRAME_COUNT = 6;
    private const int OUTPUT_WIDTH = 250;
    private const int OUTPUT_HEIGHT = 200;
    private const string GIF_EXTENSION = ".gif";
    private const int ANIMATION_OFFSET = 15;
    private const string INITIAL_FILE = "initial.gif";
    private readonly AnimationHandler animationHandler;
    private AssetManager assetManager;

    public AnimationManager(AnimationHandler animationHandler)
    {
        this.animationHandler = animationHandler;
    }

    public void SetAssetManager(AssetManager assetManager)
    {
        this.assetManager = assetManager;
        animationHandler.SetAssetManager(assetManager);
    }

    public async Task<TurnMessage> CreateBattleAnimation(Player player1, Player player2, List<string> player1actions, List<string> player2actions, string basePath)
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

        for (int i = 0; i < 1; i++)
        {
            var player1Action = player1actions[i];
            var player2Action = player2actions[i];

            // Fetch corresponding frames for each player action
            var framesPlayer1 = GetActionFrames(player1Action, false);
            var framesPlayer2 = GetActionFrames(player2Action, true);

            Winner winner = DetermineWinner(player1Action, player2Action);

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
                if (winner == Winner.Player1 && actionHitFrames[player1Action].Contains(j))
                {
                    player2HitCounter = hitFrameCount;
                    player1Strikes++;
                }

                if (winner == Winner.Player2 && actionHitFrames[player2Action].Contains(j))
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

                var combinedFrame = animationHandler.CombinePlayerSprites(framePlayer1, framePlayer2, player1.Health, player2.Health);

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

        var filePath = await SaveGif(allFrames, basePath);

        return new TurnMessage(player1Strikes, player2Strikes, filePath);
    }

    public async Task<TurnMessage> CreateDeathAnimation(Player player1, Player player2, string basePath)
    {
        int maxFrames = 12;
        int deathFrameCount = 8;
        int death2FrameCount = 4;
        int idleFrameCount = 6;

        bool player1win = player1.IsAlive ? true : false;

        var deathFrames = animationHandler.CreateHitAnimation(assetManager.GetEntitySprite("Death", player1.IsAlive), deathFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, player1win);
        var death2Frames = animationHandler.CreateHitAnimation(assetManager.GetEntitySprite("Death2", player1.IsAlive), death2FrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, player1win);
        var idleFrames = animationHandler.CreateIdleAnimation(assetManager.GetEntitySprite("Idle", !player1.IsAlive), idleFrameCount, SPRITE_DIMENSION, SPRITE_DIMENSION, !player1win);

        var allFrames = new List<Image<Rgba32>>();

        for (int j = 0; j < maxFrames; j++)
        {
            var frame1 = player1.IsAlive ? (j < idleFrames.Count ? idleFrames[j] : idleFrames[j % idleFrames.Count]) : (j < deathFrames.Count ? deathFrames[j] : death2Frames[j % death2Frames.Count]);
            var frame2 = player1.IsAlive ? (j < deathFrames.Count ? deathFrames[j] : death2Frames[j % death2Frames.Count]) : (j < idleFrames.Count ? idleFrames[j] : idleFrames[j % idleFrames.Count]);

            var combinedFrame = animationHandler.CombinePlayerSprites(frame1, frame2, player1.Health, player2.Health);

            allFrames.Add(combinedFrame);
        }

        var filePath = await SaveGif(allFrames, basePath);

        return new TurnMessage(0, 0, filePath);
    }

    public async Task<string> CreateInitialAnimation(Player player1, Player player2, string basePath)
    {
        var frames = animationHandler.CreateInitialAnimation(player1.CurrentSprite, player2.CurrentSprite, FRAME_COUNT, SPRITE_DIMENSION, SPRITE_DIMENSION, player1.Health, player2.Health);

        return await SaveGif(frames, basePath, INITIAL_FILE);
    }

    private Winner DetermineWinner(string player1action, string player2action)
    {
        if (player1action == player2action)
        {
            return Winner.Tie;
        }
        else if (player1action == "Attack" && player2action == "Defend")
        {
            return Winner.Player2;
        }
        else if (player1action == "Defend" && player2action == "Ability")
        {
            return Winner.Player2;
        }
        else if (player1action == "Ability" && player2action == "Attack")
        {
            return Winner.Player2;
        }
        else
        {
            return Winner.Player1;
        }

    }

    private async Task<string> SaveGif(List<Image<Rgba32>> frames, string basePath, string fileName = "")
    {
        using (Image<Rgba32> output = new(OUTPUT_WIDTH, OUTPUT_HEIGHT))
        {
            var gifMeta = output.Metadata.GetGifMetadata();

            gifMeta.RepeatCount = 0;

            foreach (var frame in frames)
            {
                var resizedFrame = frame.Clone(ctx => ctx.Resize(OUTPUT_WIDTH, OUTPUT_HEIGHT));
                output.Frames.AddFrame(resizedFrame.Frames.RootFrame);
            }
            //ELIMINATES BLACK FLICKERING AT BEGINNING OF GIF
            output.Frames.RemoveFrame(0);

            for (int i = 0; i < output.Frames.Count; i++)
            {
                var frame = output.Frames[i];
                var metadata = frame.Metadata;
                var frameMetadata = metadata.GetGifMetadata();
                frameMetadata.DisposalMethod = GifDisposalMethod.RestoreToBackground;
            }

            string newFileName = fileName == string.Empty ?
            Path.GetRandomFileName() + GIF_EXTENSION :
            fileName;

            await output.SaveAsGifAsync(basePath + newFileName);
            return basePath + newFileName;
        }
    }

    public bool CreateGifFromGifs(List<Image> images, string filePath)
    {
        using (var gif = new Image<Rgba32>(images[0].Width, images[0].Height))
        {
            foreach (var image in images)
            {
                foreach (var frame in image.Frames)
                {
                    gif.Frames.AddFrame(frame);
                }
            }

            var gifEncoder = new GifEncoder();

            using (var output = File.OpenWrite(filePath))
            {
                gifEncoder.Encode(gif, output);
            }

            gif.SaveAsGif(filePath);
        }

        return true;
    }
}

