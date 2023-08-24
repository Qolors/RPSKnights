using LeagueStatusBot.Common.Models;
using LeagueStatusBot.RPGEngine.Data.Contexts;
using LeagueStatusBot.RPGEngine.Data.Entities;

namespace LeagueStatusBot.RPGEngine.Data.Repository
{
    public class PlayerRepository
    {
        private GameDbContext dbContext;
        public PlayerRepository(GameDbContext gameDbContext)
        {
            this.dbContext = gameDbContext;
        }

        public bool Exists(ulong key)
        {
            return dbContext
                .Set<BeingEntity>().Any(e => e.DiscordId == key);
        }

        public LootEntity GetPlayerLootTable(ulong key)
        {
            return dbContext
                .Loot.SingleOrDefault(b => b.DiscordId == key);
        }

        public int GetLootCount(ulong key)
        {
            if (!HasLootTableCreated(key))
            {
                AddPlayerLootTable(key);
            }

            return dbContext
                .Loot.SingleOrDefault(b => b.DiscordId == key).LootCount;
        }

        public bool HasLootTableCreated(ulong key)
        {
            return dbContext
                .Set<LootEntity>().Any(e => e.DiscordId == key);
        }

        public void AddPlayerLootTable(ulong key)
        {
            dbContext.Loot.Add(new LootEntity { DiscordId = key, LootCount = 0 });
            dbContext.SaveChanges();
        }

        public void AddToPlayerLootTable(ulong key)
        {
            if (!HasLootTableCreated(key))
            {
                AddPlayerLootTable(key);
            }

            var playerLoot = GetPlayerLootTable(key);
            playerLoot.LootCount++;

            dbContext.SaveChanges();
        }

        public bool SubtractFromPlayerLootTable(ulong key)
        {
            if (!HasLootTableCreated(key))
            {
                return false;
            }

            var playerLoot = GetPlayerLootTable(key);

            if (playerLoot.LootCount <= 0)
            {
                return false;
            }

            playerLoot.LootCount--;
            dbContext.SaveChanges();

            return true;
        }

        public BeingEntity? GetBeingByDiscordId(ulong key)
        {
            return dbContext
                .Beings.SingleOrDefault(b => b.DiscordId == key);
        }

        public void UpdatePlayerStats(ulong discordId, int strength, int luck, int endurance, int charisma, int intelligence, int agility)
        {
            var player = GetBeingByDiscordId(discordId);
            if (player == null)
            {
                throw new Exception($"Player with Discord ID {discordId} not found.");
            }

            // Update the stats
            player.Strength = strength;
            player.Luck = luck;
            player.Endurance = endurance;
            player.Charisma = charisma;
            player.Intelligence = intelligence;
            player.Agility = agility;

            dbContext.SaveChanges();
        }

        public bool AddPlayerByDiscordId(BeingEntity beingEntity)
        {
            Console.WriteLine("Seeing if Exists");

            if (Exists(beingEntity.DiscordId))
            {
                return false;
            }

            Console.WriteLine("Adding after found none");
            
            dbContext.Beings.Add(beingEntity);
            dbContext.SaveChanges();

            return true;
        }

        public bool DeletePlayerByDiscordId(ulong discordId)
        {
            var player = dbContext.Beings.FirstOrDefault(p => p.DiscordId == discordId);
            if (player == null)
            {
                return false; // Player not found
            }

            dbContext.Beings.Remove(player);
            dbContext.SaveChanges();

            return true; // Successfully deleted
        }

        public BeingEntity? UpdateEquipment(ulong discordId, ItemType type, int itemId)
        {
            var player = dbContext.Beings.FirstOrDefault(p => p.DiscordId == discordId);
            if (player == null)
            {
                return null; // Player not found
            }

            switch (type)
            {
                case ItemType.Helm:
                    player.Helm = itemId;
                    break;
                case ItemType.Chest:
                    player.Chest = itemId;
                    break;
                case ItemType.Legs:
                    player.Legs = itemId;
                    break;
                case ItemType.Boots:
                    player.Boots = itemId;
                    break;
                case ItemType.Gloves:
                    player.Gloves = itemId;
                    break;
                case ItemType.Weapon:
                    player.Weapon = itemId;
                    break;
                default:
                    throw new ArgumentException($"Unsupported ItemType: {type}");
            }

            dbContext.SaveChanges();

            return player;
        }

        public BeingEntity? AddToInventory(ulong discordId, int itemId)
        {
            var player = dbContext.Beings.FirstOrDefault(p => p.DiscordId == discordId);
            if (player == null)
            {
                return null; // Player not found
            }

            player.Inventory ??= new List<int>();

            if (!player.Inventory.Contains(itemId))
            {
                player.Inventory.Add(itemId);
            }

            dbContext.SaveChanges();

            return player;
        }

        public BeingEntity? RemoveFromInventory(ulong discordId, int itemId)
        {
            var player = dbContext.Beings.FirstOrDefault(p => p.DiscordId == discordId);
            if (player == null)
            {
                return null; // Player not found
            }

            if (player.Inventory == null)
            {
                return player; // Nothing to remove
            }

            if (player.Inventory.Contains(itemId))
            {
                player.Inventory.Remove(itemId);
            }

            dbContext.SaveChanges();

            return player;
        }


    }
}
