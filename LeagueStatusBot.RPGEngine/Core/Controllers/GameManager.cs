using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Controllers
{
    public class GameManager
    {
        public Encounter? CurrentEncounter { get; set; }
        public List<string> EventHistory { get; set; } = new List<string>(); // This can be more detailed with a custom class
        public bool IsGameStarted() => CurrentEncounter == null ? false : true;

        public event EventHandler GameStarted;
        public event EventHandler GameEnded;

        public event EventHandler<string> GameEvent;
        public event EventHandler<string> GameDeath;
        public event EventHandler<string> TurnStartPlayer;
        public event EventHandler<string> TurnStartEnemy;

        public void StartGame()
        {
            this.SpawnEncounter();
            // Initialization logic
            GameStarted?.Invoke(this, EventArgs.Empty);
        }

        public void EndGame()
        {
            // Cleanup and end logic
            GameEnded?.Invoke(this, EventArgs.Empty);
        }

        public void SpawnEncounter()
        {
            CurrentEncounter = new Encounter
            {
                EncounterParty = GenerateMonsters(),
                PlayerParty = GeneratePlayers() // This can be changed to select an active party
            };

            CurrentEncounter.EncounterEnded += OnEncounterEnded;
            CurrentEncounter.PlayerTurnStarted += OnPlayerTurnStarted;
            CurrentEncounter.TurnStarted += OnTurnStarted;
            CurrentEncounter.PartyDeath += OnPartyMemberDeath;
            CurrentEncounter.PartyAction += OnPartyAction;

            Console.WriteLine("Spawned");

            CurrentEncounter.StartEncounter();

            
        }

        public void JoinParty(ulong discordId, string name)
        {
            Player player = new Player(name, discordId);

            CurrentEncounter?.PlayerParty.AddPartyMember(player);
        }

        public void LeaveParty(Player player)
        {
        }

        private Party GenerateMonsters()
        {
            // Logic to generate monsters for an encounter
            var party = new Party();
            party.AddPartyMember(new Enemy("Goblin"));
            party.AddPartyMember(new Enemy("Warlord"));

            return party;
        }

        private Party GeneratePlayers()
        {
            // Logic to generate monsters for an encounter
            var party = new Party();
            party.AddPartyMember(new Player("James The Slayer", 331308445166731266));
            party.AddPartyMember(new Player("Chris The Bard", 331308445166731266));
            return party;
        }

        private void OnEncounterEnded(object sender, EventArgs e)
        {
            EventHistory.Add(CurrentEncounter.VictoryResult);
            CurrentEncounter = null;
            GameEnded?.Invoke(sender, e);
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



    }
}
