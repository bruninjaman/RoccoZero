using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;
using O9K.Evader.Abilities.Base;
using O9K.Evader.Abilities.Base.Evadable;
using O9K.Evader.Abilities.Base.Usable.CounterAbility;
using O9K.Evader.Abilities.Items.BladeMail;

namespace O9K.Evader.Abilities.Items.ArcanistsArmor
{
    [AbilityId(AbilityId.item_force_field)]
    internal class ArcanistsArmorBase : EvaderBaseAbility, IEvadable, IUsable<CounterAbility>
    {
        public ArcanistsArmorBase(Ability9 ability)
            : base(ability)
        {
        }

        public EvadableAbility GetEvadableAbility()
        {
            return new BladeMailEvadable(this.Ability, this.Pathfinder, this.Menu);
        }

        public CounterAbility GetUsableAbility()
        {
            return new BladeMailUsable(this.Ability, this.Menu);
        }
    }
}