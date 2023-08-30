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
        public event EventHandler<GameEndedEventArgs> EncounterEnded;

        public event EventHandler<Being> TurnStarted;
        public event EventHandler TurnEnded;

        public event EventHandler RoundEnded;
        public event EventHandler RoundStarted;

        public event EventHandler<string> PartyAction;
        public event EventHandler<CharacterDeathEventArgs> PartyDeath;
        public event EventHandler<string> PartyMemberEffect;
        public event EventHandler<string> PartyMemberEffectRemoval;
        public event EventHandler<float> PartyMemberAOEDamageDone;

        public async Task StartEncounterAsync()
        {
            IsEncounterActive = true;

            PlayerActionChosen += OnPlayerActionReceived;

            PlayerParty.PartyEvent += OnPartyAction;
            PlayerParty.PartyMemberAOEDamageDone += OnPartyMemberAOEDamageDone;
            PlayerParty.PartyMemberDeath += OnPartyMemberDeath;
            PlayerParty.PartyMemberEffectApplied += OnPartyMemberEffectApplied;
            PlayerParty.PartyMemberEffectRemoved += OnPartyMemberEffectRemoved;
            PlayerParty.PartyMemberAOEHealDone += OnPartyMemberAOEHealDone;

            EncounterParty.PartyEvent += OnPartyAction;
            EncounterParty.PartyMemberAOEDamageDone += OnPartyMemberAOEDamageDone;
            EncounterParty.PartyMemberDeath += OnPartyMemberDeath;
            EncounterParty.PartyMemberEffectApplied += OnPartyMemberEffectApplied;
            EncounterParty.PartyMemberEffectRemoved += OnPartyMemberEffectRemoved;
            EncounterParty.PartyMemberAOEHealDone += OnPartyMemberAOEHealDone;

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
                    if (!CurrentTurn.ActiveEffects.Any(a => a.Type == EffectType.Stun))
                    {
                        if (CurrentTurn.Target.IsHuman && CurrentTurn.Target.AnyArmorUnused())
                        {
                            TurnStarted?.Invoke(this, CurrentTurn.Target);
                            await ProcessDefenseTurn();
                            CurrentTurn.AttackTarget();
                        }
                        else
                        {
                            CurrentTurn.AttackTarget();
                        }
                        
                    }
                }
            }

            await Task.Delay(2000);

            TurnEnded?.Invoke(this, EventArgs.Empty);
            await Task.Delay(7000);
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

        public void OnPartyMemberDeath(object? sender, CharacterDeathEventArgs e)
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

        public void OnPartyMemberAOEDamageDone(object sender, float e)
        {
            Being being = (Being)sender;

            if (PlayerParty.Members.Contains(being))
            {
                for (int i = 0; i < EncounterParty.Members.Count; i++)
                {
                    var member = EncounterParty.Members[i];
                    if (member != null && member.IsAlive && member.Name != being.Name)
                    {
                        member.TakeDamage(e, DamageType.Normal, being);
                    }
                    
                }
                PartyMemberAOEDamageDone?.Invoke(PlayerParty, e);
            }
            //TODO --> IMPLEMENT ENEMY
        }

        public void OnPartyMemberAOEHealDone(object sender, float e)
        {
            Being being = (Being)sender;

            if (PlayerParty.Members.Contains(being))
            {
                foreach (var member in PlayerParty.Members)
                {
                    if (member.IsAlive && member.Name != being.Name)
                    {
                        member.Heal(e);
                    }
                }
            }
        }

        public async Task ProcessDefenseTurn()
        {
            tcsPlayerAction = new TaskCompletionSource();

            var timeout = Task.Delay(TimeSpan.FromSeconds(10));
            var completedTask = await Task.WhenAny(tcsPlayerAction.Task, timeout);

            if (completedTask == timeout)
            {
                
            }
            else if (completedTask == tcsPlayerAction.Task)
            {
                
            }
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
                    Console.WriteLine($"Processing {being.Name}");
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

            if (CurrentTurn.ActiveEffects.Any(x => x.Type == EffectType.Stun))
            {
                TurnEnded?.Invoke(this, EventArgs.Empty);
                await Task.Delay(5000);
                await OnTurnEnded(this, EventArgs.Empty);
                return;
            }

            tcsPlayerAction = new TaskCompletionSource();

            var timeout = Task.Delay(TimeSpan.FromSeconds(15));
            var completedTask = await Task.WhenAny(tcsPlayerAction.Task, timeout);

            if (completedTask == timeout)
            {
                await ProcessTurnAsync();
                TurnEnded?.Invoke(this, EventArgs.Empty);
                await Task.Delay(3000);
                await OnTurnEnded(this, EventArgs.Empty); // Directly call the method here
            }
            else if (completedTask == tcsPlayerAction.Task)
            {
                TurnEnded?.Invoke(this, EventArgs.Empty);
                await Task.Delay(3000);
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
            PlayerParty.PartyMemberAOEDamageDone -= OnPartyMemberAOEDamageDone;

            EncounterParty.PartyEvent -= OnPartyAction;
            EncounterParty.PartyMemberDeath -= OnPartyMemberDeath;
            EncounterParty.PartyMemberEffectApplied -= OnPartyMemberEffectApplied;
            EncounterParty.PartyMemberEffectRemoved -= OnPartyMemberEffectRemoved;
            EncounterParty.PartyMemberAOEDamageDone -= OnPartyMemberAOEDamageDone;

            DetermineRewards();

            IsEncounterActive = false;
            PlayerParty = null;
            EncounterParty = null;

            
        }

        private void DetermineRewards()
        {
            if (PlayerParty.IsAlive)
            {
                VictoryResult = "Your Party has won!";
                EncounterEnded?.Invoke(this, new GameEndedEventArgs(PlayerParty.Members, true));
            }
            else if (EncounterParty.IsAlive)
            {
                VictoryResult = "Your Party has been defeated..";
                EncounterEnded?.Invoke(this, new GameEndedEventArgs(PlayerParty.Members, false));
            }
        }
    }
}
