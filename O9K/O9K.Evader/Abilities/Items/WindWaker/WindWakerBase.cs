namespace O9K.Evader.Abilities.Items.WindWaker
{
    using Base;
    using Base.Usable.CounterAbility;
    using Base.Usable.DisableAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_wind_waker)]
    internal class WindWakerBase : EvaderBaseAbility, IUsable<CounterAbility>, IUsable<DisableAbility>
    {
        public WindWakerBase(Ability9 ability)
            : base(ability)
        {
        }

        CounterAbility IUsable<CounterAbility>.GetUsableAbility()
        {
            return new CounterAbility(this.Ability, this.Menu);
        }

        DisableAbility IUsable<DisableAbility>.GetUsableAbility()
        {
            return new DisableAbility(this.Ability, this.Menu);
        }
    }
}