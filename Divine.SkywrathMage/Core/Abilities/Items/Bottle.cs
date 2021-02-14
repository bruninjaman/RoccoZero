using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bottle)]
    public sealed class Bottle : RangedItem, IHasTargetModifier, IHasHealthRestore, IHasManaRestore
    {
        public Bottle(Item item)
            : base(item)
        {
            Base = item as Ensage.Items.Bottle;
        }

        public new Ensage.Items.Bottle Base { get; }

        public override bool CanBeCasted
        {
            get
            {
                return CurrentCharges > 0 && StoredRune == RuneType.None && base.CanBeCasted;
            }
        }

        public override float Duration
        {
            get
            {
                return GetAbilitySpecialData("restore_time");
            }
        }

        public RuneType StoredRune
        {
            get
            {
                return Base.StoredRune;
            }
        }

        public string TargetModifierName { get; } = "modifier_bottle_regeneration";

        public float TotalHealthRestore
        {
            get
            {
                return GetAbilitySpecialData("health_restore") * Duration;
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("mana_restore") * Duration;
            }
        }
    }
}