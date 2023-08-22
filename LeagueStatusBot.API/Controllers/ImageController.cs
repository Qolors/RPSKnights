using Microsoft.AspNetCore.Mvc;

namespace LeagueStatusBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly string _imageBasePath = "Images";

        [HttpGet("GetMonsterPortrait/{name}")]
        public IActionResult GetMonsterPortrait(string name)
        {
            var imagePath = Path.Combine(_imageBasePath, "Monsters", $"{name}.png");
            return ServeImage(imagePath);
        }

        [HttpGet("GetAbilityImage/{name}")]
        public IActionResult GetAbilityImage(string name)
        {
            var imagePath = Path.Combine(_imageBasePath, "Abilities", $"{name}.png");
            return ServeImage(imagePath);
        }

        [HttpGet("GetArmorImage/{name}")]
        public IActionResult GetArmorImage(string name)
        {
            var imagePath = Path.Combine(_imageBasePath, "Armors", $"{name}.png");
            return ServeImage(imagePath);
        }

        [HttpGet("GetWeaponImage/{name}")]
        public IActionResult GetWeaponImage(string name)
        {
            var imagePath = Path.Combine(_imageBasePath, "Weapons", $"{name}.png");
            return ServeImage(imagePath);
        }

        [HttpGet("GetPortalImage")]
        public IActionResult GetPortalImage()
        {
            var imagePath = Path.Combine(_imageBasePath, "Portal", "portal.png");
            return ServeImage(imagePath);
        }

        private IActionResult ServeImage(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var imageBytes = System.IO.File.ReadAllBytes(path);
                return File(imageBytes, "image/png"); // Assumes all images are PNG. Adjust if necessary.
            }
            return NotFound();
        }
    }
}
