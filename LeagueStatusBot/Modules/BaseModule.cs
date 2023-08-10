using System.Net.Http;
using System.Text;
using System;
using System.Threading.Tasks;
using Discord.Commands;
using Newtonsoft.Json;

namespace LeagueStatusBot.Modules;

public class BaseModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Test if Bonk is online")]
    public Task PongAsync()
        => ReplyAsync("Pong from Bonk.");
}