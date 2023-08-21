using LeagueStatusBot.RPGEngine.Factories.ItemEffects;
using LeagueStatusBot.RPGEngine.Factories.ItemEffects.Adventurer;

namespace LeagueStatusBot.RPGEngine.Factories
{
    public class ItemEffectFactory
    {
        private readonly Dictionary<int, Func<IItemEffect>> effectMappings;

        public ItemEffectFactory()
        {
            effectMappings = new Dictionary<int, Func<IItemEffect>>
            {
                { 1, () => new Default() },
                { 2, () => new ChargingBullBlade() },
                { 3, () => new SwordOfSteadiness() },
                { 4, () => new WhirlwindAxe() },
            };
        }

        public IItemEffect GetEffect(int effectId)
        {
            if (effectMappings.TryGetValue(effectId, out var effectConstructor))
            {
                return effectConstructor();
            }

            return null;
        }
    }
}
