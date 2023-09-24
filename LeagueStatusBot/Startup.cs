using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Microsoft.EntityFrameworkCore;
using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using LeagueStatusBot.RPGEngine.Core.Engine.Animations;
using LeagueStatusBot.RPGEngine.Core.Engine.UI;
using LeagueStatusBot.Services;
using Microsoft.Extensions.DependencyInjection;
using LeagueStatusBot.RPGEngine.Data.Repository;

namespace LeagueStatusBot;

public class Startup
{
    private DiscordSocketClient _client;
        
    public async Task Initialize()
    {
        await using var services = ConfigureServices();

        _client = services.GetRequiredService<DiscordSocketClient>();
        _client.Ready += OnReady;
        _client.Log += LogAsync;

        services.GetRequiredService<CommandService>().Log += LogAsync;

        await services.GetRequiredService<InteractionHandlerService>().InitializeAsync();
        await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
        await _client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_BOT_TOKEN"));
        await _client.StartAsync();

        await Task.Delay(Timeout.Infinite);
    }
        
    private Task OnReady()
    {

        Console.WriteLine($"Connected to these servers as '{_client.CurrentUser.Username}': ");
        foreach (var guild in _client.Guilds) 
            Console.WriteLine($"- {guild.Name}");

        _client.SetGameAsync(Environment.GetEnvironmentVariable("DISCORD_BOT_ACTIVITY") ?? "I'm alive!", 
            type: ActivityType.CustomStatus);
        Console.WriteLine($"Activity set to '{_client.Activity.Name}'");

        return Task.CompletedTask;
    }

    private static Task LogAsync(LogMessage log)
    {
        Console.WriteLine(log.ToString());

        return Task.CompletedTask;
    }

    private static ServiceProvider ConfigureServices()
    {
        return new ServiceCollection()
            .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {
                MessageCacheSize = 50,
                LogLevel = LogSeverity.Info,
                AlwaysDownloadUsers= true,
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                CaseSensitiveCommands = false,
            }))
            .AddDbContext<GameDbContext>(options =>
            {
                options.UseSqlite("Data Source=/app/game.db");
            })
            .AddSingleton<PlayerRepository>()
            .AddSingleton<CommandHandlingService>()
            .AddSingleton(x => new InteractionService(x.GetRequiredService<DiscordSocketClient>()))
            .AddSingleton(new InteractiveConfig { DefaultTimeout = TimeSpan.FromMinutes(5) })
            .AddSingleton<InteractiveService>()
            .AddSingleton<InteractionHandlerService>()
            .AddSingleton<HttpClient>()
            .AddTransient<SpriteHandler>()
            .AddTransient<UIHandler>()
            .AddTransient<AnimationHandler>()
            .AddTransient<AnimationManager>()
            .AddTransient<AssetManager>()
            .AddTransient<TurnManager>()
            .AddTransient<GameFactory>()
            .BuildServiceProvider();
    }
}