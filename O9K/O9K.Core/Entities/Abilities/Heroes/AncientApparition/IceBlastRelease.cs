namespace O9K.Core.Entities.Abilities.Heroes.AncientApparition;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.ancient_apparition_ice_blast_release)]
public class IceBlastRelease : ActiveAbility
{
    public IceBlastRelease(Ability baseAbility)
        : base(baseAbility)
    {
        this.IsUltimate = false;
    }

    public override float Speed { get; } = 750;
}