using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueStatusBot.Common.Models
{
    public class MatchResult
    {
        public bool MatchWinLose { get; set; }
        public int TotalDamageDealtToChampions { get; set; }
        public int PlayerIcon { get; set; }
        public int Item0 { get; set; }
        public int Item1 { get; set; }
        public int Item2 { get; set; }
        public int Item3 { get; set; }
        public int Item4 { get; set; }
        public int Item5 { get; set; }
        public int Item6 { get; set; }
        public string GameMode { get; set; }
        public string MatchType { get; set; }
        public string ChampName { get; set; }
        public string PlayerName { get; set; }
        public string TeamPosition { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public double KDA { get; set; }
        public int WardsPlaced { get; set; }
        public int TotalPings { get; set; }
        public int EnemyMissingPings { get; set; }
        public Challenges Challenges { get; set; }
    }

    public class Challenges
    {
        public int _12AssistStreakCount { get; set; }
        public int AbilityUses { get; set; }
        public int AcesBefore15Minutes { get; set; }
        public int AlliedJungleMonsterKills { get; set; }
        public int BaronBuffGoldAdvantageOverThreshold { get; set; }
        public int BaronTakedowns { get; set; }
        public int BlastConeOppositeOpponentCount { get; set; }
        public int BountyGold { get; set; }
        public int BuffsStolen { get; set; }
        public int CompleteSupportQuestInTime { get; set; }
        public int ControlWardsPlaced { get; set; }
        public double DamagePerMinute { get; set; }
        public double DamageTakenOnTeamPercentage { get; set; }
        public int DancedWithRiftHerald { get; set; }
        public int DeathsByEnemyChamps { get; set; }
        public int DodgeSkillShotsSmallWindow { get; set; }
        public int DoubleAces { get; set; }
        public int DragonTakedowns { get; set; }
        public double EarliestBaron { get; set; }
        public double EarliestDragonTakedown { get; set; }
        public int EarlyLaningPhaseGoldExpAdvantage { get; set; }
        public double EffectiveHealAndShielding { get; set; }
        public int ElderDragonKillsWithOpposingSoul { get; set; }
        public int ElderDragonMultikills { get; set; }
        public int EnemyChampionImmobilizations { get; set; }
        public int EnemyJungleMonsterKills { get; set; }
        public int EpicMonsterKillsNearEnemyJungler { get; set; }
        public int EpicMonsterKillsWithin30SecondsOfSpawn { get; set; }
        public int EpicMonsterSteals { get; set; }
        public int EpicMonsterStolenWithoutSmite { get; set; }
        public int FirstTurretKilled { get; set; }
        public int FlawlessAces { get; set; }
        public int FullTeamTakedown { get; set; }
        public double GameLength { get; set; }
        public int GetTakedownsInAllLanesEarlyJungleAsLaner { get; set; }
        public double GoldPerMinute { get; set; }
        public int HadOpenNexus { get; set; }
        public int HighestWardKills { get; set; }
        public int ImmobilizeAndKillWithAlly { get; set; }
        public int InitialBuffCount { get; set; }
        public int InitialCrabCount { get; set; }
        public double JungleCsBefore10Minutes { get; set; }
        public int JunglerTakedownsNearDamagedEpicMonster { get; set; }
        public int KTurretsDestroyedBeforePlatesFall { get; set; }
        public double Kda { get; set; }
        public int KillAfterHiddenWithAlly { get; set; }
        public double KillParticipation { get; set; }
        public int KilledChampTookFullTeamDamageSurvived { get; set; }
        public int KillingSprees { get; set; }
        public int KillsNearEnemyTurret { get; set; }
        public int KillsOnOtherLanesEarlyJungleAsLaner { get; set; }
        public int KillsOnRecentlyHealedByAramPack { get; set; }
        public int KillsUnderOwnTurret { get; set; }
        public int KillsWithHelpFromEpicMonster { get; set; }
        public int KnockEnemyIntoTeamAndKill { get; set; }
        public int LandSkillShotsEarlyGame { get; set; }
        public int LaneMinionsFirst10Minutes { get; set; }
        public int LaningPhaseGoldExpAdvantage { get; set; }
        public int LegendaryCount { get; set; }
        public int LostAnInhibitor { get; set; }
        public double MaxCsAdvantageOnLaneOpponent { get; set; }
        public int MaxKillDeficit { get; set; }
        public int MaxLevelLeadLaneOpponent { get; set; }
        public int MejaisFullStackInTime { get; set; }
        public double MoreEnemyJungleThanOpponent { get; set; }
        public int MultiKillOneSpell { get; set; }
        public int MultiTurretRiftHeraldCount { get; set; }
        public int Multikills { get; set; }
        public int MultikillsAfterAggressiveFlash { get; set; }
        public int MythicItemUsed { get; set; }
        public int OuterTurretExecutesBefore10Minutes { get; set; }
        public int OutnumberedKills { get; set; }
        public int OutnumberedNexusKill { get; set; }
        public int PerfectDragonSoulsTaken { get; set; }
        public int PerfectGame { get; set; }
        public int PickKillWithAlly { get; set; }
        public int PlayedChampSelectPosition { get; set; }
        public int PoroExplosions { get; set; }
        public int QuickCleanse { get; set; }
        public int QuickFirstTurret { get; set; }
        public int QuickSoloKills { get; set; }
        public int RiftHeraldTakedowns { get; set; }
        public int SaveAllyFromDeath { get; set; }
        public int ScuttleCrabKills { get; set; }
        public double ShortestTimeToAceFromFirstTakedown { get; set; }
        public int SkillshotsDodged { get; set; }
        public int SkillshotsHit { get; set; }
        public int SnowballsHit { get; set; }
        public int SoloBaronKills { get; set; }
        public int SoloKills { get; set; }
        public int StealthWardsPlaced { get; set; }
        public int SurvivedSingleDigitHpCount { get; set; }
        public int SurvivedThreeImmobilizesInFight { get; set; }
        public int TakedownOnFirstTurret { get; set; }
        public int Takedowns { get; set; }
        public int TakedownsAfterGainingLevelAdvantage { get; set; }
        public int TakedownsBeforeJungleMinionSpawn { get; set; }
        public int TakedownsFirstXMinutes { get; set; }
        public int TakedownsInAlcove { get; set; }
        public int TakedownsInEnemyFountain { get; set; }
        public int TeamBaronKills { get; set; }
        public double TeamDamagePercentage { get; set; }
        public int TeamElderDragonKills { get; set; }
        public int TeamRiftHeraldKills { get; set; }
        public int TookLargeDamageSurvived { get; set; }
        public int TurretPlatesTaken { get; set; }
        public int TurretTakedowns { get; set; }
        public int TurretsTakenWithRiftHerald { get; set; }
        public int TwentyMinionsIn3SecondsCount { get; set; }
        public int TwoWardsOneSweeperCount { get; set; }
        public int UnseenRecalls { get; set; }
        public double VisionScoreAdvantageLaneOpponent { get; set; }
        public double VisionScorePerMinute { get; set; }
        public int WardTakedowns { get; set; }
        public int WardTakedownsBefore20M { get; set; }
        public int WardsGuarded { get; set; }
        public int? FasterSupportQuestCompletion { get; set; }
        public int? HighestCrowdControlScore { get; set; }
        public double? FastestLegendary { get; set; }
        public int? JunglerKillsEarlyJungle { get; set; }
        public int? KillsOnLanersEarlyJungleAsJungler { get; set; }
        public double? FirstTurretKilledTime { get; set; }
        public int? HighestChampionDamage { get; set; }
        public double? ControlWardTimeCoverageInRiverOrEnemyHalf { get; set; }
    }
}
