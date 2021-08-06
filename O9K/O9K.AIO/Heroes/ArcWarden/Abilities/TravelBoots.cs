namespace O9K.AIO.Heroes.ArcWarden.Abilities
{
    using AIO.Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Helpers;

    using TargetManager;

    internal class TravelBoots : UsableAbility
    {
        public TravelBoots(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool ForceUseAbility(TargetManager targetManager, Sleeper comboSleeper)
        {
            throw new System.NotImplementedException();
        }

        public override bool ShouldCast(TargetManager targetManager)
        {
            throw new System.NotImplementedException();
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            throw new System.NotImplementedException();
        }

    }
}