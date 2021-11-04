using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;

namespace O9K.Core.Entities.Abilities.NeutralItems
{
    [AbilityId(AbilityId.item_heavy_blade)]
    public class HeavyBlade : RangedAbility
    {
        public HeavyBlade(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override bool TargetsAlly { get; } = true;

        public override bool TargetsEnemy { get; } = true;
    }
}