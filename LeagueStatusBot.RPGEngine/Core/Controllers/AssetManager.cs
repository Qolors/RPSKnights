
using System.Drawing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class AssetManager
    {
        // Use dictionaries to load and manage assets for quick look-up.
        private Dictionary<string, Image> entitySprites = new();
        private Dictionary<string, Image> abilitySprites = new();
        private Image tilesetImage;

        public AssetManager()
        {
            LoadTileset();
            LoadEntitySprites();
            //LoadAbilitySprites();
        }

        private void LoadTileset()
        {
            // Load your tileset image
            tilesetImage = Image.Load("./Assets/Maps/floor_1.png");
        }

        private void LoadEntitySprites()
        {
            // Load all entity sprites. This is simplified; consider error-checking and other concerns.
            foreach (var file in Directory.GetFiles("./Assets/Sprites"))
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                entitySprites[spriteName] = Image.Load(file);
            }
        }

        private void LoadAbilitySprites()
        {
            // Similar approach for ability sprites
            foreach (var file in Directory.GetFiles("Assets/Sprites/Abilities"))
            {
                var spriteName = Path.GetFileNameWithoutExtension(file);
                abilitySprites[spriteName] = Image.Load(file);
            }
        }

        // Getters to access loaded assets
        public Image? GetEntitySprite(string spriteName)
        {
            return entitySprites.TryGetValue(spriteName, out var sprite) ? sprite : null;
        }

        public Image? GetAbilitySprite(string spriteName)
        {
            return abilitySprites.TryGetValue(spriteName, out var sprite) ? sprite : null;
        }

        public Image GetTileset()
        {
            return tilesetImage;
        }
    }

}
