namespace O9K.Evader.Abilities.Heroes.Doom.Devour;

using Base;
using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.doom_bringer_devour)]
internal class DevourBase : EvaderBaseAbility, IUsable<CounterAbility>
{
    public DevourBase(Ability9 ability)
        : base(ability)
    {
    }

    public CounterAbility GetUsableAbility()
    {
        return new DevourUsable(this.Ability, this.Menu);
    }
}