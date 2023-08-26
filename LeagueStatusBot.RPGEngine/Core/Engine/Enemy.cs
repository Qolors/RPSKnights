using LeagueStatusBot.RPGEngine.Factories.Monsters;
using LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities;

namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Enemy : Being
    {
        public Enemy(string name, int enemyPowerScore)
        {
            Name = name;
            ClassName = "Monster";
            BaseStats = Stats.RollStatsToPowerScore(enemyPowerScore);
            HitPoints = MaxHitPoints;
        }

        public Enemy(Monster monster)
        {
            Name = monster.Name;
            ClassName = monster.ClassName;
            BaseStats = Stats.RollStatsToPowerScore(monster.EnemyPowerScore);
            HitPoints = MaxHitPoints;

            if (monster.Abilities != null && monster.Abilities.Count > 0)
            {
                FirstAbility = LoadAbilityFromTemplate(monster.Abilities[0]);

                if (monster.Abilities.Count > 1)
                {
                    SecondAbility = LoadAbilityFromTemplate(monster.Abilities[1]);
                }
            }
        }

        private Ability LoadAbilityFromTemplate(AbilityTemplate template)
        {
            // This is a simple example. You may have different ability classes for different implementations.
            switch (template.Implementation)
            {
                case "FireBreath":
                    return new FireBreath(template);
                case "TailSwipe":
                    return new TailSwipe(template);
                case "Constrict":
                    return new Constrict(template);
                case "DiveBomb":
                    return new DiveBomb(template);
                case "JustTwo":
                    return new JustTwo(template);
                case "VenomStrike":
                    return new VenomStrike(template);
                case "VerbalAssault":
                    return new VerbalAssault(template);
                case "WingSlash":
                    return new WingSlash(template);
                // ... add other cases for other ability implementations.
                default:
                    throw new Exception($"Unknown ability implementation: {template.Implementation}");
            }
        }

        public override void AttackTarget()
        {
            if (Target == null) return;

            float attackRoll;

            AttackType chosenAttack = ChooseAttack();

            switch (chosenAttack)
            {
                case AttackType.Basic:
                    attackRoll = this.BasicAttack();
                    break;
                case AttackType.FirstAbility:
                    attackRoll = FirstAbility.Activate(this, Target);
                    break;
                case AttackType.SecondAbility:
                    attackRoll = SecondAbility.Activate(this, Target);
                    break;
                default:
                    attackRoll = this.BasicAttack();
                    break;
            }

            Console.WriteLine("Attack Type: " + chosenAttack);

            // Luck Modifier (Critical Hit)
            if (new Random().NextDouble() < BaseStats.Luck * 0.01)
            {
                attackRoll *= 1.5f;
                this.OnActionPerformed($"[{Name} ({HitPoints}/{MaxHitPoints})]: CRITICAL HIT {Target.Name}!");
            }
            else
            {
                this.OnActionPerformed($"[{Name} ({HitPoints}/{MaxHitPoints})]: {chosenAttack.ToString()} {Target.Name}!");
            }

            CurrentDamage = attackRoll;

            OnDamageGiven?.Invoke(this);

            this.LastActionPerformed = (chosenAttack == AttackType.Basic)
            ? Common.Models.ActionPerformed.BasicAttack
            : (chosenAttack == AttackType.FirstAbility
                ? Common.Models.ActionPerformed.FirstAbility
                : Common.Models.ActionPerformed.SecondAbility);


            Target.TakeDamage(CurrentDamage, DamageType.Normal, this);  // Assuming DamageType.Normal for now, you might want to adjust based on chosen attack type.
        }

        enum AttackType
        {
            Basic,
            FirstAbility,
            SecondAbility
        }

        AttackType ChooseAttack()
        {
            bool firstAbilityAvailable = (FirstAbility.Cooldown == 0);
            bool secondAbilityAvailable = (SecondAbility.Cooldown == 0);

            Console.WriteLine(firstAbilityAvailable);
            Console.WriteLine(secondAbilityAvailable);

            Random rng = new Random();

            if (firstAbilityAvailable && secondAbilityAvailable)
            {
                int choice = rng.Next(3);  // Random number between 0 and 2
                return (AttackType)choice;
            }
            else if (firstAbilityAvailable)
            {
                return (rng.Next(2) == 0) ? AttackType.Basic : AttackType.FirstAbility;
            }
            else if (secondAbilityAvailable)
            {
                return (rng.Next(2) == 0) ? AttackType.Basic : AttackType.SecondAbility;
            }
            else
            {
                return AttackType.Basic;
            }
        }



        public override float BasicAttack(float strAdRatio = 1)
        {
            return base.BasicAttack(strAdRatio);
        }

        public override void TakeDamage(float damage, DamageType damageType, Being attacker)
        {
            base.TakeDamage(damage, damageType, attacker);
        }
    }
}
