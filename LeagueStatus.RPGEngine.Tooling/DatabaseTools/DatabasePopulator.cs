using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using LeagueStatusBot.RPGEngine.Data.Entities;
using LeagueStatusBot.RPGEngine.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LeagueStatusBot.RPGEngine.Tooling.DatabaseTools
{
    public class DatabasePopulator
    {
        public DatabasePopulator(string[] args) 
        {
            string dataType = args[0];
            string jsonPath = args[1];


            if (!File.Exists(jsonPath) || !jsonPath.EndsWith(".json"))
            {
                Console.WriteLine("File does not exist or is not a json file");
                return;
            }

            var jsonData = File.ReadAllText(jsonPath);

            switch (dataType)
            {
                case "add-items":
                    AddItems(jsonData);
                    break;

                case "add-itemeffects":
                    AddItemEffects(jsonData);
                    break;

                case "add-beings":
                    AddBeings(jsonData);
                    break;

                case "peek-db":
                    SeeItemEffects();
                    break;

                default:
                    Console.WriteLine("No Recognized Action");
                    break;
            }

        }

        public void AddItems(string jsonData)
        {
            var items = JsonConvert.DeserializeObject<List<ItemEntity>>(jsonData);

            if (items  == null || items.Count == 0)
            {
                Console.WriteLine("Incorrect Format for ItemEntity List");
                return;
            }
            var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlite("Data Source=game.db");
            using var db = new GameDbContext(optionsBuilder.Options);

            db.Items.AddRange(items);
            db.SaveChanges();

            Console.WriteLine("Added Items to Database");

        }

        public void AddItemEffects(string jsonData)
        {
            var items = JsonConvert.DeserializeObject<List<ItemEffectEntity>>(jsonData);

            if (items == null || items.Count == 0)
            {
                Console.WriteLine("Incorrect Format for ItemEffectEntity List");
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlite("Data Source=game.db");
            using var db = new GameDbContext(optionsBuilder.Options);

            db.ItemEffects.AddRange(items);
            db.SaveChanges();

            Console.WriteLine("Added ItemEffects to Database");
        }

        public void SeeItemEffects()
        {
            var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlite("Data Source=game.db");
            using var db = new GameDbContext(optionsBuilder.Options);

            var itemEffects = db.ItemEffects.ToList();

            foreach (var itemEffect in itemEffects)
            {
                Console.WriteLine($"EffectId: {itemEffect.EffectId} EffectName: {itemEffect.EffectName} EffectDescription: {itemEffect.Description}");
            }
        }

        public void AddBeings(string jsonData)
        {
            var items = JsonConvert.DeserializeObject<List<BeingEntity>>(jsonData);

            if (items == null || items.Count == 0)
            {
                Console.WriteLine("Incorrect Format for BeingEntity List");
                return;
            }

            var optionsBuilder = new DbContextOptionsBuilder<GameDbContext>();
            optionsBuilder.UseSqlite("Data Source=game.db");
            using var db = new GameDbContext(optionsBuilder.Options);

            db.Beings.AddRange(items);
            db.SaveChanges();

            Console.WriteLine("Added Beings to Database");
        }
    }
}
