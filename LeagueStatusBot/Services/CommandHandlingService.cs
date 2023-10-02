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
    private readonly IServiceProvider _services;

    public CommandHandlingService(IServiceProvider services)
    {
        _commands = services.GetRequiredService<CommandService>();
        _services = services;
        _commands.CommandExecuted += CommandExecutedAsync;
    }

    public async Task InitializeAsync()
    {
        await _commands.AddModuleAsync<HelpHandler>(_services);
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