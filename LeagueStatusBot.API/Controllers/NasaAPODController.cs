using LeagueStatusBot.API.Services;
using LeagueStatusBot.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeagueStatusBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NasaAPODController : ControllerBase
    {
        private readonly NasaAPIService apiService;

        public NasaAPODController(NasaAPIService apiService)
        {
            this.apiService = apiService;
        }

        [HttpGet(Name = "GetAPOD")]
        public async Task<SpaceModel> Get()
        {
            return await apiService.GetSpacePictureOfTheDay();
        }
    }
}
