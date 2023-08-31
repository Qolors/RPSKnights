using LeagueStatusBot.RPGEngine.Core.Engine;
using LeagueStatusBot.RPGEngine.Factories;
using LeagueStatusBot.RPGEngine.Data.Entities;
using LeagueStatusBot.RPGEngine.Data.Repository;
using System.Collections.Generic;
using LeagueStatusBot.RPGEngine.Factories.ArmorEffects;
using System;

namespace LeagueStatusBot.Helpers
{
    public static class Mapper
    {
        private static PlayerRepository playerRepository;
        private static ItemRepository itemRepository;
        private static MonsterRepository monsterRepository;
        private static ItemEffectFactory itemEffectFactory;
        private static ArmorEffectFactory armorEffectFactory;

        public static void Initialize(PlayerRepository playerRepo, ItemRepository itemRepo, MonsterRepository monsterRepo)
        {
            playerRepository = playerRepo;
            itemRepository = itemRepo;
            monsterRepository = monsterRepo;
            itemEffectFactory = new ItemEffectFactory();
            armorEffectFactory = new ArmorEffectFactory();
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
            being.HitPoints = being.MaxHitPoints;
            being.Helm = ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(beingEntity.Helm));
            being.Chest = ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(beingEntity.Chest));
            being.Gloves = ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(beingEntity.Gloves));
            being.Boots = ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(beingEntity.Boots));
            being.Legs = ArmorEffectEntityToDomainModel(itemRepository.GetArmorFromId(beingEntity.Legs));
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
                Helm = being.Helm.EffectId,
                Chest = being.Chest.EffectId,
                Gloves = being.Gloves.EffectId,
                Boots = being.Boots.EffectId,
                Legs = being.Legs.EffectId,
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

        public static IArmorEffect ArmorEffectEntityToDomainModel(this ArmorEffectEntity armorEntity)
        {
            Console.WriteLine(armorEntity.Name);
            Console.WriteLine(armorEntity.EffectId.ToString());
            return armorEffectFactory.GetEffect(armorEntity.EffectId);
        }

        public static ArmorEffectEntity ArmorToArmorEntity(this IArmorEffect armorEffect)
        {
            Console.WriteLine(armorEffect.EffectId.ToString());
            return new ArmorEffectEntity
            {
                Description = armorEffect.Description,
                EffectFor = armorEffect.EffectFor,
                EffectId = armorEffect.EffectId,
                Name = armorEffect.Name,
            };
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

        public static Monster MonsterEntityToDomainModel(this SuperMonsterEntity monsterEntity)
        {
            return new Monster(monsterEntity.FirstSuper, monsterEntity.SecondSuper, monsterEntity.Name, monsterEntity.Description);
        }

        public static Campaign CampaignEntityToDomainModel(this CampaignEntity campaignEntity)
        {
            return new Campaign
            {
                IntroductionPost = campaignEntity.IntroductionPost,
                IntroductionImageUrl = campaignEntity.IntroductionImageUrl,
                MidPost = campaignEntity.MidPost,
                MidPostImageUrl = campaignEntity.MidPostImageUrl,
                PreFightPost = campaignEntity.PreFightPost,
                PreFightPostImageUrl = campaignEntity.PreFightPostImageUrl,
                MonsterName = campaignEntity.MonsterName,
            };
        }
    }
}
