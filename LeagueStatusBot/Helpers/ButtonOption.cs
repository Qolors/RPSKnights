using Discord;

namespace LeagueStatusBot.Helpers;
public record ButtonOption<T>(T Option, ButtonStyle Style, bool disable); // An option with an style