namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

using O9K.Core.Entities.Units;

[AbilityId(AbilityId.item_boots_of_bearing)]
public class BootsOfBearing : AreaOfEffectAbility, ISpeedBuff
{
    private readonly SpecialData bonusMoveSpeedData;

    public BootsOfBearing(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.bonusMoveSpeedData = new SpecialData(baseAbility, "bonus_movement_speed_pct");
    }

    public string BuffModifierName { get; } = "modifier_item_boots_of_bearing_active";

    public bool BuffsAlly { get; } = true;

    public bool BuffsOwner { get; } = true;

    public override bool CanBeCasted(bool checkChanneling = true)
    {
        return this.Charges > 0 && base.CanBeCasted(checkChanneling);
    }

    public float GetSpeedBuff(Unit9 unit)
    {
        return (unit.Speed * this.bonusMoveSpeedData.GetValue(this.Level)) / 100;
    }
}