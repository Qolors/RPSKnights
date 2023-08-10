using Microsoft.AspNetCore.Mvc;
using LeagueStatusBot.API.Services;
using LeagueStatusBot.Common.Models;

namespace LeagueStatusBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MatchHistoryController : ControllerBase
    {
        private readonly ILogger<MatchHistoryController> _logger;
        private readonly RiotAPIService _apiService;
        public MatchHistoryController(ILogger<MatchHistoryController> logger, RiotAPIService apiService)
        {
            _apiService = apiService;
            _logger = logger;
        }

        [HttpGet(Name = "GetMatchHistory")]
        public async Task<List<MatchHistoryModel>> Get(string summonerName)
        {
            var results = await _apiService.GetMatchHistory(summonerName);

            return results;
        }
    }
}
