using LeagueStatusBot.RPGEngine.Factories.ItemEffects.Adventurer;
using LeagueStatusBot.RPGEngine.Factories.ArmorEffects;
using LeagueStatusBot.RPGEngine.Factories.ArmorEffects.Adventurer;

namespace LeagueStatusBot.RPGEngine.Factories
{
    public class ArmorEffectFactory
    {
        private readonly Dictionary<int, Func<IArmorEffect>> effectMappings;

        public ArmorEffectFactory()
        {
            effectMappings = new Dictionary<int, Func<IArmorEffect>>
            {
                { 1, () => new ArmorDefault() },
                { 2, () => new BulwarkBreastplate() },
                { 3, () => new ChestOfFaith() },
                { 4, () => new MysticalVestment() },
            };
        }

        public IArmorEffect GetEffect(int effectId)
        {
            if (effectMappings.TryGetValue(effectId, out var effectConstructor))
            {
                return effectConstructor();
            }

            return null;
        }
    }
}
