using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Factories;
using LeagueStatusBot.RPGEngine.Data.Entities;
using LeagueStatusBot.RPGEngine.Data.Repository;
using System.Collections.Generic;

namespace LeagueStatusBot.Helpers
{
    public static class Mapper
    {
        private static PlayerRepository playerRepository;
        private static ItemRepository itemRepository;
        private static ItemEffectFactory itemEffectFactory;

        public static void Initialize(PlayerRepository playerRepo, ItemRepository itemRepo)
        {
            playerRepository = playerRepo;
            itemRepository = itemRepo;
            itemEffectFactory = new ItemEffectFactory();
        }
        public static Being BeingEntityToDomainModel(this BeingEntity beingEntity)
        {
            Being being = beingEntity.ClassName switch
            {
                "Adventurer" => ClassFactory.CreateAdventurer(),
                "Apprentice" => ClassFactory.CreateApprentice(),
                "Vagabond" => ClassFactory.CreateVagabond(),
                _ => null,
            };
            being.BaseStats = new()
            {
                Agility = beingEntity.Agility,
                Charisma = beingEntity.Charisma,
                Endurance = beingEntity.Endurance,
                Intelligence = beingEntity.Intelligence,
                Luck = beingEntity.Luck,
                Strength = beingEntity.Strength,
            };

            being.DiscordId = beingEntity.DiscordId;
            being.MaxHitPoints = beingEntity.MaxHitPoints;
            being.HitPoints = beingEntity.MaxHitPoints;
            being.Helm = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Chest = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Gloves = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Boots = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Legs = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(1));
            being.Weapon = ItemEntityToDomainModel(itemRepository.GetItemFromEntityId(beingEntity.Weapon));
            being.Name = beingEntity.Name;
            being.ClassName = beingEntity.ClassName;
            being.Inventory = new List<Item>();
            being.ActiveEffects = new List<Effect>();

            return being;

        }

        public static BeingEntity BeingToEntityModel(this Being being)
        {
            BeingEntity beingEntity = new()
            {
                DiscordId = being.DiscordId,
                Weapon = being.Weapon.ItemId,
                Helm = being.Helm.ItemId,
                Chest = being.Chest.ItemId,
                Gloves = being.Gloves.ItemId,
                Boots = being.Boots.ItemId,
                Legs = being.Legs.ItemId,
                Name = being.Name,
                ClassName = being.ClassName,
                Strength = being.BaseStats.Strength,
                Luck = being.BaseStats.Luck,
                Endurance = being.BaseStats.Endurance,
                Intelligence = being.BaseStats.Intelligence,
                Charisma = being.BaseStats.Charisma,
                Agility = being.BaseStats.Agility,
                MaxHitPoints = being.MaxHitPoints,
                Inventory = new List<int>()
            };

            return beingEntity;
        }

        public static ItemEntity ItemToEntityModel(this Item item)
        {
            return new ItemEntity
            {
                ItemId = item.ItemId,
                ItemEffect = item.Effect.EffectId,
                ItemName = item.ItemName,
                ItemType = item.ItemType,
                Rarity = item.Rarity
            };
        }

        public static Item ItemEntityToDomainModel(this ItemEntity itemEntity)
        {
            var effect = itemRepository.GetEffectById(itemEntity.ItemEffect);

            var item = new Item
            {
                ItemId = itemEntity.ItemId,
                ItemName = itemEntity.ItemName,
                ItemType = itemEntity.ItemType,
                Rarity = itemEntity.Rarity,
            };

            item.Effect = itemEffectFactory.GetEffect(itemEntity.ItemEffect);
            item.Effect.Name = effect.EffectName;
            item.Effect.Description = effect.Description;
            item.Effect.EffectId = effect.EffectId;

            return item;
        }
    }
}
