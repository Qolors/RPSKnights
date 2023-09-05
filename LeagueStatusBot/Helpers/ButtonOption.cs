using Discord;

namespace LeagueStatusBot.Helpers;
public record ButtonOption<T>(T Option, ButtonStyle Style); // An option with an style