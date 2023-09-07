
using System.Drawing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class AssetManager
    {
        // Use dictionaries to load and manage assets for quick look-up.
        private Dictionary<string, Image<Rgba32>> entitySpritesBlue = new();
        private Dictionary<string, Image<Rgba32>> entitySpritesRed = new();
        private Dictionary<string, Image<Rgba32>> abilitySprites = new();
        private Image<Rgba32> tilesetImage;
        private Image<Rgba32> backgroundImage;

        public AssetManager()
        {
            LoadTileset();
            LoadEntitySprites();
            //LoadAbilitySprites();
        }

        private void LoadTileset()
        {
            // Load your tileset image
            tilesetImage = Image.Load<Rgba32>("./Assets/Maps/Floor.png");
            backgroundImage = Image.Load<Rgba32>("./Assets/Maps/Sky.png");
        }

        private void LoadEntitySprites()
        {
            // Load all entity sprites. This is simplified; consider error-checking and other concerns.
            foreach (var file in Directory.GetFiles("./Assets/Sprites/Blue"))
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                entitySpritesBlue[spriteName] = (Image<Rgba32>)Image.Load(file);
            }
            foreach (var file in Directory.GetFiles("./Assets/Sprites/Red"))
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                entitySpritesRed[spriteName] = (Image<Rgba32>)Image.Load(file);
            }
        }

        private void LoadAbilitySprites()
        {
            // Similar approach for ability sprites
            foreach (var file in Directory.GetFiles("Assets/Sprites/Abilities"))
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                abilitySprites[spriteName] = (Image<Rgba32>)Image.Load(file);
            }
        }

        // Getters to access loaded assets
        public Image<Rgba32> GetEntitySprite(string spriteName, bool red)
        {
            return red 
            ? entitySpritesRed.TryGetValue(spriteName, out var bluesprite) ? bluesprite : new Image<Rgba32>(56, 56)
            : entitySpritesBlue.TryGetValue(spriteName, out var redsprite) ? redsprite : new Image<Rgba32>(56, 56);
        }

        public Image GetAbilitySprite(string spriteName)
        {
            return abilitySprites.TryGetValue(spriteName, out var sprite) ? sprite : new Image<Rgba32>(200, 200);
        }

        public Image<Rgba32> GetTileset()
        {
            return tilesetImage;
        }

        public Image<Rgba32> GetBackground()
        {
            return backgroundImage;
        }
    }

}
