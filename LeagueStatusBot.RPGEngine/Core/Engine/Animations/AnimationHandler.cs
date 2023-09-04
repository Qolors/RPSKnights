using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Generic;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class AnimationHandler
    {
        private SpriteHandler spriteHandler;

        public AnimationHandler()
        {
            spriteHandler = new SpriteHandler();
        }

        private Image<Rgba32> ExtractSprite(Image image, int x, int y, int width, int height)
        {
            spriteHandler.LoadSpriteSheet(image);
            return spriteHandler.ExtractSprite(x, y, width, height);
        }

        private IEnumerable<Image<Rgba32>> ExtractSprites(Image image, int startIdx, int count, int spriteWidth, int spriteHeight)
        {
            for (int i = 0; i < count; i++)
            {
                yield return ExtractSprite(image, startIdx + i * spriteWidth, 0, spriteWidth, spriteHeight);
            }
        }

        public List<Image<Rgba32>> CreateAnimation(
            Image<Rgba32> playerSpriteSheet,
            Image<Rgba32> targetAvatar,
            int offset,
            int frameCount,
            int spriteWidth,
            int spriteHeight)
        {
            spriteHandler.LoadSpriteSheet(playerSpriteSheet);

            List<Image<Rgba32>> playerSprites = new List<Image<Rgba32>>();

            for ( int i = 0; i < frameCount; i++ )
            {
                var sprite = spriteHandler.ExtractSprite(i * spriteWidth, 0, spriteWidth, spriteHeight);
                playerSprites.Add(sprite);
            }

            var frames = new List<Image<Rgba32>>();

            for ( int i = 0; i < playerSprites.Count; i++ )
            {
                if (i == 4)
                {
                    targetAvatar.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.Vertical));
                    targetAvatar.Mutate(x => x.Glow());
                }
                if (i == 6)
                {
                    targetAvatar.Mutate(x => x.Glow());
                    targetAvatar.Mutate(x => x.RotateFlip(RotateMode.Rotate180, FlipMode.Vertical));
                }

                frames.Add(spriteHandler.CreateAnimationFrame(playerSprites[i], new Point(15, 25), targetAvatar, new Point(15 + offset, 25)));

            }

            return frames;
        }

        public Image<Rgba32> ResizeImage(Image<Rgba32> original, int width, int height)
        {
            try
            {
                var clone = original.Clone(ctx => ctx.Resize(width, height));
                return clone;
            }
            catch (Exception)
            {
                throw;
            }
            
        }


    }


}
