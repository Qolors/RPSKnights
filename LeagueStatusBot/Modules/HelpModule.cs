using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace LeagueStatusBot.Modules;

public class HelpModule : ModuleBase<SocketCommandContext>
{
    private readonly CommandService _service;

    public HelpModule(CommandService service)
    {
        _service = service;
    }

    [SlashCommand("Help", "Show a brief description of all available commands")]
    public async Task HelpAsync()
    {
        var commands = _service.Commands.ToList();
        var embedBuilder = new EmbedBuilder();

        foreach (var command in commands)
        {
            // Get the command Summary attribute information
            var embedFieldText = command.Summary ?? "No description available\n";

            embedBuilder.AddField(command.Name, embedFieldText);
        }

        await ReplyAsync("Here's a list of all my commands and their descriptions: ", false, embedBuilder.Build());
    }
}