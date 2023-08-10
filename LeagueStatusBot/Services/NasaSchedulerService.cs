using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using Discord.WebSocket;
using LeagueStatusBot.Common.Models;
using LeagueStatusBot.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace LeagueStatusBot.Services
{
    public class NasaSchedulerService
    {
        private const string LastRunTimeFileName = "/app/lastruntime.txt";

        private Timer _timer;
        private IServiceProvider _serviceProvider;
        private DiscordSocketClient client;

        public NasaSchedulerService(IServiceProvider serviceProvider, DiscordSocketClient client)
        {
            _serviceProvider = serviceProvider;
            this.client = client;
        }

        public async Task InitializeAsync()
        {
            client.Ready += SetupTimer;
        }

        private async Task SetupTimer()
        {
            _timer = new Timer(24 * 60 * 60 * 1000);  // 24 hours in milliseconds
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

            AdjustTimerInitialDelay();
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            await PingApiEndpoint();
        }

        private async Task PingApiEndpoint()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync("http://api:80/NasaAPOD");

                if (response.IsSuccessStatusCode)
                {
                    var model = await response.Content.ReadAsStringAsync();

                    var spaceModel = JsonConvert.DeserializeObject<SpaceModel>(model);

                    await PostToFeedChannel.PostNasaToFeedChannelAsync(spaceModel, client);
                }
            }

            SaveLastRunTime(DateTime.UtcNow);
        }

        private void SaveLastRunTime(DateTime dateTime)
        {
            File.WriteAllText(LastRunTimeFileName, dateTime.ToString("o"));
        }

        private DateTime? GetLastRunTime()
        {
            if (File.Exists(LastRunTimeFileName))
            {
                var dateTimeStr = File.ReadAllText(LastRunTimeFileName);
                if (!string.IsNullOrWhiteSpace(dateTimeStr))
                {
                    if (DateTime.TryParse(dateTimeStr, null, DateTimeStyles.RoundtripKind, out DateTime parsedDate))
                    {
                        return parsedDate;
                    }
                }
            }

            return null;
        }

        private void AdjustTimerInitialDelay()
        {
            var lastRunTime = GetLastRunTime();

            if (!lastRunTime.HasValue)
            {
                Console.WriteLine("Firing Picture of the Day");
                Task.Run(() => PingApiEndpoint());
                _timer.Interval = 24 * 60 * 60 * 1000; // 24 hours
                return;
            }

            var timeSinceLastRun = DateTime.UtcNow - lastRunTime.Value;
            var initialDelay = (24 * 60 * 60 * 1000) - timeSinceLastRun.TotalMilliseconds;

            if (initialDelay < 0) initialDelay = 0;

            _timer.Interval = initialDelay;
            _timer.Elapsed += (s, e) => { _timer.Interval = 24 * 60 * 60 * 1000; };
        }
    }
}
