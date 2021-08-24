namespace O9K.Evader.Abilities.Heroes.Hoodwink.SharpShooter
{
    using Base;
    using Base.Evadable;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    [AbilityId(AbilityId.hoodwink_sharpshooter)]
    internal class SharpShooterBase : EvaderBaseAbility, IEvadable
    {
        public SharpShooterBase(Ability9 ability)
            : base(ability)
        {
        }

        public EvadableAbility GetEvadableAbility()
        {
            return new SharpShooterEvadable(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}