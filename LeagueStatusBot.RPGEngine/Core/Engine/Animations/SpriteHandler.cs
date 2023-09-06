namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations 
{
    public class SpriteHandler 
    {
        private const int CANVAS_WIDTH = 150;
        private const int CANVAS_HEIGHT = 150;

        private Image<Rgba32> spriteSheet;

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

        public Image<Rgba32> CreateAnimationFrame(Image<Rgba32> sprite1, Point position1, Image<Rgba32> sprite2, Point position2) 
        {
            using (var frame = new Image<Rgba32>(200, 200))
            {
                PlaceSpriteOnCanvas(frame, sprite2, position2.X, position2.Y);
                PlaceSpriteOnCanvas(frame, sprite1, position1.X, position1.Y);
                return frame.Clone();
            }
        }

        public List<Image<Rgba32>> CreateMovingAnimation(Image<Rgba32> sprite1, Point start, Image<Rgba32> sprite2, Point end, int steps) 
        {
            var frames = new List<Image<Rgba32>>();

            for (int i = 0; i <= steps; i++) 
            {
                var position1 = new Point(start.X + (end.X - start.X) * i / steps, start.Y + (end.Y - start.Y) * i / steps);

                using (var frame = new Image<Rgba32>(CANVAS_WIDTH, CANVAS_HEIGHT))
                {
                    PlaceSpriteOnCanvas(frame, sprite2, end.X, end.Y);
                    PlaceSpriteOnCanvas(frame, sprite1, position1.X, position1.Y);
                    frames.Add(frame.Clone());
                }
            }

            return frames;
        }
    }
}
