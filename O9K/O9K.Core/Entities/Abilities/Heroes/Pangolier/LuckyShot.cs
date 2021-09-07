namespace O9K.Core.Entities.Abilities.Heroes.Pangolier;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.pangolier_lucky_shot)]
public class LuckyShot : PassiveAbility
{
    public LuckyShot(Ability baseAbility)
        : base(baseAbility)
    {
    }
}