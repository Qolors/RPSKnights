
using System.Drawing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using Image = SixLabors.ImageSharp.Image;

namespace LeagueStatusBot.RPGEngine.Core.Controllers;

/// <summary>
/// AssetManager manages the loading of local and non-local assets
/// </summary>
public class AssetManager
{
    private Dictionary<string, Image<Rgba32>> uiSprites = new();
    private Dictionary<string, Image<Rgba32>> entitySpritesBlue = new();
    private Dictionary<string, Image<Rgba32>> entitySpritesRed = new();
    private Dictionary<string, Image<Rgba32>> abilitySprites = new();
    private Image<Rgba32> tilesetImage;
    private Image<Rgba32> backgroundImage;

    private Image player1Avatar;
    private Image player2Avatar;

    private HttpClient httpClient;

    public AssetManager()
    {
        LoadTileset();
        LoadEntitySprites();
        LoadInterfaceSprites();
        httpClient = new();
    }

    private void LoadTileset()
    {
        // Load your tileset image
        tilesetImage = Image.Load<Rgba32>("./Assets/Maps/Floor.png");
        backgroundImage = Image.Load<Rgba32>("./Assets/Maps/Sky.png");
    }

    private void LoadEntitySprites()
    {
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

    private void LoadInterfaceSprites()
    {
        foreach (var file in Directory.GetFiles("./Assets/Sprites/UI"))
        {
            var spriteName = Path.GetFileNameWithoutExtension(file);
            uiSprites[spriteName] = (Image<Rgba32>)Image.Load(file);
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

    public Image<Rgba32> GetInterfaceSprite(string spriteName)
    {
        return uiSprites.TryGetValue(spriteName, out var sprite) ? sprite : new Image<Rgba32>(200, 200);
    }

    public Image<Rgba32> GetTileset()
    {
        return tilesetImage;
    }

    public Image<Rgba32> GetBackground()
    {
        return backgroundImage;
    }

    public async Task LoadPlayer1Avatar(string url)
    {
        using (var bytes = await httpClient.GetStreamAsync(url))
        {
            var image = Image.Load(bytes);
            player1Avatar = image;
        }
    }

    public async Task LoadPlayer2Avatar(string url)
    {
        using (var bytes = await httpClient.GetStreamAsync(url))
        {
            var image = Image.Load(bytes);
            player2Avatar = image;
        }
    }

    public (Image, Image) GetPlayerAvatars()
    {
        return (player1Avatar, player2Avatar);
    }
}


