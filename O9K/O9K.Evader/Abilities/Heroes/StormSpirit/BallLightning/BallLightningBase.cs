namespace O9K.Evader.Abilities.Heroes.StormSpirit.BallLightning
{
    using Base;
    using Base.Usable.BlinkAbility;
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    [AbilityId(AbilityId.storm_spirit_ball_lightning)]
    internal class BallLightningBase : EvaderBaseAbility, IUsable<CounterAbility>, IUsable<BlinkAbility>
    {
        public BallLightningBase(Ability9 ability)
            : base(ability)
        {
        }

        BlinkAbility IUsable<BlinkAbility>.GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }

        public CounterAbility GetUsableAbility()
        {
            return new BallLightningUsable(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}