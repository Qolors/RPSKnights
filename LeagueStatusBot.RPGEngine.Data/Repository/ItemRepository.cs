using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.RPGEngine.Data.Repository
{
    public class ItemRepository
    {
        private readonly GameDbContext _context;

        public ItemRepository(GameDbContext context)
        {
            _context = context;
        }

        public ItemEntity AddItem(ItemEntity item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();

            return item;
        }

        public bool DeleteItem(int itemId)
        {
            var item = _context.Items.FirstOrDefault(i => i.ItemId == itemId);
            if (item == null)
            {
                return false; // Item not found
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return true;
        }

        public ItemEntity GenerateRandomItem()
        {
            var randomEffectId = GetRandomEffectId();

            var newItem = new ItemEntity
            {
                ItemName = $"Generated Item {Guid.NewGuid()}",
                ItemType = /* Assign appropriate ItemType */,
                Rarity = /* Assign appropriate ItemRarity */,
                ItemEffect = randomEffectId
            };

            return AddItem(newItem);
        }

        private int GetRandomEffectId()
        {
            var effects = _context.EffectEntities.ToList();
            var random = new Random();
            var randomEffect = effects[random.Next(effects.Count)];

            return randomEffect.EffectId;
        }
    }

}
