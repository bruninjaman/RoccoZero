namespace O9K.Evader.Abilities.Items.EulsScepterOfDivinity;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.item_stormcrafter)]
internal class StormcrafterBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public StormcrafterBase(Ability9 ability)
        : base(ability)
    {
    }

    CounterAbility IUsable<CounterAbility>.GetUsableAbility()
    {
        return new CounterAbility(this.Ability, this.Menu);
    }
}