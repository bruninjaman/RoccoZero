namespace O9K.Evader.Abilities.Items.TricksterCloak
{
    using Base;
    using Base.Evadable;
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_trickster_cloak)]
    internal class TricksterCloakBase : EvaderBaseAbility, IEvadable, IUsable<CounterAbility>
    {
        public TricksterCloakBase(Ability9 ability)
            : base(ability)
        {
        }

        public EvadableAbility GetEvadableAbility()
        {
            return new TricksterCloakEvadable(this.Ability, this.Pathfinder, this.Menu);
        }

        CounterAbility IUsable<CounterAbility>.GetUsableAbility()
        {
            return new CounterInvisibilityAbility(this.Ability, this.Menu);
        }
    }
}