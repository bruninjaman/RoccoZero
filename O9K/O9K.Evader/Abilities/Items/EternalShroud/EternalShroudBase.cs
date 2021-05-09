namespace O9K.Evader.Abilities.Items.EternalShroud
{
    using Base;
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_eternal_shroud)]
    internal class EternalShroudBase : EvaderBaseAbility, IUsable<CounterAbility>
    {
        public EternalShroudBase(Ability9 ability)
            : base(ability)
        {
        }

        public CounterAbility GetUsableAbility()
        {
            return new CounterAbility(this.Ability, this.Menu);
        }
    }
}