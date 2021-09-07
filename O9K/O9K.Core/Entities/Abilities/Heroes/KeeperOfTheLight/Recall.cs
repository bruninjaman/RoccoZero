namespace O9K.Core.Entities.Abilities.Heroes.KeeperOfTheLight;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.keeper_of_the_light_recall)]
public class Recall : RangedAbility
{
    public Recall(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override float CastRange { get; } = 9999999;
}