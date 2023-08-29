

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
                { "Bragore the Wretched", "https://i.imgur.com/yurFueZ.png"},
                { "Lord Tusker", "https://i.imgur.com/RsMf6Rg.png" },
                { "D the Lone Bumbis", "https://i.imgur.com/honRnCA.png" },
                { "Silthar the Slithering", "https://i.imgur.com/iFCS1or.png" },
                { "Tetheris the Skyhunter", "https://i.imgur.com/YMWXVpp.png" }
            };

            AbilityUrls = new Dictionary<string, string>
            {
                //skills
                { "Fall Back", "https://i.imgur.com/uOf4LqT.png" },
                { "Splinter Shot", "https://i.imgur.com/wMmZe5c.png" },
                { "Arcane Bolt", "https://i.imgur.com/33IYZxm.png" },
                { "Mind Snap", "https://i.imgur.com/s2qqtn7.png" },
                { "First Aid", "https://i.imgur.com/QQcHbq4.png" },
                { "Parrying Strike", "https://i.imgur.com/UMp42D3.png" },
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
                { "Just Two", "https://i.imgur.com/h6W1kSM.png" },
                { "Verbal Assault", "https://i.imgur.com/JmINZV7.png" },
                { "Fire Breath", "https://i.imgur.com/i9Kdb8f.png" },
                { "Tail Swipe", "https://i.imgur.com/F8olm1d.png" },
                { "Constrict", "https://i.imgur.com/yoJJTR5.png" },
                { "Venom Strike", "https://i.imgur.com/Ia0051V.png" },
                { "Dive Bomb", "https://i.imgur.com/BbV6CdX.png" },
                { "Wing Slash", "https://i.imgur.com/eNT9xEn.png" }
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

        public static string GetPortalImage()
        {
            return "https://i.imgur.com/2xkuvzU.png";
        }
    }
}
