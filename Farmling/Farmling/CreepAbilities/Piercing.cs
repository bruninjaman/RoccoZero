using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Farmling.CreepAbilities.Base;
using Farmling.Helpers;

namespace Farmling.CreepAbilities;

public class Piercing : BaseDamageModifierAbility
{
    public Piercing(Ability baseAbility) : base(baseAbility)
    {
        CreepDamageAmplifierData = new SpecialData(baseAbility, "creep_damage_bonus");
        HeroDamageAmplifierData = new SpecialData(baseAbility, "hero_damage_penalty");
        HeavyDamageAmplifierData = new SpecialData(baseAbility, "heavy_damage_penalty");
        Modifier = "modifier_creep_piercing";
    }

    public override float GetIncomingAmp(Unit target)
    {
        if (target.HasModifier(HeavyModifier!)) return HeavyDamageAmplifierData!.GetValue(1) / 100;

        if (target is Hero) return HeroDamageAmplifierData!.GetValue(1) / 100;

        return CreepDamageAmplifierData!.GetValue(1) / 100;
    }
}
