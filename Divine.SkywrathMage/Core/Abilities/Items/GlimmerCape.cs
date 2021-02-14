﻿using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;



namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_glimmer_cape)]
    public sealed class GlimmerCape : RangedItem, IHasTargetModifier, IHasModifierTexture
    {
        public GlimmerCape(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invisible;

        public string[] ModifierTextureName { get; } = { "item_glimmer_cape" };

        public string TargetModifierName { get; } = "modifier_item_glimmer_cape_fade";
    }
}