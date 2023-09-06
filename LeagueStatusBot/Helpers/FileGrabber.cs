using Discord;

namespace LeagueStatusBot.Helpers;

public static class FileGrabber
{
    private static string updatedBattleFile = "initial.gif";
    public static FileAttachment GetStartingFile => new("initial.gif");
    public static FileAttachment GetBattleFile => new(updatedBattleFile);
    public static void SetBattleFile(string updatedFile) => updatedBattleFile = updatedFile;

}