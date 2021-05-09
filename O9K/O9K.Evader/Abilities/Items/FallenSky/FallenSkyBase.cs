namespace O9K.Evader.Abilities.Items.BlinkDagger
{
    using Base;
    using Base.Usable.BlinkAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    using O9K.Evader.Abilities.Base.Usable.DisableAbility;

    [AbilityId(AbilityId.item_fallen_sky)]
    internal class FallenSkyBase : EvaderBaseAbility, IUsable<BlinkAbility>, IUsable<DisableAbility>
    {
        public FallenSkyBase(Ability9 ability)
            : base(ability)
        {
        }

        BlinkAbility IUsable<BlinkAbility>.GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }

        DisableAbility IUsable<DisableAbility>.GetUsableAbility()
        {
            return new DisableAbility(this.Ability, this.Menu);
        }
    }
}