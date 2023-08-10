using LeagueStatusBot.Common.Models;
using Newtonsoft.Json;

namespace LeagueStatusBot.API.Services
{
    public class NasaAPIService
    {
        private HttpClient _httpClient;
        private readonly string ApiKey;

        public NasaAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ApiKey = Environment.GetEnvironmentVariable("NASA_API_TOKEN");
        }

        public async Task<SpaceModel?> GetSpacePictureOfTheDay()
        {
            var response = await _httpClient.GetAsync($"https://api.nasa.gov/planetary/apod?api_key={ApiKey}");


            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SpaceModel>(content);
            }

            return null;
        }
    }
}
