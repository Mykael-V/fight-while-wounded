using System;
using System.Reflection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.Roster;

namespace FightWhileWounded
{
    public sealed class SubModule : MBSubModuleBase
    {
        protected override void OnGameStart(Game game, IGameStarter starter)
        {
            if (!(game.GameType is Campaign)) return;
            // Generic registration initializes BaseModel with the previously registered model.
            starter.AddModel<CharacterStatsModel>(new PlayerWoundedStatsModel());
            ((CampaignGameStarter)starter).AddBehavior(new RosterRefreshBehavior());
        }
    }

    public sealed class PlayerWoundedStatsModel : CharacterStatsModel
    {
        public override int WoundedHitPointLimit(Hero hero)
        {
            if (hero != null && ReferenceEquals(hero, Hero.MainHero)) return 0;
            return BaseModel.WoundedHitPointLimit(hero);
        }
        public override int MaxCharacterTier { get { return BaseModel.MaxCharacterTier; } }
        public override int GetTier(CharacterObject character) { return BaseModel.GetTier(character); }
        public override ExplainedNumber MaxHitpoints(CharacterObject character, bool includeDescriptions = false)
        {
            return BaseModel.MaxHitpoints(character, includeDescriptions);
        }
    }

    public sealed class RosterRefreshBehavior : CampaignBehaviorBase
    {
        public override void RegisterEvents()
        {
            CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener(this, Refresh);
        }
        public override void SyncData(IDataStore dataStore) { }

        private void Refresh(CampaignGameStarter starter)
        {
            Hero hero = Hero.MainHero;
            if (hero == null) return;
            PlayerWoundedStatsModel active = Campaign.Current.Models.CharacterStatsModel as PlayerWoundedStatsModel;
            if (active == null)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Fight While Wounded: outro mod substituiu o modelo. Coloque este mod por ultimo.", Colors.Red));
                return;
            }
            if (hero.PartyBelongedTo != null) RefreshRoster(hero.PartyBelongedTo.MemberRoster, hero);
            if (hero.PartyBelongedToAsPrisoner != null) RefreshRoster(hero.PartyBelongedToAsPrisoner.PrisonRoster, hero);
            InformationManager.DisplayMessage(new InformationMessage(
                "Fight While Wounded ativo: protagonista pode combater com 1 HP ou mais."));
        }

        private static void RefreshRoster(TroopRoster roster, Hero hero)
        {
            if (roster.FindIndexOfTroop(hero.CharacterObject) < 0) return;
            // Private in 1.4.8. Recompute, rather than applying a wound-count delta twice.
            typeof(TroopRoster).GetMethod("InitializeCachedData", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(roster, null);
            roster.UpdateVersion();
        }
    }
}
