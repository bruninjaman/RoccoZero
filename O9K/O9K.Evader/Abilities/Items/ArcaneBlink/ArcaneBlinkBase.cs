namespace O9K.Evader.Abilities.Items.ArcaneBlink
{
    using Base;
    using Base.Usable.BlinkAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_arcane_blink)]
    internal class ArcaneBlinkBase : EvaderBaseAbility, IUsable<BlinkAbility>
    {
        public ArcaneBlinkBase(Ability9 ability)
            : base(ability)
        {
        }

        public BlinkAbility GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}