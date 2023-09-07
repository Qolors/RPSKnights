using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using LeagueStatusBot.Helpers;
using Discord.Rest;
using LeagueStatusBot.Services;
using System.IO;

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private GameManager gameManager;
        private readonly InteractiveService interactiveService;
        public RPGModule(GameManager gameManager, InteractiveService interactiveService)
        {
            this.interactiveService = interactiveService;
            this.gameManager = gameManager;
        }

        [SlashCommand("challenge", "Challenge another player to a duel")]
        public async Task GenerateGif(SocketUser user)
        {
            await DeferAsync();
            
            if (!gameManager.StartGame(Context.User.Id, user.Id))
            {
                await FollowupAsync("I don't currently support multiple game instances yet :( - please wait for current match to finish", ephemeral: true);
                return;
            }

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            var acceptDenyButtons = ButtonFactory.CreateAcceptDenyButtonOptions();
            var pageMessage = MessageFactory.CreateChallengeMessage(Context.User.GlobalName, Context.User.GetAvatarUrl(), user.Mention);
            var cancelMessage = MessageFactory.CreateChallengeNeglectedMessage(user.GlobalName);
            var buttonSelection = ButtonFactory.CreateChallengeButtons(acceptDenyButtons, pageMessage, cancelMessage, user);

            InteractiveMessageResult<ButtonOption<string>> result = null;
            IUserMessage message = null;

            result = message is null
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2), cancellationToken: cts.Token)
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2), cancellationToken: cts.Token);

            if (!result.IsSuccess)
            {
                gameManager.EndGame();
                return;
            }

            message = result.Message;

            if (result.Value!.Option == "Accept")
            {
                var initialUser = Context.User;

                var embeds = MessageFactory.BuildIntroMessage(initialUser.Mention, user.Mention, initialUser.GetAvatarUrl(size: 1024), user.GetAvatarUrl(size: 1024));

                await message.DeleteAsync();

                Task.Run(() => SendBattleRequest(Context, user));

                await FollowupAsync(embeds: embeds);
            }
            else
            {
                await message.DeleteAsync();
                await FollowupAsync($"{user.Mention} declined the challenge.. Yikes.");
            }
        }

        private async Task SendBattleRequest(SocketInteractionContext context, SocketUser otherUser)
        {
            Console.WriteLine("SendBattleRequest started"); // Debugging line
            IUserMessage message = null;
            InteractiveMessageResult<ButtonOption<string>> result = null;
            string status = null;
            while (true)
            {
                List<string> player1Choices = new();
                List<string> player2Choices = new();
                
                RestUserMessage attachmentMessage = null;
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

                await Task.Delay(1000);

                Console.WriteLine("Initializing Attachment Message"); // Debugging line
                attachmentMessage = await MessageFactory.InitializeAttachmentMessage(context, attachmentMessage, new FileAttachment("initial.gif") , new FileAttachment("initial.gif"));
                Console.WriteLine("Attachment Message Initialized"); // Debugging line

                var options = ButtonFactory.CreateButtonOptions();
                var optionsDisplayOnly = ButtonFactory.CreateDisplayOnlyButtonOptions();
                var pageBuilder = MessageFactory.CreatePageBuilder(context, player1Choices, otherUser, player2Choices, gameManager.GetCurrentHitPoints(), status);
                var buttonSelection = ButtonFactory.CreateButtonSelection(options, pageBuilder, otherUser, context);

                Console.WriteLine("Processing Player Choices"); // Debugging line
                await ProcessPlayerChoices(context, otherUser, player1Choices, player2Choices, buttonSelection, message, cts);
                Console.WriteLine("Player Choices Processed"); // Debugging line
                buttonSelection = ButtonFactory.CreateButtonSelection(optionsDisplayOnly, pageBuilder, otherUser, context);
                Console.WriteLine("Button Factory Processed");

                Console.WriteLine("Processing Turn"); // Debugging line
                if (gameManager.ProcessTurn(player1Choices, player2Choices))
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    await Task.Delay(3000);
                }
                else
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    Console.WriteLine("Breaking from loop"); // Debugging line
                    break;
                }
            }

            await context.Channel.SendMessageAsync("Haha wow nice man u did the win");
            gameManager.EndGame();
            Console.WriteLine("SendBattleRequest ended"); // Debugging line
        }        

        

        private async Task ProcessPlayerChoices(SocketInteractionContext context, SocketUser otherUser, List<string> player1Choices, List<string> player2Choices, ButtonSelection<string> buttonSelection, IUserMessage message, CancellationTokenSource cts)
        {
            Console.WriteLine($"context: {context == null}");
            Console.WriteLine($"otherUser: {otherUser == null}");
            Console.WriteLine($"player1Choices: {player1Choices == null}");
            Console.WriteLine($"player2Choices: {player2Choices == null}");
            Console.WriteLine($"buttonSelection: {buttonSelection == null}");
            Console.WriteLine($"message: {message == null}");
            Console.WriteLine($"cts: {cts == null}");

            Console.WriteLine($"context: {context}");
            Console.WriteLine($"otherUser: {otherUser}");
            Console.WriteLine($"player1Choices: {string.Join(", ", player1Choices)}");
            Console.WriteLine($"player2Choices: {string.Join(", ", player2Choices)}");
            Console.WriteLine($"buttonSelection: {buttonSelection}");
            Console.WriteLine($"message: {message}");
            Console.WriteLine($"cts: {cts}");
            
            
            while (player1Choices.Count < 3 || player2Choices.Count < 3)
            {
                var result = message is null
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2), cancellationToken: cts.Token)
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2), cancellationToken: cts.Token);

                // Check if result or message is null
                if (result == null)
                {
                    Console.WriteLine("Result is null");
                    return;
                }

                message = result.Message;

                Console.WriteLine(message);

                if (message == null)
                {
                    Console.WriteLine("Message is null");
                    return;
                }

                if (!result.IsSuccess)
                {
                    gameManager.EndGame();
                    return;
                }

                Console.WriteLine(1);

                UpdatePlayerChoices(context, otherUser, player1Choices, player2Choices, result);

                // Check if any of the player choices are null
                if (player1Choices == null || player2Choices == null)
                {
                    Console.WriteLine("Player choices are null");
                    return;
                }

                var updatedBuilder = MessageFactory.CreatePageBuilder(context, player1Choices, otherUser, player2Choices, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner);

                // Check if updatedBuilder is null
                if (updatedBuilder == null)
                {
                    Console.WriteLine("UpdatedBuilder is null");
                    return;
                }

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, otherUser, context);

                // Check if buttonSelection is null
                if (buttonSelection == null)
                {
                    Console.WriteLine("ButtonSelection is null");
                    return;
                }
            }
        }

        private void UpdatePlayerChoices(SocketInteractionContext context, SocketUser otherUser, List<string> player1Choices, List<string> player2Choices, InteractiveMessageResult<ButtonOption<string>> result)
        {
            if (context.User.Id == result.User.Id && player1Choices.Count < 3)
            {
                player1Choices.Add(result.Value.Option);
            }
            else if (otherUser.Id == result.User.Id && player2Choices.Count < 3)
            {
                player2Choices.Add(result.Value.Option);
            }
        }

        
    }
}
