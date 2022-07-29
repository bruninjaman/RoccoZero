using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Farmling.CreepAbilities.Base;
using Farmling.Helpers;

namespace Farmling.CreepAbilities;

public class Reinforced : BaseDamageModifierAbility
{
    public Reinforced(Ability baseAbility) : base(baseAbility)
    {
        BonusDamageData = new SpecialData(baseAbility, "bonus_building_damage");
        IncomingHeroAmplifierData = new SpecialData(baseAbility, "incoming_hero_damage_penalty");
        IncomingBasicAmplifierData = new SpecialData(baseAbility, "incoming_basic_damage_penalty");
        Modifier = "modifier_creep_siege";
    }

    public override float GetIncomingAmp(Unit target)
    {
        if (target.HasModifier(Modifier!)) return BonusDamageData!.GetValue(1) / 100;
        return 0;
    }

    public override float GetOutgoingAmp(Unit target)
    {
        if (target is Hero) return IncomingHeroAmplifierData!.GetValue(1) / 100;

        return IncomingBasicAmplifierData!.GetValue(1) / 100;

        return 0;
    }
}
