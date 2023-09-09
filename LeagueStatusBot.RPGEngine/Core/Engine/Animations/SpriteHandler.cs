using LeagueStatusBot.RPGEngine.Core.Controllers;
using LeagueStatusBot.RPGEngine.Core.Engine.UI;

namespace LeagueStatusBot.RPGEngine.Core.Engine.Animations
{
    public class SpriteHandler
    {
        private Image<Rgba32> spriteSheet;
        private AssetManager assetManager;
        private UIHandler interfacehandler;

        public SpriteHandler(AssetManager assetManager, UIHandler interfaceHandler)
        {
            this.assetManager = assetManager;
            this.interfacehandler = interfaceHandler;
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

        public void PlaceHealthbarUIOnCanvas(Image<Rgba32> canvas, Image<Rgba32> sprite, Image<Rgba32> avatar, bool isPlayer1)
        {
            Point side = isPlayer1 ? new Point(5, 60) : new Point(250 - 55, 60);
            Point portrait = isPlayer1 ? new Point(5, 5) : new Point(250 - avatar.Width - 5, 5);
            canvas.Mutate(ctx => ctx.DrawImage(sprite, side, 1));
            canvas.Mutate(ctx => ctx.DrawImage(avatar, portrait, 1));
        }

        private Image<Rgba32> ResizeAvatar(Image<Rgba32> original, int width, int height)
        {
            original.Mutate(x => x.Pixelate());
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

        //TODO --> MAKE THIS INTAKE A MESSAGING MODEL
        public Image<Rgba32> CreateAnimationFrame(Image<Rgba32> sprite1, Point position1, Image<Rgba32> sprite2, Point position2, int health1, int health2)
        {

            var (avatar1, avatar2) = assetManager.GetPlayerAvatars();

            var pixelAvatar1 = ResizeAvatar(avatar1.CloneAs<Rgba32>(), 50, 50);
            var pixelAvatar2 = ResizeAvatar(avatar2.CloneAs<Rgba32>(), 50, 50);


            using (var frame = new Image<Rgba32>(250, 200))
            {
                CreateBackground(frame);
                PlaceHealthbarUIOnCanvas(frame, interfacehandler.GenerateHealthbar(health1, true), pixelAvatar1, true);
                PlaceHealthbarUIOnCanvas(frame, interfacehandler.GenerateHealthbar(health2, false), pixelAvatar2, false);
                PlaceSpriteOnCanvas(frame, sprite1, position1.X, position1.Y);
                PlaceSpriteOnCanvas(frame, sprite2, position2.X, position2.Y);
                return frame.Clone();
            }
        }
    }
}
