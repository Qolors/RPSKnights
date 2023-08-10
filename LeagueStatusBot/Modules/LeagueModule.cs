using Discord.Interactions;
using System.Net.Http;
using System.Threading.Tasks;
using LeagueStatusBot.Common.Models;
using Newtonsoft.Json;
using LeagueStatusBot.Helpers;
using Discord;
using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;

namespace LeagueStatusBot.Modules
{
    [Group("league", "league stats commands")]
    public class LeagueModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly HttpClient _httpClient;

        public LeagueModule()
        {
            _httpClient = new HttpClient();
        }


        [SlashCommand("championperformance", "Get Summoner's Champion Performances")]
        public async Task GetLastGameSummary(string userName)
        {
            await this.DeferAsync();

            string apiUrl = $"http://api:80/GameSummary?summonerName={userName}";
            var response = await _httpClient.GetAsync(apiUrl);

            var content = await response.Content.ReadAsStringAsync();

            var champs = JsonConvert.DeserializeObject<Dictionary<string, ChampionModel>>(content);

            Console.WriteLine(champs);

            // Sort champions based on the performance score and pick the top 5.
            var topChampions = champs
                .OrderByDescending(pair => CalculatePerformanceScore(pair.Value.Kills, pair.Value.Deaths, pair.Value.Assist, pair.Value.Wins, pair.Value.Losses, pair.Value.Wins + pair.Value.Losses))
                .Take(5)
                .ToList();

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine("```c");
            stringBuilder.AppendLine("TOP 5 CHAMPS IN LAST 45 RANKED GAMES BASED OFF SOME WEIGHTED PERFORMANCE SCORE THAT I TRUSTED WITH CHATGPT");
            stringBuilder.AppendLine("");
            stringBuilder.AppendLine($"FOR {userName.ToUpper()}");

            foreach (var (key, value) in topChampions)
            {
                var kda = KDAHelper.GetKDA(value.Kills, value.Deaths, value.Assist);
                var winrate = WinRateHelper.GetWinRate(value.Wins, value.Losses);
                var kdaString = $"{value.Kills}/{value.Deaths}/{value.Assist}";

                string resultString =
                    $"\n" +
                    $"{key} - W/L --> {value.Wins}/{value.Losses}\n" +
                    $"\n" +
                    $"KDA: {kdaString} ~{Math.Round(kda, 2)}\n" +
                    $"Win Rate: {winrate}%\n" +
                    $"Killing Sprees: {value.KillingSprees}\n" +
                    $"Total Damage Dealt to Champions: {value.TotalDamageDealtToChampions}\n" +
                    $"Total Healing on Teammates: {value.TotalHealingOnTeammates}\n" +
                    $"Total Shielding on Teammates: {value.TotalShieldingOnTeammates}\n";

                stringBuilder.Append(resultString);
            }

            stringBuilder.AppendLine("```");

            await FollowupAsync(stringBuilder.ToString());
        }

        [SlashCommand("matchhistory", "Get the last 10 Ranked Match History of a Summoner")]
        public async Task MatchHistorySummary(string summonerName)
        {
            await this.DeferAsync();

            string apiUrl = $"http://api:80/MatchHistory?summonerName={summonerName}";

            var response = await _httpClient.GetAsync(apiUrl);

            var content = await response.Content.ReadAsStringAsync();

            var matchHistory = JsonConvert.DeserializeObject<List<MatchHistoryModel>>(content);

            await FollowupAsync(GetChallengeString.GetTopChallengesString(matchHistory, summonerName));
        }

        public double CalculatePerformanceScore(int kills, int deaths, int assists, int wins, int losses, int gamesPlayed)
        {
            double kda = (kills + assists) / Math.Max(1.0, deaths); // Avoid division by zero
            double winRate = (double)wins / (wins + losses);
            double weight = 1 + Math.Log10(Math.Max(1, gamesPlayed)); // Avoid log(0)

            const double kdaWeight = 1.0;
            const double winRateWeight = 1.5;

            return weight * (kda * kdaWeight + winRate * winRateWeight);
        }
    }
}
