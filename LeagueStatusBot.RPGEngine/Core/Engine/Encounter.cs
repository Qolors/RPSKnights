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

        public event EventHandler PlayerActionChosen;
        private TaskCompletionSource tcsPlayerAction;

        public event EventHandler EncounterStarted;
        public event EventHandler EncounterEnded;

        public event EventHandler<Being> TurnStarted;
        public event EventHandler TurnEnded;

        public event EventHandler RoundEnded;
        public event EventHandler RoundStarted;

        public event EventHandler<string> PartyAction;
        public event EventHandler<string> PartyDeath;
        public event EventHandler<string> PartyMemberEffect;
        public event EventHandler<string> PartyMemberEffectRemoval;

        public async Task StartEncounterAsync()
        {
            IsEncounterActive = true;

            PlayerActionChosen += OnPlayerActionReceived;

            PlayerParty.PartyEvent += OnPartyAction;
            PlayerParty.PartyMemberDeath += OnPartyMemberDeath;
            PlayerParty.PartyMemberEffectApplied += OnPartyMemberEffectApplied;
            PlayerParty.PartyMemberEffectRemoved += OnPartyMemberEffectRemoved;

            EncounterParty.PartyEvent += OnPartyAction;
            EncounterParty.PartyMemberDeath += OnPartyMemberDeath;
            EncounterParty.PartyMemberEffectApplied += OnPartyMemberEffectApplied;
            EncounterParty.PartyMemberEffectRemoved += OnPartyMemberEffectRemoved;

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

            _ = LoopLogic();

            _ = TakeTurn();
        }

        private async Task LoopLogic()
        {
            while (IsEncounterActive)
            {
                // Check if any of the parties are defeated.
                if (!PlayerParty.IsAlive || !EncounterParty.IsAlive)
                {
                    Console.WriteLine("Party Died - Ending Game");
                    RoundEnded?.Invoke(this, EventArgs.Empty);
                    EndEncounter();
                    return;
                }
                await Task.Delay(1000);
            }
        }

        private async Task ProcessTurnAsync()
        {
            await Task.Delay(1000);
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
            await OnTurnEnded(this, EventArgs.Empty); // Directly call the method here
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

        public void OnPartyMemberEffectApplied(object? sender, string e)
        {
            PartyMemberEffect?.Invoke(sender, e);
        }

        public void OnPartyMemberEffectRemoved(object? sender, string e)
        {
            PartyMemberEffectRemoval?.Invoke(sender, e);
        }

        private async Task TakeTurn()
        {
            CurrentTurn = TurnQueue.Dequeue();

            TurnStarted?.Invoke(this, CurrentTurn);

            if (CurrentTurn.IsHuman)
            {
                await ProcessHumanTurn();
            }
            else
            {
                await ProcessNonHumanTurn();
            }
        }

        public async Task OnTurnEnded(object? sender, EventArgs e)
        {
            if (TurnQueue.Count == 0)
            {
                RoundEnded?.Invoke(this, EventArgs.Empty);
                
                SetTurnOrder();

                foreach (var being in TurnQueue)
                {
                    being.ProcessEndOfRound();
                }
            }

            CurrentTurn = TurnQueue.Dequeue();

            TurnStarted?.Invoke(this, CurrentTurn);

            if (CurrentTurn.IsHuman)
            {
                await ProcessHumanTurn();
            }
            else
            {
                await ProcessNonHumanTurn();
            }
        }

        private async Task ProcessHumanTurn()
        {
            tcsPlayerAction = new TaskCompletionSource();

            var timeout = Task.Delay(TimeSpan.FromSeconds(30));
            var completedTask = await Task.WhenAny(tcsPlayerAction.Task, timeout);

            if (completedTask == timeout)
            {
                TurnEnded?.Invoke(this, EventArgs.Empty);
                await OnTurnEnded(this, EventArgs.Empty); // Directly call the method here
            }
            else if (completedTask == tcsPlayerAction.Task)
            {
                TurnEnded?.Invoke(this, EventArgs.Empty);
                await OnTurnEnded(this, EventArgs.Empty); // Directly call the method here
            }
        }

        public void OnPlayerActionReceived(object sender, EventArgs e)
        {
            tcsPlayerAction?.SetResult();
        }

        private async Task ProcessNonHumanTurn()
        {
            await ProcessTurnAsync();
            await OnTurnEnded(this, EventArgs.Empty);
        }

        public void RaisePlayerActionChosen(EventArgs e)
        {
            PlayerActionChosen?.Invoke(this, e);
        }

        public void EndEncounter()
        {
            PlayerActionChosen -= OnPlayerActionReceived;
            PlayerParty.PartyEvent -= OnPartyAction;
            PlayerParty.PartyMemberDeath -= OnPartyMemberDeath;
            PlayerParty.PartyMemberEffectApplied -= OnPartyMemberEffectApplied;
            PlayerParty.PartyMemberEffectRemoved -= OnPartyMemberEffectRemoved;

            EncounterParty.PartyEvent -= OnPartyAction;
            EncounterParty.PartyMemberDeath -= OnPartyMemberDeath;
            EncounterParty.PartyMemberEffectApplied -= OnPartyMemberEffectApplied;
            EncounterParty.PartyMemberEffectRemoved -= OnPartyMemberEffectRemoved;

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
