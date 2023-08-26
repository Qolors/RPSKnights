using LeagueStatusBot.RPGEngine.Core.Engine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace LeagueStatusBot.RPGEngine.Factories.Monsters
{
    public static class MonsterLoader
    {
        public static Enemy GetRandomEnemy(int powerScore)
        {
            var monsters = LoadMonstersFromJson("monsters.json");

            if (monsters == null || monsters.Count == 0)
            {
                throw new Exception("No monsters loaded from JSON.");
            }

            var random = new Random();
            var selectedMonster = monsters[random.Next(monsters.Count)];
            selectedMonster.EnemyPowerScore = powerScore;

            return new Enemy(selectedMonster);
        }

        private static List<Monster> LoadMonstersFromJson(string filePath)
        {
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Monster>>(jsonData);
            }
            else
            {
                throw new FileNotFoundException($"The file at path {filePath} could not be found.");
            }
        }


    }

    public class AbilityTemplate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cooldown { get; set; }
        public string DamageType { get; set; }
        public string Implementation { get; set; }
    }

    public class Monster
    {
        public string Name { get; set; }
        public string ClassName { get; set; }
        public int EnemyPowerScore { get; set; }
        public List<AbilityTemplate> Abilities { get; set; }
    }
}
