namespace LeagueStatusBot.RPGEngine.Messaging;

public record TurnMessage
{
    public int Player1Health;
    public int Player2Health;
    public string FileName;

    public TurnMessage(int p1, int p2, string file)
    {
        Player1Health = p1;
        Player2Health = p2;
        FileName = file;
    }
}