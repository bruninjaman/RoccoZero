namespace O9K.Core.Entities.Abilities.Heroes.TreantProtector;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.treant_eyes_in_the_forest)]
public class EyesInTheForest : RangedAbility
{
    public EyesInTheForest(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override bool TargetsEnemy { get; } = false;
}