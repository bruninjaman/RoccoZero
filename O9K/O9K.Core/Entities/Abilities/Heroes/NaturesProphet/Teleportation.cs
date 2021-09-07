namespace O9K.Core.Entities.Abilities.Heroes.NaturesProphet;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.furion_teleportation)]
public class Teleportation : RangedAbility
{
    public Teleportation(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override float CastRange { get; } = 9999999;
}