using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LeagueStatusBot.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace LeagueStatusBot.Services
{
    public class InteractionHandlerService
    {
        private readonly DiscordSocketClient _client;
        private readonly InteractionService _handler;
        private GameControllerService gameControllerService;
        private readonly IServiceProvider _services;

        public InteractionHandlerService(DiscordSocketClient client, InteractionService handler, IServiceProvider services)
        {
            _client = client;
            _handler = handler;
            _services = services;
            gameControllerService = _services.GetRequiredService<GameControllerService>();
        }

        public async Task InitializeAsync()
        {
            _client.Ready += ReadyAsync;

            await _handler.AddModuleAsync<LeagueModule>(_services);
            await _handler.AddModuleAsync<MiscellaneousModule>(_services);
            await _handler.AddModuleAsync<RPGModule>(_services);

            _client.InteractionCreated += HandleInteractionAsync;
            _client.ButtonExecuted += HandleButtonAsync;
            _client.SelectMenuExecuted += HandleSelectMenuAsync;
            _client.ModalSubmitted += async modal =>
            {
                List<SocketMessageComponentData> components =
                modal.Data.Components.ToList();
                string food = components
                    .First(x => x.CustomId == "action").Value;

                gameControllerService.SetPlayerTargetString(modal.User.Id, food);

                await modal.RespondAsync("Submitted");
            };
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

        private async Task HandleButtonAsync(SocketMessageComponent component)
        {
            switch(component.Data.CustomId)
            {
                case "perform-actions":
                    await HandleTurnActionAsync(component);
                    break;

                case "attack":
                    break;

                case "defend":
                    break;

                case "skill1":
                    break;

                case "skill2":
                    break;

                default:
                    break;
            }

        }

        private async Task HandleSelectMenuAsync(SocketMessageComponent args)
        {
            switch (args.Data.CustomId)
            {
                case "target-select":
                    await HandleTargetSelectAsync(args, args.Data.Values.First());
                    break;

                default:
                    break;
            }
        }

        private async Task HandleTargetSelectAsync(SocketMessageComponent args, string target)
        {
            var playerTurn = gameControllerService.ReceiveRequest(args.User.Id);

            if (playerTurn == null)
            {
                await args.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {
                gameControllerService.SetPlayerTarget(playerTurn, target);

                await args.UpdateAsync(x =>
                {
                    x.Content = $"{playerTurn.Name} Has Targeted {args.Data.Values.First()}";
                    x.Components = new ComponentBuilder().WithButton("Attack", "attack").WithButton("Defend", "defend").Build();
                });
            }
        }

        private async Task HandleTurnActionAsync(SocketMessageComponent component)
        {
            var playerTurn = gameControllerService.ReceiveRequest(component.User.Id);

            if (playerTurn == null)
            {
                await component.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {

                var mb = new ModalBuilder()
                    .WithCustomId("action_menu")
                    .WithTitle($"What Will {component.User.GlobalName} do?")
                    .AddTextInput("I Will do this", "action", TextInputStyle.Paragraph);

                await component.RespondWithModalAsync(mb.Build());
            }
        }

        
    }
}
