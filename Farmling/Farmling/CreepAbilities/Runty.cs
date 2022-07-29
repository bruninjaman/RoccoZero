using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Farmling.CreepAbilities.Base;
using Farmling.Helpers;

namespace Farmling.CreepAbilities;

public class Runty : BaseDamageModifierAbility
{
    public Runty(Ability baseAbility) : base(baseAbility)
    {
        HeroDamageAmplifierData = new SpecialData(baseAbility, "hero_damage_penalty");
        Modifier = "modifier_creep_irresolute";
    }

    public override float GetIncomingAmp(Unit target)
    {
        if (target is Hero) return HeroDamageAmplifierData!.GetValue(1) / 100;

        return 0;
    }
}
