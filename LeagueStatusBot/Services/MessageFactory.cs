using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Fergun.Interactive;

namespace LeagueStatusBot.Services;

public static class MessageFactory
{
    public async static Task UpdateAttachmentMessage(RestUserMessage attachmentMessage, FileAttachment file)
    {
        await attachmentMessage.ModifyAsync(x => x.Attachments = new FileAttachment[] { file });
    }

    public async static Task<RestUserMessage> InitializeAttachmentMessage(SocketInteractionContext context, RestUserMessage attachmentMessage, FileAttachment file, FileAttachment starterFile)
    {
        if (attachmentMessage == null)
        {
            attachmentMessage = await context.Channel.SendFileAsync(starterFile);
        }
        else
        {
            await attachmentMessage.ModifyAsync(x => x.Attachments = new FileAttachment[] { file });
        }
        return attachmentMessage;
    }

    public static PageBuilder CreatePageBuilder(SocketInteractionContext context, List<string> player1Choices, SocketUser otherUser, List<string> player2Choices, int[] hitpoints)
    {
        return new PageBuilder()
            .WithTitle("Turn Phase")
            .WithDescription($"**{context.User.GlobalName}** - [{hitpoints[0]}/3 HP]: {player1Choices.Count}/3\n**{otherUser.GlobalName}** - [{hitpoints[1]}/3 HP]: {player2Choices.Count}/3");
    }

    public static PageBuilder CreateChallengeMessage(string challenger, string avatarUrl, string mention)
    {
        return new PageBuilder()
                .WithTitle($"{challenger} challenges you to a duel")
                .WithThumbnailUrl(avatarUrl)
                .WithDescription($"*{mention}, What is your response?*");
    }

    public static PageBuilder CreateChallengeNeglectedMessage(string username)
    {
        return new PageBuilder()
                .WithDescription($"*{username}'s duel expired..*");
    }

    public static Embed[] BuildIntroMessage(string firstMention, string secondMention, string firstAvatar, string secondAvatar)
    {
        var embed = new EmbedBuilder()
                .WithTitle("A Battle Begins!")
                .WithDescription($"**{firstMention}** VS **{secondMention}**")
                .WithUrl("https://google.com")
                .WithImageUrl(firstAvatar)
                .Build();

        var embed2 = new EmbedBuilder()
        .WithUrl("https://google.com")
        .WithImageUrl(secondAvatar)
        .Build();

        return new Embed[] { embed, embed2 };
    }


}