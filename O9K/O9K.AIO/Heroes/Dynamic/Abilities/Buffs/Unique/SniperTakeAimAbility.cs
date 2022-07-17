using Divine.Entity.Entities.Abilities.Components;
using O9K.AIO.Heroes.Dynamic.Abilities.Blinks;
using O9K.AIO.Modes.Combo;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Entities.Units;

namespace O9K.AIO.Heroes.Dynamic.Abilities.Buffs.Unique;

[AbilityId(AbilityId.sniper_take_aim)]
internal class SniperTakeAimAbility : OldBuffAbility
{
    public SniperTakeAimAbility(IBuff ability) : base(ability)
    {
    }

    protected override bool ShouldCastBuff(Unit9 target, BlinkAbilityGroup blinks, ComboModeMenu menu)
    {
        var distance = this.Ability.Owner.Distance(target);
        var attackRange = this.Ability.Owner.GetAttackRange(target);

        if (distance > attackRange - 250)
        {
            return false;
        }

        return true;
    }
}
