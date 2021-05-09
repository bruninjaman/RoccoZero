namespace O9K.Evader.Abilities.Items.SwiftBlink
{
    using Base;
    using Base.Usable.BlinkAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_swift_blink)]
    internal class SwiftBlinkBase : EvaderBaseAbility, IUsable<BlinkAbility>
    {
        public SwiftBlinkBase(Ability9 ability)
            : base(ability)
        {
        }

        public BlinkAbility GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}