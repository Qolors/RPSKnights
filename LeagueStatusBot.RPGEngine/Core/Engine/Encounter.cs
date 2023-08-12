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

        public event EventHandler<Being> TurnStarted;
        public event EventHandler TurnEnded;

        public event EventHandler RoundEnded;
        public event EventHandler RoundStarted;

        public event EventHandler<string> PartyAction;
        public event EventHandler<string> PartyDeath;

        public async Task StartEncounterAsync()
        {
            IsEncounterActive = true;

            PlayerParty.PartyEvent += OnPartyAction;
            PlayerParty.PartyMemberDeath += OnPartyMemberDeath;

            EncounterParty.PartyEvent += OnPartyAction;
            EncounterParty.PartyMemberDeath += OnPartyMemberDeath;

            EncounterStarted?.Invoke(this, EventArgs.Empty);

            await ProcessRoundAsync();
        }

        private void SetTurnOrder()
        {
            //TODO --> ORDER BY SOMETHING
            TurnQueue = new Queue<Being>(
                PlayerParty.Members.Concat(EncounterParty.Members)
                .ToList()
            );
        }

        private async Task ProcessRoundAsync()
        {
            SetTurnOrder();

            while (IsEncounterActive)
            {
                if (TurnQueue.Count == 0)
                {
                    RoundEnded?.Invoke(this, EventArgs.Empty);
                    SetTurnOrder();
                    await Task.Delay(5000);
                }

                CurrentTurn = TurnQueue.Dequeue();

                if (CurrentTurn.IsHuman)
                {
                    await StartTurnTimerAsync();
                }
                else
                {
                    await StartTurnTimerAsyncMon();
                }

                await ProcessTurnAsync();

                // Check if any of the parties are defeated.
                if (!PlayerParty.IsAlive || !EncounterParty.IsAlive)
                {
                    RoundEnded?.Invoke(this, EventArgs.Empty);
                    await Task.Delay(5000);
                    EndEncounter();
                    return;
                }
            }
        }

        private async Task StartTurnTimerAsync()
        {
            TurnStarted?.Invoke(this, CurrentTurn);

            await Task.Delay(12000);
        }

        private async Task StartTurnTimerAsyncMon()
        {
            await Task.Delay(3000);
        }

        private async Task ProcessTurnAsync()
        {
            if (CurrentTurn.IsAlive)
            {
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

            TurnEnded?.Invoke(this, EventArgs.Empty);
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
