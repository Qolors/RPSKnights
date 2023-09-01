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
            spriteHandler = new();
        }

        public List<Image<Rgba32>> CreateIdleAnimation(Image image)
        {
            spriteHandler.LoadSpriteSheet(image);
            // Extract idle sprites from the sprite sheet
            var idleSprite1 = spriteHandler.ExtractSprite(1, 1, 16, 29);   // Coordinates for the 1st idle sprite
            var idleSprite2 = spriteHandler.ExtractSprite(19, 1, 16, 29);  // 2nd idle sprite
            var idleSprite3 = spriteHandler.ExtractSprite(37, 1, 16, 29);  // 3rd idle sprite
            var idleSprite4 = spriteHandler.ExtractSprite(55, 1, 16, 29);

            var frames = new List<Image<Rgba32>>();

            // Sequence the sprites to create the animation
            frames.Add(spriteHandler.CreateAnimationFrame(idleSprite1, new Point(50, 50), null, Point.Empty)); // Assuming character is centered at 50,50
            frames.Add(spriteHandler.CreateAnimationFrame(idleSprite2, new Point(50, 50), null, Point.Empty));
            frames.Add(spriteHandler.CreateAnimationFrame(idleSprite3, new Point(50, 50), null, Point.Empty));
            frames.Add(spriteHandler.CreateAnimationFrame(idleSprite4, new Point(50, 50), null, Point.Empty));
            
            return frames;
        }
    }

}
