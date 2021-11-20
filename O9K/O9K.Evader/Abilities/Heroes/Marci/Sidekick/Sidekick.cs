using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;
using O9K.Evader.Abilities.Base;
using O9K.Evader.Abilities.Base.Usable.CounterAbility;

namespace O9K.Evader.Abilities.Heroes.Marci.Sidekick
{
    [AbilityId(AbilityId.marci_guardian)]
    internal class SidekickBase : EvaderBaseAbility, IUsable<CounterAbility>
    {
        public SidekickBase(Ability9 ability) : base(ability)
        {
        }

        public CounterAbility GetUsableAbility()
        {
            return new CounterAbilityWithNotStableMagicImmune(this.Ability, this.Menu);
        }
    }
    
}
