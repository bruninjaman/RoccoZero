namespace O9K.Core.Entities.Abilities.Heroes.Kunkka;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.kunkka_tidal_wave)]
public class TidalWave : RangedAbility
{
    public TidalWave(Ability baseAbility)
        : base(baseAbility)
    {
    }
}