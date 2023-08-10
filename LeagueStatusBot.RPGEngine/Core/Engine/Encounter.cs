using LeagueStatusBot.RPGEngine.Core.Events;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Encounter
    {
        public Party? EncounterParty { get; set; }
        public Party? PlayerParty { get; set; }
        public bool IsEncounterActive { get; private set; }
        private Queue<Being> TurnQueue { get; set; }
        public Being CurrentTurn { get; private set; }

        public string VictoryResult { get; set; }

        public event EventHandler EncounterStarted;
        public event EventHandler EncounterEnded;
        public event EventHandler TurnStarted;
        public event EventHandler<string> PlayerTurnStarted;
        public event EventHandler TurnEnded;
        public event EventHandler RoundEnded;

        public event EventHandler<string> PartyAction;
        public event EventHandler<string> PartyDeath;

        public async Task StartEncounterAsync()
        {
            IsEncounterActive = true;

            SetTurnOrder();

            PlayerParty.PartyEvent += OnPartyAction;
            PlayerParty.PartyMemberDeath += OnPartyMemberDeath;

            EncounterParty.PartyEvent += OnPartyAction;
            EncounterParty.PartyMemberDeath += OnPartyMemberDeath;

            EncounterStarted?.Invoke(this, EventArgs.Empty);

            await ProcessTurnAsync();
        }

        private void SetTurnOrder()
        {
            //TODO --> ORDER BY SOMETHING
            TurnQueue = new Queue<Being>(
                PlayerParty.Members.Concat(EncounterParty.Members)
                .ToList()
            );
        }

        private async Task ProcessTurnAsync()
        {
            while (IsEncounterActive)
            {
                if (TurnQueue.Count == 0)
                {
                    RoundEnded?.Invoke(this, EventArgs.Empty);
                    SetTurnOrder();  // Reorder and reset the queue for the next round.
                    await Task.Delay(3000);
                }

                CurrentTurn = TurnQueue.Dequeue();

                // Ensure the current being has a target.
                if (CurrentTurn.IsAlive)
                {
                    // Check if the target is dead. If yes, reassign a new target.
                    if (CurrentTurn.Target == null || !CurrentTurn.Target.IsAlive)
                    {
                        AssignTargetFor(CurrentTurn);
                    }

                    // Now check if the target is valid and attack.
                    if (CurrentTurn.Target != null)
                    {
                        CurrentTurn.AttackTarget();
                    }
                }

                // Check if any of the parties are defeated.
                if (!PlayerParty.IsAlive || !EncounterParty.IsAlive)
                {
                    RoundEnded?.Invoke(this, EventArgs.Empty);
                    await Task.Delay(3000);
                    EndEncounter();
                    return;
                }
            }
        }

        private void AssignTargetFor(Being being)
        {
            // This assigns a random target from the opposite party.
            if (PlayerParty.Members.Contains(being))
            {
                being.Target = EncounterParty.Members
                    .Where(m => m.IsAlive)
                    .OrderBy(_ => Guid.NewGuid()) // Random selection
                    .FirstOrDefault();
            }
            else
            {
                being.Target = PlayerParty.Members
                    .Where(m => m.IsAlive)
                    .OrderBy(_ => Guid.NewGuid())
                    .FirstOrDefault();
            }
        }

        public void OnPartyAction(object? sender, string e)
        {
            PartyAction?.Invoke(sender, e);
        }

        public void OnPartyMemberDeath(object? sender, string e)
        {
            PartyDeath?.Invoke(sender, e);
        }

        public void EndEncounter()
        {
            IsEncounterActive = false;
            DetermineRewards();
            EncounterEnded?.Invoke(this, EventArgs.Empty);
        }

        private void DetermineRewards()
        {
            if (PlayerParty.IsAlive)
            {
                VictoryResult = "Your Party has won!";
            }
            else if (EncounterParty.IsAlive)
            {
                VictoryResult = "Your Party has been defeated..";
            }
        }
    }
}
