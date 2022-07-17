using Divine.Entity.Entities.Abilities.Components;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;

namespace O9K.AIO.Heroes.Dynamic.Abilities.Disables.Unique;

[AbilityId(AbilityId.sniper_concussive_grenade)]
internal class ConcussiveGrenadeDisable : OldDisableAbility
{
    public ConcussiveGrenadeDisable(IDisable ability) : base(ability)
    {
    }
}
