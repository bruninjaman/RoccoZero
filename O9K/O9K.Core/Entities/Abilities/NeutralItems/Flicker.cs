namespace O9K.Core.Entities.Abilities.NeutralItems;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.item_flicker)]
public class Flicker : ActiveAbility, IBlink
{
    public Flicker(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public BlinkType BlinkType { get; } = BlinkType.Blink;
}