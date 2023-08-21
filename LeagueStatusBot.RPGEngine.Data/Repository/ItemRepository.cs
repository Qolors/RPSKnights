﻿using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;

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
                return false;
            }

            _context.Items.Remove(item);
            _context.SaveChanges();

            return true;
        }

        public ItemEntity GenerateRandomWeapon()
        {
            var randomEffectId = GetRandomEffect();

            var newItem = new ItemEntity
            {
                ItemName = randomEffectId.EffectName,
                ItemType = ItemType.Weapon,
                Rarity = ItemRarity.Enchanted,
                ItemEffect = randomEffectId.EffectId
            };

            return AddItem(newItem);
        }

        public ItemEntity? GetItemFromEntityId(int id)
        {
            return _context.Items.FirstOrDefault(x => x.ItemId == id);
        }

        private ItemEffectEntity GetRandomEffect()
        {
            int[] numbers = { 2, 3, 4 };
            var effects = _context.ItemEffects.ToList();
            var random = new Random();
            var randomEffect = effects[random.Next(2, numbers.Length)];

            return randomEffect;
        }

        public ItemEffectEntity GetEffectById(int id)
        {
            return _context.ItemEffects.FirstOrDefault(x => x.EffectId == id);
        }
    }

}
