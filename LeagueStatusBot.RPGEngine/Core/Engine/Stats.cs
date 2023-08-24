
namespace LeagueStatusBot.RPGEngine.Core.Engine
{
    public class Stats
    {
        public int Strength { get; set; } = 10; //Attack Damage
        public int Luck { get; set; } = 10; //Crit Chance
        public int Endurance { get; set; } = 10; //Max HP
        public int Charisma { get; set; } = 10; //Healing & Buffs
        public int Intelligence { get; set; } = 10; //Magic Damage
        public int Agility { get; set; } = 10; //Dodge Chance & Initiative

        public int GearScore => Strength + Luck + Endurance + Charisma + Intelligence + Agility;

        public static Stats RollStatsToPowerScore(int powerScore)
        {
            if (powerScore < 30) // Minimum possible score to assign 5 to each property
                throw new ArgumentException("Power score is too low!");

            Stats stats = new Stats
            {
                Strength = 5,
                Luck = 5,
                Endurance = 5,
                Charisma = 5,
                Intelligence = 5,
                Agility = 5
            };

            int excess = powerScore - 30; // subtracting the total minimum from the given powerScore
            Random rand = new Random();

            while (excess > 0)
            {
                int randomStat = rand.Next(6);
                switch (randomStat)
                {
                    case 0:
                        stats.Strength++;
                        break;
                    case 1:
                        stats.Luck++;
                        break;
                    case 2:
                        stats.Endurance++;
                        break;
                    case 3:
                        stats.Charisma++;
                        break;
                    case 4:
                        stats.Intelligence++;
                        break;
                    case 5:
                        stats.Agility++;
                        break;
                }
                excess--;
            }

            return stats;
        }
    }
}
