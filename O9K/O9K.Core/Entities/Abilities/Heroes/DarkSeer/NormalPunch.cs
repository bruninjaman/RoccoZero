namespace O9K.Core.Entities.Abilities.Heroes.DarkSeer;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.dark_seer_normal_punch)]
public class NormalPunch : PassiveAbility
{
    public NormalPunch(Ability baseAbility)
        : base(baseAbility)
    {
    }
}