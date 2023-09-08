using SixLabors.ImageSharp.Formats.Gif;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class AnimationHandler
    {
        private readonly SpriteHandler spriteHandler;

        public AnimationHandler(SpriteHandler spriteHandler)
        {
            this.spriteHandler = spriteHandler ?? throw new ArgumentNullException(nameof(spriteHandler));
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
                frames.Add(spriteHandler.CreateAnimationFrame(player1Sprites[i], new Point((250 / 2) - player1Sprites[i].Width - offset, 200-(player1Sprites[i].Height + 16)), player2Sprites[i], new Point((250 / 2) - player2Sprites[i].Width + offset, 200-(player2Sprites[i].Height + 16))));
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

        public Image<Rgba32> CombinePlayerSprites(Image<Rgba32> player1Sprite, Image<Rgba32> player2Sprite)
        {
            return spriteHandler.CreateAnimationFrame(player1Sprite, new Point((250 / 2) - player1Sprite.Width - 15, 200-(player1Sprite.Height + 16)), player2Sprite, new Point((250 / 2) - player2Sprite.Width + 15, 200-(player2Sprite.Height + 16)));
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

        public bool CreateGifFromGifs(List<Image> images, string filePath)
        {
            try
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
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (var image in images)
            {
                image.Dispose();
            }

            return true;
        }
        

    }


}
