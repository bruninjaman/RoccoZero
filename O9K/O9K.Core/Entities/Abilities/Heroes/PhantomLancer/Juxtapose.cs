namespace O9K.Core.Entities.Abilities.Heroes.PhantomLancer;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.phantom_lancer_juxtapose)]
public class Juxtapose : PassiveAbility
{
    public Juxtapose(Ability baseAbility)
        : base(baseAbility)
    {
    }
}