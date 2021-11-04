namespace O9K.Evader.Abilities.Items.PsychicHeadband;

using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;
using O9K.Evader.Abilities.Base;
using O9K.Evader.Abilities.Base.Usable.CounterAbility;
using O9K.Evader.Abilities.Items.HurricanePike;

[AbilityId(AbilityId.item_psychic_headband)]
internal class PsychicHeadbandBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public PsychicHeadbandBase(Ability9 ability)
        : base(ability)
    {
    }

    CounterAbility IUsable<CounterAbility>.GetUsableAbility()
    {
        return new HurricanePikeUsable(this.Ability, this.Menu);
    }
}