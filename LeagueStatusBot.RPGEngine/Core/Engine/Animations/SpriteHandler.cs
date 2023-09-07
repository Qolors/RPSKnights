using LeagueStatusBot.RPGEngine.Core.Controllers;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class SpriteHandler
    {
        private Image<Rgba32> spriteSheet;
        private AssetManager assetManager;

        public SpriteHandler(AssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        public void LoadSpriteSheet(Image<Rgba32> image)
        {
            spriteSheet = image;
        }

        public Image<Rgba32> ExtractSprite(int x, int y, int width, int height)
        {
            if (spriteSheet == null)
                throw new InvalidOperationException("SpriteSheet not loaded.");

            return spriteSheet.Clone(ctx => ctx.Crop(new Rectangle(x, y, width, height)));
        }

        public void PlaceSpriteOnCanvas(Image<Rgba32> canvas, Image<Rgba32> sprite, int x, int y)
        {
            canvas.Mutate(ctx => ctx.DrawImage(sprite, new Point(x, y), 1));
        }

        public Image<Rgba32> ResizeImage(Image<Rgba32> original, int width, int height)
        {
            return original.Clone(ctx => ctx.Resize(width, height));
        }

        private void CreateBackground(Image<Rgba32> canvas)
        {
            var floortile = assetManager.GetTileset();
            var background = assetManager.GetBackground();
            PlaceSpriteOnCanvas(canvas, background, 0, 0);
            for (int x = 0; x < canvas.Width; x += floortile.Width)
            {
                PlaceSpriteOnCanvas(canvas, floortile, x, canvas.Height - floortile.Height);
            }
        }

        public Image<Rgba32> CreateAnimationFrame(Image<Rgba32> sprite1, Point position1, Image<Rgba32> sprite2, Point position2)
        {
            using (var frame = new Image<Rgba32>(250, 200))
            {
                CreateBackground(frame);
                PlaceSpriteOnCanvas(frame, sprite2, position2.X, position2.Y);
                PlaceSpriteOnCanvas(frame, sprite1, position1.X, position1.Y);
                return frame.Clone();
            }
        }
    }
}
