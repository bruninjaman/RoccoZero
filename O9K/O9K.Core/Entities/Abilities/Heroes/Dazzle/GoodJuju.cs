namespace O9K.Core.Entities.Abilities.Heroes.Dazzle;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.dazzle_good_juju)]
public class GoodJuju : RangedAbility
{
    public GoodJuju(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override AbilityBehavior AbilityBehavior
    {
        get
        {
            var behavior = base.AbilityBehavior;

            if (this.Owner.HasAghanimsScepter)
            {
                behavior = (behavior & ~AbilityBehavior.Passive) | AbilityBehavior.UnitTarget;
            }

            return behavior;
        }
    }
}