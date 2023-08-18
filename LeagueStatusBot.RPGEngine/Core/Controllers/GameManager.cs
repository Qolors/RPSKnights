using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Core.Engine;
using System.Linq;
using LeagueStatusBot.RPGEngine.Core.Events;
using LeagueStatusBot.RPGEngine.Factories;

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
        public event EventHandler<Being> TurnStarted;
        public event EventHandler<List<string>> TurnEnded;
        public event EventHandler RoundEnded;
        public event EventHandler RoundStarted;
        private Random Random { get; set; } = new Random();

        public async Task StartGameAsync(List<Being> beings)
        {
            Party party = new Party();

            foreach(var being in beings)
            {
                party.AddPartyMember(being);
            }

            CurrentEncounter = new Encounter
            {
                PlayerParty = party,
                EncounterParty = GenerateMonsters()
            };

            GameStarted?.Invoke(this, EventArgs.Empty);

            await Task.Delay(5000);

            await SpawnEncounterAsync();
        }

        public Being AssignRandomClass()
        {
            switch (Random.Next(0, 3))
            {
                case 0:
                    return ClassFactory.CreateVagabond();
                case 1:
                    return ClassFactory.CreateApprentice();
                case 2:
                    return ClassFactory.CreateAdventurer();
                default:
                    return ClassFactory.CreateAdventurer();
            }
        }

        private async Task SpawnEncounterAsync()
        {

            CurrentEncounter.EncounterEnded += OnEncounterEnded;

            CurrentEncounter.TurnStarted += OnTurnStarted;
            CurrentEncounter.TurnEnded += OnTurnEnded;

            CurrentEncounter.PartyDeath += OnPartyMemberDeath;
            CurrentEncounter.PartyAction += OnPartyAction;

            CurrentEncounter.RoundEnded += OnRoundEnded;
            CurrentEncounter.RoundStarted += OnRoundStarted;

            CurrentEncounter.PartyMemberEffect += OnPartyEffect;
            CurrentEncounter.PartyMemberEffectRemoval += OnPartyEffectRemoval;

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

            CurrentEncounter.EncounterEnded -= OnEncounterEnded;
            CurrentEncounter.TurnStarted -= OnTurnStarted;
            CurrentEncounter.TurnEnded -= OnTurnEnded;
            CurrentEncounter.PartyDeath -= OnPartyMemberDeath;
            CurrentEncounter.PartyAction -= OnPartyAction;
            CurrentEncounter.RoundEnded -= OnRoundEnded;
            CurrentEncounter.RoundStarted -= OnRoundStarted;
            CurrentEncounter.PartyMemberEffect -= OnPartyEffect;
            CurrentEncounter.PartyMemberEffectRemoval -= OnPartyEffectRemoval;

            EventHistory.Clear();

            CurrentEncounter = null;
        }

        private void OnTurnStarted(object sender, Being e)
        {
            TurnStarted?.Invoke(sender, e);
        }

        private void OnTurnEnded(object sender, EventArgs e)
        {
            TurnEnded?.Invoke(sender, EventHistory);
        }

        private void OnPartyMemberDeath(object sender, string e)
        {
            if (EventHistory.Count >= 14)
            {
                EventHistory.RemoveAt(0);
            }
            EventHistory.Add(e);
            GameDeath?.Invoke(sender, e);
        }

        private void OnPartyAction(object sender, string e)
        {
            if (EventHistory.Count >= 14)
            {
                EventHistory.RemoveAt(0);
            }
            EventHistory.Add(e);
            GameEvent?.Invoke(sender, e);
        }

        private void OnPartyEffect(object sender, string e)
        {
            EventHistory.Add(e);
            GameEvent?.Invoke(sender, e);
        }

        private void OnPartyEffectRemoval(object sender, string e)
        {
            EventHistory.Add(e);
            GameEvent?.Invoke(sender, e);
        }

        private void OnRoundEnded(object sender, EventArgs e)
        {
            RoundEnded?.Invoke(sender, e);
        }

        private void OnRoundStarted(object sender, EventArgs e)
        {
            RoundStarted?.Invoke(sender, e);
        }

        public Being? CheckIfActivePlayer(ulong id)
        {
            if (CurrentEncounter?.CurrentTurn.DiscordId != id) return null;

            return CurrentEncounter?.CurrentTurn;
        }

        public List<string> GetEnemyPartyNames()
        {
            return CurrentEncounter?.EncounterParty?.Members.Select(m => m.Name).ToList() ?? new List<string>();
        }

        public List<string> GetPlayerPartyNames()
        {
            return CurrentEncounter?.PlayerParty?.Members.Select(m => m.Name).ToList() ?? new List<string>();
        }

        public void SetPlayerTarget(Being player, string name)
        {
            Being? enemy = CurrentEncounter?.EncounterParty?.Members
                .FirstOrDefault(e => e.Name == name);

            if (enemy == null)
            {
                Being? ally = CurrentEncounter?.PlayerParty?.Members
                    .FirstOrDefault(a => a.Name == name);

                player?.SetTarget(ally);
            }
            else
            {
                player?.SetTarget(enemy);
            }

            
        }

    }
}
