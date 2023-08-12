using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LeagueStatusBot.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueStatusBot.Services;

public class CommandHandlingService
{
    private readonly CommandService _commands;
    private readonly DiscordSocketClient _discord;
    private readonly IServiceProvider _services;

    public CommandHandlingService(IServiceProvider services)
    {
        _commands = services.GetRequiredService<CommandService>();
        _discord = services.GetRequiredService<DiscordSocketClient>();
        _services = services;
        _commands.CommandExecuted += CommandExecutedAsync;
        _discord.MessageReceived += MessageReceivedAsync;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModuleAsync<HelpHandler>(_services);
    }

    private async Task MessageReceivedAsync(SocketMessage rawMessage)
    {
        if (rawMessage is not SocketUserMessage {Source: MessageSource.User} message) 
            return;

        if ((message.Channel.Id == 702684769200111716 || message.Channel.Id == 958560743517720597) && 
            message.Author.Id != 331308445166731266)
        {
            SocketGuild guild = ((SocketGuildChannel)message.Channel).Guild;
            IEmote emote = guild.Emotes.First(e => e.Name == "chris");
            await message.AddReactionAsync(emote);
        }
        
    }

    private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        if (!command.IsSpecified)
        {
            Console.WriteLine("No Command Found");
            return;
        }

        if (result.IsSuccess)
            return;

        await context.Channel.SendMessageAsync($"error: {result}");
    }
}