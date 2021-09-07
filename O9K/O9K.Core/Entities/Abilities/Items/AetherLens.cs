﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;
using Helpers.Range;

using Metadata;

[AbilityId(AbilityId.item_aether_lens)]
public class AetherLens : PassiveAbility, IHasRangeIncrease
{
    private readonly SpecialData castRange;

    public AetherLens(Ability baseAbility)
        : base(baseAbility)
    {
        this.castRange = new SpecialData(baseAbility, "cast_range_bonus");
    }

    public bool IsRangeIncreasePermanent { get; } = true;

    public RangeIncreaseType RangeIncreaseType { get; } = RangeIncreaseType.Ability;

    public string RangeModifierName { get; } = "modifier_item_aether_lens";

    public float GetRangeIncrease(Unit9 unit, RangeIncreaseType type)
    {
        if (!this.IsUsable)
        {
            return 0;
        }

        return this.castRange.GetValue(this.Level);
    }
}