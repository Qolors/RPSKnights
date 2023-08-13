using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Core.Engine;
using System.Linq;
using LeagueStatusBot.RPGEngine.Core.Events;
using System.Collections.Immutable;

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

        public event EventHandler<PlayerTurnRequest> TurnStarted;
        public event EventHandler<List<string>> TurnEnded;

        public event EventHandler<List<string>> RoundEnded;
        public event EventHandler RoundStarted;
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

            await Task.Delay(20000);

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

            CurrentEncounter.TurnStarted += OnTurnStarted;
            CurrentEncounter.TurnEnded += OnTurnEnded;

            CurrentEncounter.PartyDeath += OnPartyMemberDeath;
            CurrentEncounter.PartyAction += OnPartyAction;

            CurrentEncounter.RoundEnded += OnRoundEnded;
            CurrentEncounter.RoundStarted += OnRoundStarted;

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

        private void OnTurnStarted(object sender, Being e)
        {
            TurnStarted?.Invoke(sender, this.BuildTurnRequest(e));
        }

        private void OnTurnEnded(object sender, EventArgs e)
        {
            TurnEnded?.Invoke(sender, EventHistory);
        }

        private void OnPartyMemberDeath(object sender, string e)
        {
            //EventHistory.Add(e);
            GameDeath?.Invoke(sender, e);
        }

        private void OnPartyAction(object sender, string e)
        {
            //EventHistory.Add(e);
            GameEvent?.Invoke(sender, e);
        }

        private void OnRoundEnded(object sender, EventArgs e)
        {
            EventHistory.Clear();

            EventHistory.AddRange(CurrentEncounter.PlayerParty.Members.Select(m => m.Action));
            EventHistory.AddRange(CurrentEncounter.EncounterParty.Members.Select(m => m.Action));

            RoundEnded?.Invoke(sender, EventHistory);

            EventHistory.Clear();
        }

        private void OnRoundStarted(object sender, EventArgs e)
        {
            RoundStarted?.Invoke(sender, e);
        }

        public PlayerTurnRequest? CheckIfActivePlayer(ulong id)
        {
            if (CurrentEncounter?.CurrentTurn.DiscordId != id) return null;

            Being? targetPlayer = CurrentEncounter?
                .PlayerParty?.Members
                .FirstOrDefault(player => player.DiscordId == id);

            if (targetPlayer == null)
            {
                return null;
            }

            return new PlayerTurnRequest
            {
                Attack = "Attack",
                Defend = "Defend",
                UserId = targetPlayer.DiscordId,
                MaxHealth = targetPlayer.MaxHitPoints,
                Health = targetPlayer.HitPoints,
                Name = targetPlayer.Name,
            };
        }

        public PlayerTurnRequest BuildTurnRequest(Being targetPlayer)
        {
            return new PlayerTurnRequest
            {
                Attack = "Attack",
                Defend = "Defend",
                UserId = targetPlayer.DiscordId,
                MaxHealth = targetPlayer.MaxHitPoints,
                Health = targetPlayer.HitPoints,
                Name = targetPlayer.Name,
            };
        }

        public List<string> GetEnemyPartyNames()
        {
            return CurrentEncounter?.EncounterParty?.Members.Select(m => m.Name).ToList() ?? new List<string>();
        }

        public void SetPlayerTarget(ulong id, string name)
        {
            Being? player = CurrentEncounter?.PlayerParty?.Members
                .FirstOrDefault(player => player.DiscordId == id);

            player.Action = name;

        }

        public void UpdatePlayerStats(List<LiveGameData> liveGameData)
        {
            if (!CurrentEncounter.IsEncounterActive || liveGameData == null) return;

            Console.WriteLine(liveGameData);

            CurrentEncounter.UpdatePlayerStats(liveGameData);
        }

    }
}
