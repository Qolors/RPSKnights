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
        private IUserMessage message = null;
        public RPGModule(GameManager gameManager, InteractiveService interactiveService)
        {
            this.interactiveService = interactiveService;
            this.gameManager = gameManager;
        }

        [SlashCommand("challenge", "Challenge another player to a duel")]
        public async Task GenerateGif(SocketUser user)
        {
            await DeferAsync();
            
            var gameReady = await gameManager.StartGame(Context.User.Id, user.Id, Context.User.GlobalName, user.GlobalName, Context.User.GetAvatarUrl(), user.GetAvatarUrl());

            if (!gameReady)
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
                gameManager.Dispose();
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
            Console.WriteLine("SendBattleRequest started");
            RestUserMessage attachmentMessage = null;
            int round = 1;
            while (true)
            {
                List<string> player1Choices = new();
                List<string> player2Choices = new();
                
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

                await Task.Delay(1000);

                attachmentMessage = await MessageFactory.InitializeAttachmentMessage(context, attachmentMessage, new FileAttachment("initial.gif") , new FileAttachment("initial.gif"));

                var options = ButtonFactory.CreateButtonOptions();
                var optionsDisplayOnly = ButtonFactory.CreateDisplayOnlyButtonOptions();
                var pageBuilder = MessageFactory.CreatePageBuilder(context.User, Color.Blue, player1Choices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);
                var buttonSelection = ButtonFactory.CreateButtonSelection(options, pageBuilder, otherUser);

                await ProcessPlayerChoices(context, otherUser, player1Choices, player2Choices, buttonSelection, cts, round);

                var nobuttonSelection = ButtonFactory.CreateButtonSelection(optionsDisplayOnly, pageBuilder, otherUser);

                if (gameManager.ProcessTurn(player1Choices, player2Choices))
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    using var disabledCts = new CancellationTokenSource(TimeSpan.FromSeconds(6));
                    round++;
                    await interactiveService.SendSelectionAsync(nobuttonSelection, message, TimeSpan.FromSeconds(6), cancellationToken: disabledCts.Token);
                    gameManager.ProcessDecisions();
                }
                else
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    break;
                }
            }

            gameManager.ProcessDeathScene();

            gameManager.EndGame();
            await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment("FinalBattle.gif"));
            var optionsDisplay = ButtonFactory.CreateDisplayOnlyButtonOptions();
            var endPage = MessageFactory.CreateEndGameMessage(gameManager.FinalWinnerName);
            var finalSelection = ButtonFactory.CreateButtonSelection(optionsDisplay, endPage, otherUser);
            await interactiveService.SendSelectionAsync(finalSelection, message, TimeSpan.FromSeconds(6));

            message = null;
            gameManager.Dispose();
        }        

        

        private async Task ProcessPlayerChoices(SocketInteractionContext context, SocketUser otherUser, List<string> player1Choices, List<string> player2Choices, ButtonSelection<string> buttonSelection, CancellationTokenSource cts, int round)
        {            
            while (player1Choices.Count < 3)
            {
                var updatedBuilder = MessageFactory.CreatePageBuilder(context.User, Color.Blue, player1Choices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, context.User);

                var result = message is null
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2), cancellationToken: cts.Token)
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2), cancellationToken: cts.Token);

                message = result.Message;

                if (!result.IsSuccess)
                {
                    gameManager.EndGame();
                    return;
                }

                UpdatePlayerChoices(context, otherUser, player1Choices, player2Choices, result);

                updatedBuilder = MessageFactory.CreatePageBuilder(context.User, Color.Blue, player1Choices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, context.User);

                if (buttonSelection == null)
                {
                    Console.WriteLine("ButtonSelection is null");
                    return;
                }
            }

            while (player2Choices.Count < 3)
            {
                var updatedBuilder = MessageFactory.CreatePageBuilder(otherUser, Color.Red, player2Choices, context.User, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, otherUser);

                var result = message is null
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2), cancellationToken: cts.Token)
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2), cancellationToken: cts.Token);

                message = result.Message;

                if (!result.IsSuccess)
                {
                    gameManager.EndGame();
                    return;
                }

                UpdatePlayerChoices(context, otherUser, player1Choices, player2Choices, result);

                updatedBuilder = MessageFactory.CreatePageBuilder(otherUser, Color.Red, player1Choices, context.User, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, otherUser);

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
