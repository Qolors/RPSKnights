

using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Factories.ArmorEffects;
using LeagueStatusBot.RPGEngine.Factories.ItemEffects;
using System.Reflection;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public abstract class Being
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public ActionPerformed LastActionPerformed { get; set; }
        public Item Weapon { get; set; }
        public IArmorEffect Helm { get; set; }
        public IArmorEffect Chest { get; set; }
        public IArmorEffect Gloves { get; set; }
        public IArmorEffect Boots { get; set; }
        public IArmorEffect Legs { get; set; }
        public IArmorEffect ActiveDefenseItem { get; set; } = null;
        public List<Item> Inventory { get; set; }
        public int HitPoints { get; set; }
        public int MaxHitPoints { get; set; }
        public Stats BaseStats { get; set; } = new Stats();
        public ulong DiscordId { get; set; }
        public Ability FirstAbility { get; set; }
        public Ability SecondAbility { get; set; }
        public virtual float ArmorClassValue { get; set; } = 0.1f;
        public float CurrentDamage { get; set; } = 0;
        public virtual DamageType Vulnerability { get; set; } = DamageType.Normal;
        public virtual DamageType Resistance { get; set; } = DamageType.Normal;
        public List<Effect> ActiveEffects { get; set; } = new List<Effect>();
        public List<string> EffectsLog { get; set; } = new List<string>();
        public List<IItemEffect> WeaponEffects { get; set; } = new List<IItemEffect>();
        public List<IItemEffect> ArmorEffects { get; set; } = new List<IItemEffect>();
        public bool IsHuman { get; set; } = false;
        public bool IsAlive => HitPoints > 0;
        public Being? Target { get; set; }

        public event EventHandler<string> ActionPerformed;
        public event EventHandler<float> AoeDamagePerformed;
        public event EventHandler<float> AoeHealPerformed;
        public event EventHandler<string> DamageTaken;
        public event EventHandler<string> EffectApplied;
        public event EventHandler<string> EffectRemoved;

        public Action<Being> OnDamageGiven;
        public Action<Being> OnDamageTaken;

        public event EventHandler Killed;

        public bool AnyArmorUnused()
        {
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.PropertyType == typeof(IArmorEffect))
                {
                    var armorEffect = (IArmorEffect)prop.GetValue(this);
                    if (armorEffect != null && !armorEffect.IsUsed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void InitializeAbilities()
        {
            Weapon.Effect.Register(this);
        }

        public virtual float BasicAttack(float strAdRatio = 1.0f)
        {
            CurrentDamage = 0;

            float dmg = 0.1f;

            // Effects
            foreach (Effect effect in ActiveEffects)
            {
                if (effect.Type == EffectType.BasicDamageBoost)
                {
                    dmg += effect.ModifierAmount;
                }
            }

            // Strength Modifier
            dmg += BaseStats.Strength * 0.2f;

            CurrentDamage =  dmg * strAdRatio;

            OnDamageGiven?.Invoke(this);

            return CurrentDamage;
        }

        public void UseWeaponActive()
        {
            CurrentDamage = this.BasicAttack();

            Weapon.Effect.OnExecuteActive(this);

            this.Target?.TakeDamage(CurrentDamage, DamageType.Normal, this);

            LastActionPerformed = Common.Models.ActionPerformed.WeaponAbility;

            CurrentDamage = 0;
        }

        public virtual void ChosenAbility(Ability ability)
        {
            // Existing code
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Used {ability.Name}!");

            float abilityDmg = ability.Activate(this, this.Target);

            if (abilityDmg == 0) return;

            OnDamageGiven?.Invoke(this);

            this.Target?.TakeDamage(abilityDmg, ability.DamageType, this);
        }


        public virtual void TakeDamage(float enemyDamageRoll, DamageType dmgType, Being enemy)
        {
            //BEFORE MODIFICATIONS MADE
            float modifiedDmg = GetModifiedDamage(enemyDamageRoll, dmgType);
            if (modifiedDmg == 0)
            {
                ActionPerformed.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Dodged the attack!");
                return;
            }
            //AFTER MODIFICATIONS MADE
            ApplyDamage(modifiedDmg, enemy);
        }

        private float GetModifiedDamage(float damage, DamageType dmgType)
        {
            float damageWithMultiplier = damage * DamageMultiplier(dmgType);

            // Endurance Modifier
            float EnduranceReduction = BaseStats.Endurance * 0.1f;
            damageWithMultiplier -= EnduranceReduction;

            // Effects
            float reducedDamage = ApplyEffects(damageWithMultiplier);

            // Agility (Dodge)
            if (new Random().NextDouble() < BaseStats.Agility * 0.01)
            {
                
                return 0;
            }

            return reducedDamage * (1 - DefenseModifier() / 100f);
        }

        private void ApplyDamage(float damage, Being enemy)
        {
            int finalDamage = Convert.ToInt32(Math.Max(0, damage));

            OnDamageTaken?.Invoke(this);

            if (ActiveDefenseItem != null)
            {
                ActiveDefenseItem.ActivateArmor(this, enemy, finalDamage);
                ActiveDefenseItem = null;
                return;
            }

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

        public virtual void TakeEffect(float effectDamage, EffectType effectType)
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
            float rawDefense = this.BaseStats.Endurance * 2;

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

            // Luck Modifier (Critical Hit)
            if (new Random().NextDouble() < BaseStats.Luck * 0.01)
            {
                attackRoll *= 1.5f;
                ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: CRITICAL HIT {Target.Name}!");
            }
            else
            {
                ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Attacked {Target.Name}!");
            }

            CurrentDamage = attackRoll;

            OnDamageGiven?.Invoke(this);

            this.LastActionPerformed = Common.Models.ActionPerformed.BasicAttack;

            Target.TakeDamage(CurrentDamage, DamageType.Normal, this);
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
                    TakeEffect(ActiveEffects[i].ModifierAmount, ActiveEffects[i].Type);

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

        public void DealAOEDamage(float dmg)
        {
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: AOE Hit!");
            AoeDamagePerformed?.Invoke(this, dmg);
        }

        public void HealAOEDamage(float dmg)
        {
            AoeHealPerformed?.Invoke(this, dmg);
        }

        public void Heal(float heal)
        {
            HitPoints += (int)heal;
            if (HitPoints > MaxHitPoints) HitPoints = MaxHitPoints;
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: Healed {heal} HP!");
        }

        public void BroadCast(string effect)
        {
            ActionPerformed?.Invoke(this, $"[{Name} ({HitPoints}/{MaxHitPoints})]: activated {effect}!");
        }

        public virtual void SetTarget(Being target)
        {
            this.Target = target;
        }

    }
}
