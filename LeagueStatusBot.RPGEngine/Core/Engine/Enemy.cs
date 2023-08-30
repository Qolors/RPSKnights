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
            return template.Implementation switch
            {
                "FireBreath" => new FireBreath(template),
                "TailSwipe" => new TailSwipe(template),
                "Constrict" => new Constrict(template),
                "DiveBomb" => new DiveBomb(template),
                "JustTwo" => new JustTwo(template),
                "VenomStrike" => new VenomStrike(template),
                "VerbalAssault" => new VerbalAssault(template),
                "WingSlash" => new WingSlash(template),
                _ => throw new Exception($"Unknown ability implementation: {template.Implementation}"),
            };
        }

        public override void AttackTarget()
        {
            if (Target == null) return;
            AttackType chosenAttack = ChooseAttack();
            var attackRoll = chosenAttack switch
            {
                AttackType.Basic => this.BasicAttack(),
                AttackType.FirstAbility => FirstAbility.Activate(this, Target),
                AttackType.SecondAbility => SecondAbility.Activate(this, Target),
                _ => this.BasicAttack(),
            };
            Console.WriteLine("Attack Type: " + chosenAttack);

            CurrentDamage = attackRoll;

            OnDamageGiven?.Invoke(this);

            this.LastActionPerformed = (chosenAttack == AttackType.Basic)
            ? Common.Models.ActionPerformed.BasicAttack
            : (chosenAttack == AttackType.FirstAbility
                ? Common.Models.ActionPerformed.FirstAbility
                : Common.Models.ActionPerformed.SecondAbility);

            var attackString = chosenAttack switch
            {
                AttackType.Basic => "Basic Attack",
                AttackType.FirstAbility => FirstAbility.Name,
                AttackType.SecondAbility => SecondAbility.Name,
                _ => "Basic Attack"
            };

            this.OnActionPerformed($"**{Name}**: {attackString} {Target.Name}");

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
