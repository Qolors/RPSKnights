namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class AnimationHandler
    {
        private readonly SpriteHandler spriteHandler;

        public AnimationHandler(SpriteHandler spriteHandler)
        {
            this.spriteHandler = spriteHandler ?? throw new ArgumentNullException(nameof(spriteHandler));
        }

        private Image<Rgba32> ExtractSprite(Image<Rgba32> image, int x, int y, int width, int height)
        {
            spriteHandler.LoadSpriteSheet(image);
            return spriteHandler.ExtractSprite(x, y, width, height);
        }

        private IEnumerable<Image<Rgba32>> ExtractSprites(Image<Rgba32> image, int startIdx, int count, int spriteWidth, int spriteHeight) 
        { 
            spriteHandler.LoadSpriteSheet(image); 
            for (int i = 0; i < count; i++) 
            { 
                yield return spriteHandler.ExtractSprite(startIdx + i * spriteWidth, 0, spriteWidth, spriteHeight); 
            } 
        }

        public List<Image<Rgba32>> CreateInitialAnimation(
            Image<Rgba32> player1SpriteSheet,
            Image<Rgba32> player2SpriteSheet,
            int offset,
            int frameCount,
            int spriteWidth,
            int spriteHeight)
        {

            var player1Sprites = GetPlayerSpriteFrames(player1SpriteSheet, frameCount, spriteWidth, spriteHeight, false);
            var player2Sprites = GetPlayerSpriteFrames(player2SpriteSheet, frameCount, spriteWidth, spriteHeight, true);

            var frames = new List<Image<Rgba32>>();

            for (int i = 0; i < frameCount; i++)
            {
                frames.Add(spriteHandler.CreateAnimationFrame(player1Sprites[i], new Point(15, 25), player2Sprites[i], new Point(15 + offset, 25)));
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
                
                playerSprites.Add(sprite);
            }

            return playerSprites;
        }

    }


}
