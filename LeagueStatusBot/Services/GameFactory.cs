using System;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace LeagueStatusBot.Services;

public class GameFactory
{
    private readonly IServiceProvider serviceProvider;
    public GameFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public GameManager Create(ulong gameKey)
    {
        var turnManager = serviceProvider.GetRequiredService<TurnManager>();
        var assetManager = serviceProvider.GetRequiredService<AssetManager>();
        var animationManager = serviceProvider.GetRequiredService<AnimationManager>();

        Console.WriteLine("When Is This");
        return new GameManager(turnManager, assetManager, animationManager, gameKey);
    }
}