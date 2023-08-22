using LeagueStatusBot.Common.Models;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LeagueStatusBot.Helpers
{
    public static class GetChallengeString
    {
        public static string GetTopChallengesString(List<MatchHistoryModel> matchHistory, string summonerName)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.AppendLine($"**Match History for {summonerName}**");

            stringBuilder.AppendLine("```csharp");
            foreach (var match in matchHistory)
            {
                var numberStat = match.HealingDone > match.DamageDealt || 
                    match.ShieldingDone > match.DamageDealt ? 
                    $"Healing/Shielding --> Shield --> {match.ShieldingDone} - Heal --> {match.HealingDone}" :
                    $"Dmg To Champs --> {match.DamageDealt}";

                stringBuilder
                    .AppendLine($"{match.Victory}   ---> {match.Champion} - KDA --> {match.Kills}/{match.Deaths}/{match.Assists} - {numberStat}");
                stringBuilder.AppendLine("");
            }
            stringBuilder.AppendLine("```");

            return stringBuilder.ToString();
        }
    }
}
