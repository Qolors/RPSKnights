using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LeagueStatusBot.Modules;
using System;
using System.Threading.Tasks;

namespace LeagueStatusBot.Services
{
    public class InteractionHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private readonly IServiceProvider _services;

        public InteractionHandlerService(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
        {
            _client = client;
            _handler = handler;
            _services = services;
        }

        public async Task InitializeAsync()
        {
            _client.Ready += ReadyAsync;

            await _handler.AddModuleAsync<RPGModule>(_services);

            _client.InteractionCreated += HandleInteractionAsync;
        }

        private async Task ReadyAsync()
        {
            if (ulong.TryParse(Environment.GetEnvironmentVariable("DISCORD_MAIN_GUILD"), out ulong guildId))
            {
                await _handler.RegisterCommandsToGuildAsync(guildId, true);
            }
            else
            {
                // Handle the error: the environment variable value wasn't a valid ulong.
                Console.WriteLine("Invalid DISCORD_MAIN_GUILD environment variable value.");
            }
        }

        private async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            try
            {
                // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
                var context = new SocketInteractionContext(_client, interaction);

                // Execute the incoming command.
                var result = await _handler.ExecuteCommandAsync(context, _services);

                if (!result.IsSuccess)
                    switch (result.Error)
                    {
                        case InteractionCommandError.UnmetPrecondition:
                            // implement
                            break;
                        default:
                            break;
                    }
            }
            catch
            {
                // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
                // response, or at least let the user know that something went wrong during the command execution.
                if (interaction.Type is InteractionType.ApplicationCommand)
                    await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }

    }
}
