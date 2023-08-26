using LeagueStatusBot.RPGEngine.Core.Engine;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters.Abilities
{
    public class FireBreath : Ability
    {

        public FireBreath(AbilityTemplate template)
        {
            Name = template.Name;

            Description = template.Description;

            Cooldown = 0;

            DamageType = Enum.Parse<DamageType>(template.DamageType);
        }

        public FireBreath()
        {
            Name = "Fire Breath";
            Description = "High damage spell";
            Cooldown = 0;
            DamageType = DamageType.Magic;
        }

        public override float Activate(Being user, Being? target)
        {
            if (target == null) return 0;

            float baseDamage = user.BaseStats.Intelligence * 2;

            bool isCrit = new Random().Next(100) < user.BaseStats.Luck * 2;

            float totalDamage = isCrit ? baseDamage * 1.5f : baseDamage;

            user.BroadCast($"{user.Name} used {this.Name} on {target.Name}!");

            //target.TakeDamage(totalDamage, DamageType, user);

            Cooldown = 3;

            return totalDamage;
        }

        public override double ExpectedDamage(Being user)
        {
            return user.BaseStats.Intelligence * 2 * 1.25;
        }

    }

}
