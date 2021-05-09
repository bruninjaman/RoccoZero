namespace O9K.Evader.Abilities.Items.Bullwhip
{
    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine;

    using O9K.Evader.Abilities.Base.Usable.DodgeAbility;

    [AbilityId(AbilityId.item_bullwhip)]
    internal class BullwhipBase : EvaderBaseAbility, IUsable<DodgeAbility>
    {
        public BullwhipBase(Ability9 ability)
            : base(ability)
        {
        }

        public DodgeAbility GetUsableAbility()
        {
            return new DodgeAbility(this.Ability, this.Menu);
        }
    }
}