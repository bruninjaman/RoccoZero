namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;

    [AbilityId(AbilityId.item_book_of_shadows)]
    public class BookOfShadows : RangedAbility, IDisable, IShield, IDebuff
    {
        public BookOfShadows(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public UnitState AppliesUnitState { get; } = UnitState.Silenced | UnitState.Muted | UnitState.Disarmed | UnitState.AttackImmune | UnitState.Untargetable;

        public string ShieldModifierName { get; } = "modifier_item_book_of_shadows_buff";

        public bool ShieldsAlly { get; } = true;

        public bool ShieldsOwner { get; } = true;

        public string DebuffModifierName { get; } = "modifier_item_book_of_shadows_buff";
    }
}