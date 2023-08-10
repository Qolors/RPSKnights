using LeagueStatusBot.API.Models;
using LeagueStatusBot.API.Services;
using LeagueStatusBot.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeagueStatusBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameSummaryController : ControllerBase
    {   
        private readonly ILogger<GameSummaryController> _logger;
        private readonly RiotAPIService _apiService;

        public GameSummaryController(ILogger<GameSummaryController> logger, RiotAPIService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        [HttpGet(Name = "GetGameInfo")]
        public async Task<Dictionary<string, ChampionModel>> Get(string summonerName)
        {
            var results = await _apiService.GetGameSummary(summonerName.ToLower());

            return results;
        }
    }
}