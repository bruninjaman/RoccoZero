using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Metadata;
using O9K.Evader.Abilities.Base;
using O9K.Evader.Abilities.Base.Usable.DisableAbility;

namespace O9K.Evader.Abilities.Heroes.Sniper.Shrapnel;

[AbilityId(AbilityId.sniper_concussive_grenade)]
internal class ConcussiveGrenadeBase : EvaderBaseAbility, IUsable<DisableAbility>
{
    public ConcussiveGrenadeBase(Ability9 ability)
        : base(ability)
    {
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }
}
