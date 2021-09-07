namespace O9K.Core.Entities.Abilities.Heroes.NagaSiren;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.naga_siren_song_of_the_siren_cancel)]
public class SongOfTheSirenEnd : ActiveAbility
{
    public SongOfTheSirenEnd(Ability baseAbility)
        : base(baseAbility)
    {
    }
}