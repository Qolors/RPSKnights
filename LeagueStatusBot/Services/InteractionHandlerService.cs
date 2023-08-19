using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using LeagueStatusBot.Modules;
using LeagueStatusBot.RPGEngine.Core.Engine;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
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

            await _handler.AddModuleAsync<MiscellaneousModule>(_services);
            await _handler.AddModuleAsync<RPGModule>(_services);

            _client.InteractionCreated += HandleInteractionAsync;
            _client.ButtonExecuted += HandleButtonAsync;
            _client.SelectMenuExecuted += HandleSelectMenuAsync;
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
                case "join-party":
                    await HandleJoinParty(component);
                    break;

                case "basic-attack":
                    await HandleTurnActionAsync(component, "basic");
                    break;

                case "first-ability":
                    await HandleTurnActionAsync(component, "ability1");
                    break;

                case "second-ability":
                    await HandleTurnActionAsync(component, "ability2");
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
                    await HandleTargetSelectAsync(args);
                    break;

                default:
                    break;
            }
        }

        private async Task HandleTargetSelectAsync(SocketMessageComponent args)
        {
            var playerTurn = gameControllerService.ReceiveRequest(args.User.Id);

            if (playerTurn == null)
            {
                await args.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {
                string selectedTarget = args.Data.Values.First().Split("&")[0];
                string selectedAttack = args.Data.Values.First().Split("&")[1];

                gameControllerService.SetPlayerTarget(playerTurn, selectedTarget);

                await gameControllerService.HandleActionAsync(args, selectedAttack);
            }
        }

        private async Task HandleJoinParty(SocketMessageComponent args)
        {
            await args.DeferAsync();

            if (!gameControllerService.IsLobbyOpen)
            {
                await args.FollowupAsync("There is no lobby open!", ephemeral: true);
            }
            else if (!gameControllerService.CheckIfPlayerExists(args.User.Id))
            {
                
                await args.FollowupAsync("You don't have a character created! /roll to create a character.", ephemeral: true);
            }
            else if (gameControllerService.Members.ContainsKey(args.User.Id))
            {
                await args.FollowupAsync("You are already in the party!", ephemeral: true);
            }
            else
            {
                gameControllerService.JoinLobby(args.User.Id, args.User.Username);
                await args.FollowupAsync("You joined", ephemeral: true);
            }
        }

        private async Task HandleTurnActionAsync(SocketMessageComponent component, string attack)
        {
            var playerTurn = gameControllerService.ReceiveRequest(component.User.Id);

            if (playerTurn == null)
            {
                await component.RespondAsync("It is not your turn!", ephemeral: true);
            }
            else
            {
                var targetList = new SelectMenuBuilder()
                    .WithPlaceholder("Select Target")
                    .WithCustomId("target-select")
                    .WithMinValues(0)
                    .WithMaxValues(1);

                var dmgType = attack switch
                {
                    "ability1" => playerTurn.FirstAbility.DamageType,
                    "ability2" => playerTurn.SecondAbility.DamageType,
                    _ => DamageType.Normal,
                };

                if (dmgType == DamageType.Heal)
                {
                    foreach (var ally in gameControllerService.GetAllies())
                    {
                        targetList.AddOption(ally, ally + "&" + attack);
                    }
                }
                else
                {
                    foreach (var enemy in gameControllerService.GetEnemies())
                    {
                        targetList.AddOption(enemy, enemy + "&" + attack);
                    }
                }

                var builder = new ComponentBuilder()
                    .WithSelectMenu(targetList);

                var playerStatus = $"[{component.User.Mention}]: Health - {playerTurn.HitPoints}/{playerTurn.MaxHitPoints}\n";

                await component.UpdateAsync(x =>
                {
                    x.Content = playerStatus;
                    x.Components = builder.Build();
                });
            }
        }

        

        
    }
}
