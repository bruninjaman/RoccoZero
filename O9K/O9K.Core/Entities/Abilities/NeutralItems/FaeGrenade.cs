using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;

namespace O9K.Core.Entities.Abilities.NeutralItems
{
    [AbilityId(AbilityId.item_paintball)]
    public class FaeGrenade : RangedAbility, IDebuff
    {
        public FaeGrenade(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public string DebuffModifierName { get; } = "modifier_item_paintball_debuff";
    }
}