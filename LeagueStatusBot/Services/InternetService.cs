using System;
using System.Net.Http;
using Discord;
using SixLabors.ImageSharp.PixelFormats;

namespace LeagueStatusBot.Services
{
    public class InternetService
    {
        private HttpClient httpClient;
        public InternetService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        //TODO --> SET UP LOADING AVATARS
    }
}