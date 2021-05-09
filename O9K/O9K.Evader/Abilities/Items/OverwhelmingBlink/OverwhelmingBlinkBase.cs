namespace O9K.Evader.Abilities.Items.OverwhelmingBlink
{
    using Base;
    using Base.Usable.BlinkAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    [AbilityId(AbilityId.item_overwhelming_blink)]
    internal class OverwhelmingBlinkBase : EvaderBaseAbility, IUsable<BlinkAbility>
    {
        public OverwhelmingBlinkBase(Ability9 ability)
            : base(ability)
        {
        }

        public BlinkAbility GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }
    }
}