﻿

namespace LeagueStatusBot.RPGEngine.Core.Engine.Beings;

/// <summary>
/// Player class is the Human player instances in a running game. Needs some revision eventually
/// </summary>
public class Player : Being
{
    public Player(Image<Rgba32> idleImage, ulong userId, string name, string url) : base(idleImage)
    {
        this.userId = userId;
        this.Name = name;
        this.url = url;
    }
    public string Name { get; set; }
    public int Health { get; set; } = 3;
    public int Energy { get; set; } = 5;
    public bool IsAlive => Health > 0;
    private readonly ulong userId;
    private readonly string url;
    public ulong GetUserId => userId;
    public string GetUrl => url;
}
