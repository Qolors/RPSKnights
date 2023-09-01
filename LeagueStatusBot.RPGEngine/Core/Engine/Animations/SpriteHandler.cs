using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    

    public class SpriteHandler
    {
        private Image<Rgba32> spriteSheet;

        public SpriteHandler()
        {
        }

        public void LoadSpriteSheet(Image image)
        {
            spriteSheet = (Image<Rgba32>)image;
        }

        // Extract a sprite from the sprite sheet
        public Image<Rgba32> ExtractSprite(int x, int y, int width, int height)
        {
            // Crop the sprite out
            var sprite = spriteSheet.Clone(ctx => ctx.Crop(new Rectangle(x, y, width, height)));
            return sprite;
        }

        // Place a sprite onto another image (canvas)
        public void PlaceSpriteOnCanvas(Image<Rgba32> canvas, Image<Rgba32> sprite, int x, int y)
        {
            canvas.Mutate(ctx => ctx.DrawImage(sprite, new Point(x, y), 1));
        }

        // Create an animation frame using two sprites for simplicity (extend as needed)
        public Image<Rgba32> CreateAnimationFrame(Image<Rgba32> sprite1, Point position1, Image<Rgba32> sprite2, Point position2)
        {
            var frame = new Image<Rgba32>(100, 100); // Assuming a canvas size of 100x100
            PlaceSpriteOnCanvas(frame, sprite1, position1.X, position1.Y);
            //PlaceSpriteOnCanvas(frame, sprite2, position2.X, position2.Y);
            return frame;
        }
    }

}
