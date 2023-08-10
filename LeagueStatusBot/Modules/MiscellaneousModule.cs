using Discord.Interactions;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LeagueStatusBot.Common.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace LeagueStatusBot.Modules
{
    public class MiscellaneousModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly HttpClient _httpClient;
        private const string TodoFileName = "/app/todos.txt";

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


        [SlashCommand("tasks", "Get current list of tasks for Bonk Development")]
        public async Task GetTasks()
        {
            if (Context.User.Id != 331308445166731266)
            {
                await RespondAsync("You're not KKat you fkn pez");
                return;
            }

            StringBuilder sb = new StringBuilder();

            var lines = File.ReadAllLines(TodoFileName).ToList();

            sb.AppendLine("**Bonk Development Task List**");
            sb.AppendLine("```csharp");

            for (int i = 0; i < lines.Count; i++)
            {
                sb.AppendLine($"[{i + 1}] ---> {lines[i]}");
            }

            sb.AppendLine("```");

            await RespondAsync(sb.ToString());
        }

        [SlashCommand("delete-task", "Delete a completed Task")]
        public async Task DeleteTask(int index)
        {
            if (Context.User.Id != 331308445166731266)
            {
                await RespondAsync("You're not KKat you fkn pez");
                return;
            }

            int zeroindex = index - 1;

            var lines = File.ReadAllLines(TodoFileName).ToList();

            var newLines = new List<string>();

            for (int i = 0; i < lines.Count; i++)
            {
                if (i == zeroindex) continue;

                newLines.Add(lines[i]);
            }

            File.WriteAllLines(TodoFileName, newLines);

            await RespondAsync($"**Task #[{index}] deleted.**");
        }

        [SlashCommand("add-task", "Add a Bonk Development Task")]
        public async Task AddTask(string task)
        {
            if (Context.User.Id != 331308445166731266)
            {
                await RespondAsync("You're not KKat you fkn pez");
                return;
            }

            var lines = File.ReadAllLines(TodoFileName).ToList();

            lines.Add(task);

            File.WriteAllLines(TodoFileName, lines);

            await RespondAsync($"**New Task: '{task}' Added.**");
        }
    }
}
