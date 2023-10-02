using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace LeagueStatusBot.Modules;

public class HelpModule : InteractionModuleBase<SocketInteractionContext>
{
    private readonly CommandService _service;

    public HelpModule(CommandService service)
    {
        _service = service;
    }

    [SlashCommand("help", "Show a brief description of all available commands")]
    public async Task HelpAsync()
    {
        var embedBuilder = new EmbedBuilder();

        embedBuilder.AddField("/challenge", "Challenge another user to a duel");
        embedBuilder.AddField("/leaderboard", "Get the leaderboard stats for this server");
        embedBuilder
        .WithTitle("RPS Knights Commands Info")
        .WithUrl("https://github.com/Qolors/RPSKnights")
        .WithDescription("For gameplay tutorial and understanding, please visit the docs [here](https://qolors.github.io/RPSKnights/)")
        .WithFooter("Issues and need assistance? Please reach out to me on Discord - darktideplayer192");

        await RespondAsync(embed: embedBuilder.Build());
    }
}