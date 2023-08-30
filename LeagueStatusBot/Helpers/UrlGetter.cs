

using System.Collections.Generic;

namespace LeagueStatusBot.Helpers
{
    public static class UrlGetter
    {
        private static Dictionary<string, string> MonsterUrls;
        private static Dictionary<string, string> AbilityUrls;
        private static Dictionary<string, string> WeaponUrls;
        private static Dictionary<string, string> ArmorUrls;
        private static Dictionary<string, string> ClassUrls;

        public static void Initialize()
        {
            MonsterUrls = new Dictionary<string, string>
            {
                { "Bragore the Wretched", "https://i.imgur.com/GnEgSHG.png"},
                { "Lord Tusker", "https://i.imgur.com/RsMf6Rg.png" },
                { "D the Lone Bumbis", "https://i.imgur.com/s6RplW6.png" },
                { "Silthar the Slithering", "https://i.imgur.com/OFk5YyT.png" },
                { "Tetheris the Skyhunter", "https://i.imgur.com/NrzAn3k.png" }
            };

            ClassUrls = new Dictionary<string, string>
            {
                { "Vagabond", "https://i.imgur.com/39DcQYR.png" },
                { "Apprentice", "https://i.imgur.com/v6TjlVR.png" },
                { "Adventurer", "https://i.imgur.com/5fwyxAe.png" },
            };

            AbilityUrls = new Dictionary<string, string>
            {
                //skills
                { "Fall Back", "https://i.imgur.com/nrPVHqL.png" },
                { "Splinter Shot", "https://i.imgur.com/fDS3kVN.png" },
                { "Arcane Bolt", "https://i.imgur.com/EiSH3Er.png" },
                { "Mind Snap", "https://i.imgur.com/kuM6Xx8.png" },
                { "First Aid", "https://i.imgur.com/85yl9Pt.png" },
                { "Parrying Strike", "https://i.imgur.com/d1yDWEw.png" },
                //weapons
                { "Whirlwind Axe", "https://i.imgur.com/enACulR.png" },
                { "Blade of the Charging Bull", "https://i.imgur.com/SZm9uNQ.png" },
                { "Sword of Steadiness", "https://i.imgur.com/eO5uPMg.png" },
                { "Bulwark Breastplate", "https://i.imgur.com/ej8vREr.png" },
                { "Chest of Faith", "https://i.imgur.com/BRDc63M.png" },
                { "Mystical Vestment", "https://i.imgur.com/a11ehRq.png" },
                { "Basic", "https://i.imgur.com/kTaaNAY.png" },
                { "Default","https://i.imgur.com/kTaaNAY.png" },
                //monsters
                { "Just Two", "https://i.imgur.com/kUWYQbN.png" },
                { "Verbal Assault", "https://i.imgur.com/V50obkV.png" },
                { "Fire Breath", "https://imgur.com/OeJprdg" },
                { "Tail Swipe", "https://i.imgur.com/6pdr5FC.png" },
                { "Constrict", "https://i.imgur.com/7YrZ4ya.png" },
                { "Venom Strike", "https://i.imgur.com/uq9HA3M.png" },
                { "Dive Bomb", "https://i.imgur.com/UpDA7JN.png" },
                { "Wing Slash", "https://i.imgur.com/BXiUh74.png" }
            };

            WeaponUrls = new Dictionary<string, string>
            {
                { "Whirlwind Axe", "https://i.imgur.com/WgUA22a.png" },
                { "Blade of the Charging Bull", "https://i.imgur.com/3CA50zy.png" },
                { "Sword of Steadiness", "https://i.imgur.com/l3QTXdp.png" }
            };

            ArmorUrls = new Dictionary<string, string>
            {
                { "Bulwark Breastplate", "https://i.imgur.com/ej8vREr.png" },
                { "Chest of Faith", "https://i.imgur.com/BRDc63M.png" },
                { "Mystical Vestment", "https://i.imgur.com/a11ehRq.png" },
            };
        }

        public static string GetMonsterPortrait(string name)
        {
            return MonsterUrls[name];
        }

        public static string GetAbilityImage(string name)
        {
            return AbilityUrls[name];
        }

        public static string GetArmorImage(string name)
        {
            return null;
        }

        public static string GetWeaponImage(string name)
        {
            return WeaponUrls[name];
        }

        public static string GetClassImage(string name)
        {
            return ClassUrls[name];
        }

        public static string GetPortalImage()
        {
            return "https://i.imgur.com/WYGDMaV.png";
        }
    }
}
