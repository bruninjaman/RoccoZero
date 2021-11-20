using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;
using O9K.Evader.Abilities.Base;
using O9K.Evader.Abilities.Base.Usable.CounterAbility;
using O9K.Evader.Abilities.Base.Usable.DisableAbility;

namespace O9K.Evader.Abilities.Items.HeaveBlade
{
    [AbilityId(AbilityId.item_heavy_blade)]
    internal class HeaveBladeBase : EvaderBaseAbility, IUsable<CounterAbility>, IUsable<DisableAbility>
    {
        public HeaveBladeBase(Ability9 ability)
            : base(ability)
        {
        }

        CounterAbility IUsable<CounterAbility>.GetUsableAbility()
        {
            return new CounterAbility(this.Ability, this.Menu);
        }

        DisableAbility IUsable<DisableAbility>.GetUsableAbility()
        {
            return new DisableAbility(this.Ability, this.Menu);
        }
    }
}