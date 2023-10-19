using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord.Rest;
using Fergun.Interactive;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using LeagueStatusBot.Helpers;
using LeagueStatusBot.Services;
using LeagueStatusBot.Factories;
using LeagueStatusBot.RPGEngine.Data.Repository;
using LeagueStatusBot.RPGEngine.Core.Controllers;

/// <summary>
/// The RPGModule class is responsible for handling RPG related commands and interactions.
/// </summary>

namespace LeagueStatusBot.Modules
{
    public class RPGModule : InteractionModuleBase<SocketInteractionContext>
    {
        private GameFactory gameFactory;
        private readonly InteractiveService interactiveService;
        private PlayerRepository playerRepository;
        private Dictionary<ulong, GameManager> activeGames;
        public RPGModule(GameFactory gameFactory, InteractiveService interactiveService, PlayerRepository playerRepository)
        {
            this.interactiveService = interactiveService;
            this.playerRepository = playerRepository;
            this.gameFactory = gameFactory;
            activeGames = new();
        }

        [SlashCommand("leaderboard", "Get this Discord Server's Top Ranked Players")]
        public async Task GetPlayerRanks()
        {
            await DeferAsync();

            var splashScreen = Context.Guild.DiscoverySplashUrl;
            var guildName = Context.Guild.Name;

            var playerList = await playerRepository.GetLeaderboard(Context.Guild.Id);

            var embeds = MessageFactory.BuildLeaderboard(playerList, splashScreen, guildName);

            await FollowupAsync(embeds: embeds);
        }

        [SlashCommand("challenge", "Challenge another player to a duel")]
        public async Task GenerateGif(SocketUser user)
        {
            await DeferAsync();

            ulong gameKey = Context.User.Id;

            if (activeGames.ContainsKey(gameKey))
            {
                await FollowupAsync("You already have an active game!", ephemeral: true);
                return;
            }

            if (Context.User.Id == user.Id)
            {
                await FollowupAsync("You cannot verse yourself!", ephemeral: true);
                return;
            }

            var gameManager = gameFactory.Create(gameKey);

            gameManager.OnGameEnded += (sender, args) => activeGames.Remove(Context.User.Id);
            activeGames[gameKey] = gameManager;
            
            string gifAnimation = await gameManager.StartGame(Context.User.Id, user.Id, Context.User.GlobalName, user.GlobalName, Context.User.GetAvatarUrl(), user.GetAvatarUrl());

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

                Task.Run(() => SendBattleRequest(Context, user, gameManager, gifAnimation));

                await FollowupAsync(embeds: embeds);
            }
            else
            {
                await message.DeleteAsync();
                await FollowupAsync($"{user.Mention} declined the challenge..");
            }
        }

        private async Task SendBattleRequest(SocketInteractionContext context, SocketUser otherUser, GameManager gameManager, string filePath)
        {

            RestUserMessage attachmentMessage = null;
            IUserMessage message = null;
            int round = 1;

            while (true)
            {
                List<string> player1Choices = new();
                List<string> player2Choices = new();
                
                using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

                attachmentMessage = await MessageFactory.InitializeAttachmentMessage(context, attachmentMessage, new FileAttachment(filePath) , new FileAttachment(filePath));

                var options = ButtonFactory.CreateButtonOptions();
                var optionsDisplayOnly = ButtonFactory.CreateDisplayOnlyButtonOptions();

                var pageBuilder = MessageFactory.CreatePageBuilder(context.User, Color.Blue, player1Choices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);
                var buttonSelection = ButtonFactory.CreateButtonSelection(options, pageBuilder, otherUser);

                message = await ProcessPlayerChoices(context, otherUser, player1Choices, player2Choices, buttonSelection, cts, round, gameManager, message);

                if (gameManager.Forfeit)
                    break;

                var nobuttonSelection = ButtonFactory.CreateButtonSelection(optionsDisplayOnly, pageBuilder, otherUser);

                if (await gameManager.ProcessTurn(player1Choices, player2Choices))
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    using var disabledCts = new CancellationTokenSource(TimeSpan.FromSeconds(1));
                    round++;

                    await interactiveService.SendSelectionAsync(nobuttonSelection, message, TimeSpan.FromSeconds(1), cancellationToken: disabledCts.Token);

                    await gameManager.ProcessDecisions();
                }
                else
                {
                    await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment(gameManager.MostRecentFile));
                    break;
                }
            }

            gameManager.ProcessDeathScene();
            gameManager.EndGame();

            await MessageFactory.UpdateAttachmentMessage(attachmentMessage, new FileAttachment($"{Context.User.Id}/FinalBattle.gif"));

            var optionsDisplay = ButtonFactory.CreateDisplayOnlyButtonOptions();
            

            var player1 = await context.Channel.GetUserAsync(context.User.Id);
            var player2 = await context.Channel.GetUserAsync(otherUser.Id);

            var player1Rating = await playerRepository.GetPlayerElo(context.Guild.Id, player1.Id);
            var player2Rating = await playerRepository.GetPlayerElo(context.Guild.Id, player2.Id);

            double playerARating = player1Rating?? 1200;
            double playerBRating = player2Rating?? 1200;

            double playerABefore = playerARating;
            double playerBBefore = playerBRating;

            int winner = gameManager.Player1Won ? 1 : 0;

            EloRating.UpdateRatings(ref playerARating, ref playerBRating, winner);

            var endPage = MessageFactory.CreateEndGameMessage(gameManager.FinalWinnerName, gameManager.Forfeit, (int)playerABefore, (int)playerARating, (int)playerBBefore, (int)playerBRating, player1.Username, player2.Username);
            var finalSelection = ButtonFactory.CreateButtonSelection(optionsDisplay, endPage, otherUser);

            await interactiveService.SendSelectionAsync(finalSelection, message, TimeSpan.FromSeconds(6));

            await playerRepository.UpdateOrAddPlayer(context.Guild.Id, player1.Id, player1.Username, (int)playerARating, gameManager.Player1Won);
            await playerRepository.UpdateOrAddPlayer(context.Guild.Id, player2.Id, player2.Username, (int)playerBRating, !gameManager.Player1Won);

            gameManager.Dispose();
        }
        private async Task<IUserMessage> ProcessPlayerChoices(SocketInteractionContext context, 
            SocketUser otherUser, 
            List<string> player1Choices, 
            List<string> player2Choices, 
            ButtonSelection<string> buttonSelection, 
            CancellationTokenSource cts, 
            int round, 
            GameManager gameManager,
            IUserMessage message)
        {
            message = await ProcessChoicesForPlayer(context, otherUser, player1Choices, buttonSelection, cts, round, gameManager, message, context.User, gameManager.GetPlayer1Energy);

            if (gameManager.Forfeit)
                return message;

            message = await ProcessChoicesForPlayer(context, otherUser, player2Choices, buttonSelection, cts, round, gameManager, message, otherUser, gameManager.GetPlayer2Energy);
            return message;
        }

        private async Task<IUserMessage> ProcessChoicesForPlayer(SocketInteractionContext context, 
            SocketUser otherUser, 
            List<string> playerChoices, 
            ButtonSelection<string> buttonSelection, 
            CancellationTokenSource cts, 
            int round, 
            GameManager gameManager,
            IUserMessage message,
            SocketUser player,
            int energyAvailable)
        {

            Color color = context.User.Id == player.Id ? Color.Blue : Color.Red;
            
            var updatedButtons = ButtonFactory.CreateButtonOptions(energyAvailable);

            while (playerChoices.Count < 1)
            {
                var updatedBuilder = MessageFactory.CreatePageBuilder(player, color, playerChoices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.GetPlayer1Energy, gameManager.GetPlayer2Energy, gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(updatedButtons, updatedBuilder, player);

                var result = await SendSelectionAsync(buttonSelection, message, cts);

                if (result.IsCanceled)
                {
                    gameManager.PlayerForfeit(player.Id);
                    message = result.Message;
                    Console.WriteLine("Forfeited Match due to inactivity");
                    break;
                }

                message = result.Message;

                UpdatePlayerChoices(player, playerChoices, result);

                updatedBuilder = MessageFactory.CreatePageBuilder(player, color, playerChoices, otherUser, gameManager.GetCurrentHitPoints(), gameManager.CurrentWinner, round);

                buttonSelection = ButtonFactory.CreateButtonSelection(buttonSelection.Options.ToArray(), updatedBuilder, player);

            }

            return message;
        }

        private async Task<InteractiveMessageResult<ButtonOption<string>>> SendSelectionAsync(ButtonSelection<string> buttonSelection, IUserMessage message, CancellationTokenSource cts)
        {
            return message is null
                    ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(1), cancellationToken: cts.Token)
                    : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(1), cancellationToken: cts.Token);
        }

        private void UpdatePlayerChoices(SocketUser player, List<string> playerChoices, InteractiveMessageResult<ButtonOption<string>> result)
        {
            if (player.Id == result.User.Id && playerChoices.Count < 1)
            {
                playerChoices.Add(result.Value.Option);
            }
        }

        
    }
}
