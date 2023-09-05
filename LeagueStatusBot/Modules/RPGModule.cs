using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Fergun.Interactive;
using Fergun.Interactive.Selection;
using LeagueStatusBot.RPGEngine.Core.Controllers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using LeagueStatusBot.Helpers;
using Discord.Rest;

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

            if (!gameManager.StartGame(Context.User.Id, user.Id)) return;

            using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));

            InteractiveMessageResult<ButtonOption<string>> result = null;
            IUserMessage message = null;

            var acceptDeny = new ButtonOption<string>[]
            {
                new("Accept", ButtonStyle.Primary),
                new("Deny", ButtonStyle.Secondary)
            };

            var pageBuilder = new PageBuilder()
                .WithTitle($"{user.Mention} has been challenged to duel")
                .WithThumbnailUrl(Context.User.GetAvatarUrl())
                .WithDescription("What is your response?");

            var buttonSelection = new ButtonSelectionBuilder<string>()
                .WithOptions(acceptDeny)
                .WithStringConverter(x => x.Option)
                .WithSelectionPage(pageBuilder)
                .AddUser(user)
                .Build();

            result = message is null
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2), cancellationToken: cts.Token)
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2), cancellationToken: cts.Token);

            message = result.Message;

            if (result.Value!.Option == "Accept")
            {
                var embed = new EmbedBuilder()
                .WithTitle("Challenge Accepted!")
                .WithDescription($"**{Context.User.Mention}** VS **{user.Mention}**")
                .WithUrl("https://google.com")
                .WithImageUrl(Context.User.GetAvatarUrl(format: ImageFormat.Png, size: 1024));

                var embed2 = new EmbedBuilder()
                .WithUrl("https://google.com")
                .WithImageUrl(user.GetAvatarUrl(format: ImageFormat.Png, size: 1024));

                await message.DeleteAsync();

                Task.Run(() => SendBattleRequest(Context, user));

                await FollowupAsync(embeds: new Embed[] { embed.Build(), embed2.Build() });
            }
            else
            {
                await message.DeleteAsync();
                await FollowupAsync($"{user.Mention} declined the challenge.");
            }
        }

        private async Task SendBattleRequest(SocketInteractionContext context, SocketUser otherUser)
        {
            List<string> player1Choices = new();
            List<string> player2Choices = new();
            IUserMessage message = null;
            RestUserMessage attachmentMessage = null;

            await Task.Delay(5000);

            attachmentMessage = await context.Channel.SendFileAsync(GetStartingFile());

            var options = new ButtonOption<string>[]
            {
                new("Attack", ButtonStyle.Primary),
                new("Defend", ButtonStyle.Secondary),
                new("Ability", ButtonStyle.Success)
            };

            var pageBuilder = new PageBuilder()
                .WithTitle("Turn Phase")
                .WithDescription($"**{context.User.GlobalName}**: {player1Choices.Count}/3 Turns Made\n**{otherUser.GlobalName}**: {player2Choices.Count}/3 Turns Made");

            var buttonSelection = new ButtonSelectionBuilder<string>()
                .WithOptions(options)
                .WithStringConverter(x => x.Option)
                .WithSelectionPage(pageBuilder)
                .AddUser(otherUser)
                .AddUser(context.User)
                .Build();

            while (player1Choices.Count <= 3 && player2Choices.Count <= 3)
            {
                var result = message is null 
                ? await interactiveService.SendSelectionAsync(buttonSelection, Context.Channel, TimeSpan.FromMinutes(2))
                : await interactiveService.SendSelectionAsync(buttonSelection, message, TimeSpan.FromMinutes(2));

                message = result.Message;

                if (context.User.Id == result.User.Id && player1Choices.Count < 3)
                {
                    player1Choices.Add(result.Value.Option);
                }
                else if (otherUser.Id == result.User.Id && player2Choices.Count < 3)
                {
                    player2Choices.Add(result.Value.Option);
                }

                pageBuilder.WithDescription($"{context.User.GlobalName}: {player1Choices.Count}/3\n{otherUser.GlobalName}: {player2Choices.Count}/3");

                buttonSelection = new ButtonSelectionBuilder<string>()
                    .WithOptions(options)
                    .WithStringConverter(x => x.Option)
                    .WithSelectionPage(pageBuilder)
                    .AddUser(otherUser)
                    .AddUser(context.User)
                    .Build();
            }
        }

        private static FileAttachment GetStartingFile()
        {
            return new FileAttachment("initial.gif");
        }
    }
}
