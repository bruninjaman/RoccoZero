namespace O9K.Core.Entities.Abilities.Heroes.WitchDoctor;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.witch_doctor_voodoo_switcheroo)]
public class VoodooSwitcheroo : ActiveAbility, IChanneled
{
    public VoodooSwitcheroo(Ability baseAbility)
        : base(baseAbility)
    {
        this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
    }

    public float ChannelTime { get; }

    public bool IsActivatesOnChannelStart { get; } = true;
}