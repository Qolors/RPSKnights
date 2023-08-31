using LeagueStatusBot.API.Models;
using LeagueStatusBot.Common.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace LeagueStatusBot.API.Services
{
    public class RiotAPIService
    {

        const int MAX_MATCHES = 45;
        const int HISTORY_MATCHES = 10;

        private readonly HttpClient _httpClient;
        private readonly string ApiKey;

        public RiotAPIService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ApiKey = Environment.GetEnvironmentVariable("RIOT_API_TOKEN");
        }

        public async Task<List<MatchHistoryModel>> GetMatchHistory(string userName)
        {
            var summonerModel = await GetSummonerByName(userName);

            if (summonerModel == null)
            {
                throw new Exception($"Error retrieving summoner data for user: {userName}.");
            }

            var allMatchId = await GetAllMatchId(summonerModel.Puuid, HISTORY_MATCHES);

            if (allMatchId.Count == 0)
            {
                throw new Exception($"No matches found for user: {userName}.");
            }

            var historyList = new List<MatchHistoryModel>();

            foreach (var match in allMatchId)
            {
                try
                {
                    var matchHistory = await GetThisMatchHistory(match, summonerModel.Puuid);
                    historyList.Add(matchHistory);
                }
                catch(Exception ex)
                {
                    continue;
                }
                
            }

            return historyList;
        }

        public async Task<Dictionary<string, ChampionModel>> GetGameSummary(string userName)
        {
            var summonerModel = await GetSummonerByName(userName);
            if (summonerModel == null)
            {
                throw new Exception($"Error retrieving summoner data for user: {userName}.");
            }

            var allMatchId = await GetAllMatchId(summonerModel.Puuid, MAX_MATCHES);
            if (allMatchId.Count == 0)
            {
                throw new Exception($"No matches found for user: {userName}.");
            }

            var championsModel = await GetMatches(allMatchId, summonerModel.Puuid);
            if (championsModel == null)
            {
                throw new Exception($"Error retrieving match data for match: {allMatchId}.");
            }

            return championsModel;
        }

        private async Task<SummonerModel> GetSummonerByName(string userName)
        {
            var response = await _httpClient.GetAsync($"https://na1.api.riotgames.com/lol/summoner/v4/summoners/by-name/{userName}?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<SummonerModel>(results);
            }

            return null;
        }

        private async Task<List<string>> GetAllMatchId(string puuid, int numMatches)
        {
            var response = await _httpClient.GetAsync($"https://americas.api.riotgames.com/lol/match/v5/matches/by-puuid/{puuid}/ids?queue=420&start=0&count={numMatches}&api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var matches = JsonConvert.DeserializeObject<List<string>>(results);
                Console.WriteLine(matches);
                return matches;
            }

            return null;
        }

        private async Task<MatchHistoryModel> GetThisMatchHistory(string matchId, string puuid)
        {
            var response = await _httpClient.GetAsync($"https://americas.api.riotgames.com/lol/match/v5/matches/{matchId}?api_key={ApiKey}");

            if (response.IsSuccessStatusCode)
            {

                var results = await response.Content.ReadAsStringAsync();
                var matchModel = JsonConvert.DeserializeObject<MatchModel>(results);

                Participant? participant = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid);

                MatchHistoryModel matchHistoryModel = new MatchHistoryModel()
                {
                    Champion = participant.ChampionName,
                    Kills = participant.Kills,
                    Deaths = participant.Deaths,
                    Assists = participant.Assists,
                    DamageDealt = participant.TotalDamageDealtToChampions,
                    HealingDone = participant.TotalHealsOnTeammates,
                    IsCombat = participant.TeamPosition == "UTILITY" ? false : true,
                    ShieldingDone = participant.TotalDamageShieldedOnTeammates,
                    Victory = participant.Win ? "WIN" : "LOSS"
                };

                return matchHistoryModel;

            }

            return null;

        }

        private async Task<Dictionary<string, ChampionModel>> GetMatches(List<string> matchIds, string puuid)
        {
            var championStats = new Dictionary<string, ChampionModel>();

            foreach (var match in matchIds)
            {
                await Task.Delay(250);

                try
                {
                    var response = await _httpClient.GetAsync($"https://americas.api.riotgames.com/lol/match/v5/matches/{match}?api_key={ApiKey}");

                    if (response.IsSuccessStatusCode)
                    {
                        var results = await response.Content.ReadAsStringAsync();
                        var matchModel = JsonConvert.DeserializeObject<MatchModel>(results);

                        var championName = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).ChampionName;

                        

                        if (championStats.ContainsKey(championName))
                        {
                            championStats[championName].Kills += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Kills;
                            championStats[championName].Deaths += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Deaths;
                            championStats[championName].Assist += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Assists;
                            championStats[championName].Wins += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Win ? 1 : 0;
                            championStats[championName].Losses += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Win ? 0 : 1;
                            championStats[championName].KillingSprees += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).KillingSprees;
                            championStats[championName].TotalDamageDealtToChampions += matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).TotalDamageDealtToChampions;

                            Console.WriteLine(championStats[championName].Wins);
                        }
                        else
                        {
                            Console.WriteLine(championName);
                            championStats.Add(championName, new ChampionModel
                            {
                                Kills = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Kills,
                                Deaths = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Deaths,
                                Assist = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Assists,
                                Wins = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Win ? 1 : 0,
                                Losses = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).Win ? 0 : 1,
                                KillingSprees = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).KillingSprees,
                                TotalDamageDealtToChampions = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).TotalDamageDealtToChampions,
                                TotalHealingOnTeammates = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).TotalHealsOnTeammates,
                                TotalShieldingOnTeammates = matchModel.Info.Participants.FirstOrDefault(x => x.Puuid == puuid).TotalDamageShieldedOnTeammates
                            });
                        }

                    }
                }
                catch (Exception ex)
                {
                    continue;
                }

                

                
            }

            return championStats;
        }
    }
}
