using LeagueStatusBot.RPGEngine.Factories.ItemEffects;

namespace LeagueStatusBot.RPGEngine.Factories
{
    public class ItemEffectFactory
    {
        private readonly Dictionary<int, Func<IItemEffect>> effectMappings;

        public ItemEffectFactory()
        {
            effectMappings = new Dictionary<int, Func<IItemEffect>>
            {
                { 1, () => new ReflectiveGuard() },
                { 2, () => new DivineShield() },
                { 3, () => new FortifiedArmor() },
                { 4, () => new QuickMend() },
                { 5, () => new SturdyShielding() },
                { 6, () => new EvasiveCloak() },
                { 7, () => new AuraOfThorns() },
                { 8, () => new AegisOfTheAncients() },
                { 9, () => new RegenerativeMail() },
                { 10, () => new MysticalResistance() },
                { 11, () => new StalwartDefender() },
                { 12, () => new EnergizingGuard() },
                { 13, () => new BlessedVestments() },
                { 14, () => new MirroredPlate() },
                { 15, () => new BarrierSurge() },
                { 16, () => new GuardiansCall() },
                { 17, () => new AbsorbingPadding() },
                { 18, () => new ProtectiveRune() },
                { 19, () => new AdaptiveAlloy() },
                { 20, () => new GracefulDodge() },
                { 21, () => new HarmonicResonance() },
                { 22, () => new ElementalWard() },
                { 23, () => new SacredInfusion() },
                { 24, () => new VampiricVeil() },
                { 25, () => new ChargingBulwark() },
                { 26, () => new NaturesEmbrace() },
                { 27, () => new SpiritualBond() },
                { 28, () => new MysticalBarrier() },
                { 29, () => new GuardiansRejuvenation() },
                { 30, () => new AegisOfTheAncients() },
                { 31, () => new BladeOfEchoes() },
                { 32, () => new EnchantedPierce() },
                { 33, () => new LifeLeech() },
                { 34, () => new ElementalBurst() },
                { 35, () => new CriticalSurge() },
                { 36, () => new RapidStrikes() },
                { 37, () => new GuardBreaker() },
                { 38, () => new ChainLightning() },
                { 39, () => new HealingSlash() },
                { 40, () => new CursedBlade() },
                { 41, () => new KineticPulse() },
                { 42, () => new VampiricEdge() },
                { 43, () => new ArmorShred() },
                { 44, () => new MagicInfusion() },
                { 45, () => new QuakeHammer() },
                { 46, () => new BerserkerRage() },
                { 47, () => new SpectralArrow() },
                { 48, () => new BlazingFury() },
                { 49, () => new ChillingTouch() },
                { 50, () => new HarmonicResonance() },
                { 51, () => new PoisonedTip() },
                { 52, () => new MysticEnhancement() },
                { 53, () => new SunderingStrike() },
                { 54, () => new WindsGrace() },
                { 55, () => new GravityCrush() },
                { 56, () => new ManaSiphon() },
                { 57, () => new ThunderingWrath() },
                { 58, () => new EchoingSlam() },
                { 59, () => new ReverberatingImpact() },
                { 60, () => new BlindJustice() }


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
