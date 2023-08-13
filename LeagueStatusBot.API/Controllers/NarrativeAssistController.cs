using LeagueStatusBot.API.Services;
using LeagueStatusBot.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace LeagueStatusBot.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NarrativeAssistController
    {
        private readonly ChatGPTService chatGPTService;

        public NarrativeAssistController(ChatGPTService chatGPTService)
        {
            this.chatGPTService = chatGPTService;
        }

        [HttpGet(Name = "StartAdventure")]
        public async Task<string> Get(string prompt)
        {
            var results = await chatGPTService.GenerateCompletionAsync(prompt);

            return results;
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class NarrativeAddOnController
    {
        private readonly ChatGPTService chatGPTService;
        public NarrativeAddOnController(ChatGPTService chatGPTService)
        {
            this.chatGPTService= chatGPTService;
        }

        [HttpGet(Name = "ContinueAdventure")]
        public async Task<string> Get(string prompt)
        {
            var results = await chatGPTService.GenerateARound(prompt);

            return results;
        }
    }
}
