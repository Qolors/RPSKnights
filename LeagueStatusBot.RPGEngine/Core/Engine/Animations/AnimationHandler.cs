using LeagueStatusBot.RPGEngine.Core.Controllers;
using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations;

/// <summary>
/// AnimationHandler handles the creation of each frame for the Gif files - Mutation, Drawing.
/// Currently AnimationHandler and SpriteHandler have an identity crisis - future updates need refactoring of the two.
/// </summary>
public class AnimationHandler
{
    private readonly SpriteHandler spriteHandler;
    public AnimationHandler(SpriteHandler spriteHandler)
    {
        this.spriteHandler = spriteHandler ?? throw new ArgumentNullException(nameof(spriteHandler));
    }

    public void SetAssetManager(AssetManager assetManager)
    {
        spriteHandler.SetAssetManager(assetManager);
    }

    public List<Image<Rgba32>> CreateInitialAnimation(
        Image<Rgba32> player1SpriteSheet,
        Image<Rgba32> player2SpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        int health1,
        int health2)
    {

        var player1Sprites = GetPlayerSpriteFrames(player1SpriteSheet, frameCount, spriteWidth, spriteHeight, false);
        var player2Sprites = GetPlayerSpriteFrames(player2SpriteSheet, frameCount, spriteWidth, spriteHeight, true);
        var frames = new List<Image<Rgba32>>();

        for (int i = 0; i < frameCount; i++)
        {
            frames.Add(spriteHandler.CreateAnimationFrame(
                player1Sprites[i], new Point((250 / 2) - 60, 200 - (player1Sprites[i].Height + 16)),
                player2Sprites[i], new Point((250 / 2) - 30, 200 - (player2Sprites[i].Height + 16)),
                health1,
                health2)
            );
        }

        return frames;

    }

    private List<Image<Rgba32>> GetPlayerSpriteFrames(Image<Rgba32> playerImage, int frameCount, int spriteWidth, int spriteHeight, bool flip)
    {
        spriteHandler.LoadSpriteSheet(playerImage);

        List<Image<Rgba32>> playerSprites = new();

        for (int i = 0; i < frameCount; i++)
        {
            var sprite = spriteHandler.ExtractSprite(i * spriteWidth, 0, spriteWidth, spriteHeight);

            if (flip)
            {
                sprite.Mutate(x => x.Flip(FlipMode.Horizontal));
            }

            sprite.Mutate(x => x.Resize(new Size(112, 112)));

            playerSprites.Add(sprite);
        }

        return playerSprites;
    }

    public Image<Rgba32> CombinePlayerSprites(
        Image<Rgba32> player1Sprite,
        Image<Rgba32> player2Sprite,
        int health1,
        int health2)
    {
        return spriteHandler.CreateAnimationFrame(player1Sprite, new Point((250 / 2) - 60, 200 - (player1Sprite.Height + 16)), player2Sprite, new Point((250 / 2) - 30, 200 - (player2Sprite.Height + 16)), health1, health2);
    }

    public List<Image<Rgba32>> CreateAttackAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        return GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);
    }

    public List<Image<Rgba32>> CreateDefendAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        return GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);
    }

    public List<Image<Rgba32>> CreateAbilityAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        return GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);
    }

    public List<Image<Rgba32>> CreateHitAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        var hitFrames = GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);

        hitFrames[0].Mutate(x => x.Brightness(100));
        return hitFrames;
    }

    public List<Image<Rgba32>> CreateIdleAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        return GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);
    }

    public List<Image<Rgba32>> CreateDeathAnimation(
        Image<Rgba32> playerSpriteSheet,
        int frameCount,
        int spriteWidth,
        int spriteHeight,
        bool flip)
    {
        return GetPlayerSpriteFrames(playerSpriteSheet, frameCount, spriteWidth, spriteHeight, flip);
    }
}
