

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public abstract class Being
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public Item Weapon { get; set; }
        public Item Helm { get; set; }
        public Item Chest { get; set; }
        public Item Gloves { get; set; }
        public Item Boots { get; set; }
        public Item Legs { get; set; }
        public List<Item> Inventory { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public Stats BaseStats { get; set; } = new Stats();
        public ulong DiscordId { get; set; }
        public Ability FirstAbility { get; set; }
        public Ability SecondAbility { get; set; }
        public virtual float ArmorClassValue { get; set; } = 0.1f;
        public virtual DamageType Vulnerability { get; set; } = DamageType.Normal;
        public virtual DamageType Resistance { get; set; } = DamageType.Normal;
        public List<Effect> ActiveEffects { get; set; } = new List<Effect>();
        public bool IsHuman { get; set; } = false;
        public bool IsAlive => HitPoints > 0;
        public Being? Target { get; set; }

        public event EventHandler<string> ActionPerformed;
        public event EventHandler<string> DamageTaken;
        public event EventHandler<string> EffectApplied;
        public event EventHandler<string> EffectRemoved;

        public event EventHandler Killed;

        public virtual float BasicAttack(float strAdRatio = 1.0f)
        {
            float dmg = 0.1f;

            foreach(Effect effect in ActiveEffects)
            {
                if (effect.Type == EffectType.BasicDamageBoost)
                {
                    dmg = effect.ModifierAmount;
                }
            }

            return (1 + dmg * this.BaseStats.Strength) * strAdRatio;
        }

        public virtual void ChosenAbility(Ability ability)
        {
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Used {ability.Name}!");

            float abilityDmg = ability.Activate(this, this.Target);

            if (abilityDmg == 0) return;

            this.Target?.TakeDamage(abilityDmg, ability.DamageType);
        }

        public virtual void TakeDamage(float enemyDamageRoll, DamageType dmgType)
        {
            float modifiedDmg = GetModifiedDamage(enemyDamageRoll, dmgType);
            ApplyDamage(modifiedDmg);
        }

        private float GetModifiedDamage(float damage, DamageType dmgType)
        {
            float damageWithMultiplier = damage * DamageMultiplier(dmgType);
            float reducedDamage = ApplyEffects(damageWithMultiplier);

            return reducedDamage * (1 - DefenseModifier() / 100f);
        }

        private void ApplyDamage(float damage)
        {
            int finalDamage = Convert.ToInt32(Math.Max(0, damage));

            HitPoints -= finalDamage;

            if (HitPoints < 0) HitPoints = 0;

            DamageTaken?.Invoke(this, $"- {Name} took {finalDamage} damage. HP is Now ({this.HitPoints}/{this.MaxHitPoints})\n");

            if (!IsAlive) Killed?.Invoke(this, EventArgs.Empty);
        }

        private float ApplyEffects(float damage)
        {
            foreach (Effect effect in ActiveEffects)
            {
                if (effect.Type == EffectType.DamageReduction)
                {
                    damage *= (1 - effect.ModifierAmount);
                }
            }
            return damage;
        }

        public virtual void TakeEffectDamage(float effectDamage, EffectType effectType)
        {

            if (effectType == EffectType.Bleed)
            {
                float finalDmg = MaxHitPoints * effectDamage;
                int finalDmgRound = 1 + Convert.ToInt32(finalDmg);
                HitPoints -= finalDmgRound;
                DamageTaken?.Invoke(this, $"- {Name} bled for {finalDmgRound} damage. HP is Now ({this.HitPoints}/{this.MaxHitPoints})\n");
            }

            if (!IsAlive) Killed?.Invoke(this, EventArgs.Empty);
        }

        public float DefenseModifier()
        {
            float rawDefense = this.BaseStats.Endurance + (this.BaseStats.Agility * 2);

            // Convert rawDefense into a percentage, and cap it between 0 and 100.
            float defensePercentage = Math.Clamp(rawDefense, 0, 100);

            return defensePercentage;
        }

        public float DamageMultiplier(DamageType dmgType)
        {
            if (dmgType == Vulnerability) return 1.5f;
            if (dmgType == Resistance) return 0.5f;
            return 1.0f;
        }

        public virtual void AttackTarget()
        {
            if (Target == null) return;

            //TODO --> WEAPON SYSTEM SHOULD REPLACE DAMAGE CALCULATIONS
            float attackRoll = this.BasicAttack();

            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Attacked {Target.Name}!");

            Target.TakeDamage(attackRoll, DamageType.Normal);
        }

        public void AddEffect(Effect effect)
        {
            ActiveEffects.Add(effect);
            EffectApplied?.Invoke(this, $"{Name} has {effect.Name} applied for {effect.Duration} round!\n");
        }

        public void AddDelayedEffect(Effect effect)
        {
            ActiveEffects.Add(effect);
        }

        public void RemoveEffect(Effect effect)
        {
            ActiveEffects.Remove(effect);
            EffectRemoved?.Invoke(this, $"{Name}'s {effect.Name} wore off..\n");
        }

        public void ProcessEndOfRound()
        {
            for (int i = ActiveEffects.Count - 1; i >= 0; i--)
            {
                if (ActiveEffects[i].BufferDuration > 0)
                {
                    ActiveEffects[i].BufferDuration--;

                    if (ActiveEffects[i].BufferDuration == 0)
                    {
                        EffectApplied?.Invoke(this, $"{Name} has {ActiveEffects[i].Name} applied for {ActiveEffects[i].Duration} round!\n");
                    }
                }
                else
                {
                    TakeEffectDamage(ActiveEffects[i].ModifierAmount, ActiveEffects[i].Type);

                    ActiveEffects[i].Duration--;

                    if (ActiveEffects[i].Duration <= 0)
                    {
                        EffectRemoved?.Invoke(this, $"{Name}'s {ActiveEffects[i].Name} wore off..\n");
                        ActiveEffects.RemoveAt(i);
                    }
                }
            }

            if (IsHuman)
            {
                if (FirstAbility.Cooldown > 0)
                {
                    FirstAbility.Cooldown--;
                }
                if (SecondAbility.Cooldown > 0)
                {
                    SecondAbility.Cooldown--;
                }
            }
        }

        public virtual void SetTarget(Being target)
        {
            this.Target = target;
        }

    }
}
