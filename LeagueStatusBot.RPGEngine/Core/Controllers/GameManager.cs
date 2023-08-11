using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Core.Events;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class GameManager
    {
        public Encounter? CurrentEncounter { get; set; }
        public List<string> EventHistory { get; set; } = new List<string>(); // This can be more detailed with a custom class
        public bool IsGameStarted() => CurrentEncounter == null ? false : true;

        public event EventHandler GameStarted;
        public event EventHandler<string?> GameEnded;
        public event EventHandler<string> GameEvent;
        public event EventHandler<string> GameDeath;
        public event EventHandler<string> TurnStartPlayer;
        public event EventHandler<string> TurnStartEnemy;
        public event EventHandler RoundEnded;
        public async Task StartGameAsync(Dictionary<ulong, string> partyMembers)
        {
            Party party = new Party();

            foreach (var member in partyMembers)
            {
                var player = new Player(member.Key, member.Value);

                party.AddPartyMember(player);
            }

            CurrentEncounter = new Encounter
            {
                PlayerParty = party,
                EncounterParty = GenerateMonsters()
            };

            GameStarted?.Invoke(this, EventArgs.Empty);

            await Task.Delay(2000);

            await SpawnEncounterAsync();
            // Initialization logic
        }

        public void EndGame()
        {
            // Cleanup and end logic
            
        }

        private async Task SpawnEncounterAsync()
        {

            CurrentEncounter.EncounterEnded += OnEncounterEnded;
            CurrentEncounter.PlayerTurnStarted += OnPlayerTurnStarted;
            CurrentEncounter.TurnStarted += OnTurnStarted;
            CurrentEncounter.PartyDeath += OnPartyMemberDeath;
            CurrentEncounter.PartyAction += OnPartyAction;
            CurrentEncounter.RoundEnded += OnRoundEnded;

            Console.WriteLine("Spawned");

            await CurrentEncounter.StartEncounterAsync();
        }

        private Party GenerateMonsters()
        {
            // Logic to generate monsters for an encounter
            var party = new Party();
            party.AddPartyMember(new Enemy("Bragore the Wretched"));
            party.AddPartyMember(new Enemy("Lord Tusker"));
            party.AddPartyMember(new Enemy("D the Lone Bumbis"));

            return party;
        }

        private void OnEncounterEnded(object sender, EventArgs e)
        {
            GameEnded?.Invoke(sender, CurrentEncounter?.VictoryResult);
            
            CurrentEncounter = null;
        }

        private void OnPlayerTurnStarted(object sender, string e)
        {
            TurnStartPlayer?.Invoke(sender, e);
        }

        private void OnTurnStarted(object sender, EventArgs e)
        {
            TurnStartEnemy?.Invoke(sender, "Enemy Move");
        }

        private void OnPartyMemberDeath(object sender, string e)
        {
            EventHistory.Add(e);
            GameDeath?.Invoke(sender, e);
        }

        private void OnPartyAction(object sender, string e)
        {
            EventHistory.Add(e);
            GameEvent?.Invoke(sender, e);
        }

        private void OnRoundEnded(object sender, EventArgs e)
        {
            RoundEnded?.Invoke(sender, e);

            EventHistory.Clear();
        }


    }
}
