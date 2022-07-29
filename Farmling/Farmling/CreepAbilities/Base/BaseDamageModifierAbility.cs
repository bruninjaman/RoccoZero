using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units;
using Farmling.Helpers;

namespace Farmling.CreepAbilities.Base;

public abstract class BaseDamageModifierAbility
{
    private readonly Ability _baseAbility;

    protected BaseDamageModifierAbility(Ability baseAbility)
    {
        _baseAbility = baseAbility;
        Owner = (Unit?) baseAbility.Owner!;
    }

    protected Unit Owner { get; set; }

    protected SpecialData? CreepDamageAmplifierData { get; init; }

    protected SpecialData? HeroDamageAmplifierData { get; init; }

    protected SpecialData? HeavyDamageAmplifierData { get; init; }

    protected SpecialData? BonusDamageData { get; init; }

    protected SpecialData? IncomingHeroAmplifierData { get; init; }

    protected SpecialData? IncomingBasicAmplifierData { get; init; }

    protected string? Modifier { get; init; }

    protected string HeavyModifier => "modifier_creep_siege";
    protected string RangeModifier => "modifier_creep_piercing";
    protected string MeleeModifier => "modifier_creep_irresolute";

    public virtual float GetIncomingAmp(Unit target)
    {
        return 0;
    }

    public virtual float GetOutgoingAmp(Unit target)
    {
        return 0;
    }
}
