namespace O9K.Evader.Abilities.Heroes.Hoodwink.Bushwhack
{
    using Base;
    using Base.Evadable;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    [AbilityId(AbilityId.hoodwink_bushwhack)]
    internal class BushwhackBase : EvaderBaseAbility, IEvadable
    {
        public BushwhackBase(Ability9 ability)
            : base(ability)
        {
        }

        public EvadableAbility GetEvadableAbility()
        {
            return new BushwhackEvadable(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}