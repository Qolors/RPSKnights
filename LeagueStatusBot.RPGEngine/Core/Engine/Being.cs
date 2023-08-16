using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public abstract class Being
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public Stats BaseStats { get; set; } = new Stats();
        public ulong DiscordId { get; set; }
        public Ability FirstAbility { get; set; }
        public Ability SecondAbility { get; set; }
        public virtual float ArmorClassValue { get; } = 0.1f;
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

        public virtual float ChosenAttack(float strAdRatio = 1.0f)
        {
            return (1 + 0.1f * this.BaseStats.Strength) * strAdRatio;
        }

        public virtual void ChosenAbility(Ability ability)
        {
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Used {ability.Name}!");

            float abilityDmg = ability.Activate(this, this.Target);

            if (abilityDmg == 0) return;

            this.Target?.TakeDamage(abilityDmg, DamageType.Normal);
        }

        public virtual void TakeDamage(float enemyDamageRoll, DamageType dmgType)
        {
            // This is the damage the enemy will deal, modified by the damage type (0.5x, 1.0x, 1.5x)
            float modifiedDmg = enemyDamageRoll * DamageMultiplier(dmgType);

            foreach (Effect effect in ActiveEffects)
            {
                if (effect.Type == EffectType.DamageReduction)
                {
                    modifiedDmg *= (1 - effect.ModifierAmount);
                }
            }


            // Get the defense multiplier - a value between 0 (no defense) to 1 (full defense)
            float defenseMultiplier = this.DefenseModifier() / 100f;
            // Reduce the damage by the defense multiplier
            float finalDmg = modifiedDmg * (1 - defenseMultiplier);

            finalDmg = Math.Max(0, finalDmg);

            int finalDmgRound = Convert.ToInt32(finalDmg);

            HitPoints -= finalDmgRound;

            if (HitPoints < 0) HitPoints = 0;

            DamageTaken?.Invoke(this, $"- {Name} took {finalDmgRound} damage. HP is Now ({this.HitPoints}/{this.MaxHitPoints})\n");

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
            float attackRoll = this.ChosenAttack();

            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Attacked {Target.Name}!");

            Target.TakeDamage(attackRoll, DamageType.Normal);
        }

        public void AddEffect(Effect effect)
        {
            ActiveEffects.Add(effect);
            EffectApplied?.Invoke(this, $"{Name} applied {effect.Name} for {effect.Duration} round!\n");
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
                ActiveEffects[i].Duration--;

                if (ActiveEffects[i].Duration <= 0)
                {
                    EffectRemoved?.Invoke(this, $"{Name}'s {ActiveEffects[i].Name} wore off..\n");
                    ActiveEffects.RemoveAt(i);
                }
            }
        }

        public virtual void SetTarget(Being target)
        {
            this.Target = target;
        }

    }
}
