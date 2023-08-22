

using System.Collections.Generic;

namespace LeagueStatusBot.Helpers
{
    public static class UrlGetter
    {
        private static Dictionary<string, string> MonsterUrls;
        private static Dictionary<string, string> AbilityUrls;
        private static Dictionary<string, string> WeaponUrls;
        private static Dictionary<string, string> ArmorUrls;

        public static void Initialize()
        {
            MonsterUrls = new Dictionary<string, string>
            {
                { "Bragore the Wretched", "https://i.imgur.com/zfP1ABN.png"},
                { "Lord Tusker", "https://i.imgur.com/RsMf6Rg.png" },
                { "D the Lone Bumbis", "https://i.imgur.com/Y0u6RHn.png" }
            };

            AbilityUrls = new Dictionary<string, string>
            {
                { "First Aid", "https://i.imgur.com/QQcHbq4.png" },
                { "Parrying Strike", "https://i.imgur.com/UMp42D3.png" },
                { "Whirlwind Axe", "https://i.imgur.com/enACulR.png" },
                { "Blade of the Charging Bull", "https://i.imgur.com/SZm9uNQ.png" },
                { "Sword of Steadiness", "https://i.imgur.com/eO5uPMg.png" },
                { "Bulwark Breastplate", "https://i.imgur.com/ej8vREr.png" },
                { "Chest of Faith", "https://i.imgur.com/BRDc63M.png" },
                { "Mystical Vestment", "https://i.imgur.com/a11ehRq.png" },
                { "Basic", "https://i.imgur.com/kTaaNAY.png" }
            };

            WeaponUrls = new Dictionary<string, string>
            {
                { "Whirlwind Axe", "https://i.imgur.com/WgUA22a.png" },
                { "Blade of the Charging Bull", "https://i.imgur.com/3CA50zy.png" },
                { "Sword of Steadiness", "https://i.imgur.com/l3QTXdp.png" }
            };

            ArmorUrls = new Dictionary<string, string>
            {
                
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

        public static string GetPortalImage()
        {
            return "https://i.imgur.com/2xkuvzU.png";
        }
    }
}
