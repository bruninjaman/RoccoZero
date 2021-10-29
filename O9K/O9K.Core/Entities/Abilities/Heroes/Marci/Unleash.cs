namespace O9K.Core.Entities.Abilities.Heroes.Marci;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using O9K.Core.Helpers;

[AbilityId(AbilityId.marci_unleash)]
public class Unleash : ActiveAbility, IBuff
{
    public Unleash(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public string BuffModifierName { get; } = "modifier_marci_unleash";

    public bool BuffsAlly { get; } = false;

    public bool BuffsOwner { get; } = true;
}