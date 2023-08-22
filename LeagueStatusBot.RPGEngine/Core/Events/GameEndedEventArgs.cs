using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Core.Events
{
    public class GameEndedEventArgs : EventArgs
    {
        public List<Being> PlayerParty { get; set; }
        public bool IsVictory { get; set; }
        public string Announcement => IsVictory ? "Victory!" : "Defeat!";

        public GameEndedEventArgs(List<Being> playerParty, bool isVictory)
        {
            PlayerParty = playerParty;
            IsVictory = isVictory;
        }
    }
}
