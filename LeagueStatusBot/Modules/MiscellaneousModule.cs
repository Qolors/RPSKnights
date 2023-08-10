using Discord.Interactions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LeagueStatusBot.Common.Models;

namespace LeagueStatusBot.Modules
{
    public class MiscellaneousModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly HttpClient _httpClient;

        public MiscellaneousModule()
        {
            _httpClient = new HttpClient();
        }

        [SlashCommand("kanye", "Get A Famous Quote from Kanye")]
        public async Task GetKanyeQuote()
        {
            string url = "https://api.kanye.rest/";
            var response = await _httpClient.GetAsync(url);

            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                var kanye = JsonConvert.DeserializeObject<KanyeQuote>(content);

                var quote = $"""

                    "*{kanye.Quote}*"
                                \- Kanye West
                    """;

                await RespondAsync(quote);
            }
        }
    }
}
