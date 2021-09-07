namespace O9K.Core.Entities.Abilities.Heroes.Tidehunter;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.tidehunter_kraken_shell)]
public class KrakenShell : PassiveAbility
{
    public KrakenShell(Ability baseAbility)
        : base(baseAbility)
    {
    }
}