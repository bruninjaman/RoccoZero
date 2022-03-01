namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

[AbilityId(AbilityId.techies_reactive_tazer)]
public class ReactiveTazer : ActiveAbility, IShield, ISpeedBuff
{
    private readonly SpecialData bonusMoveSpeedData;

    public ReactiveTazer(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "stun_radius");
        this.bonusMoveSpeedData = new SpecialData(baseAbility, "bonus_ms");
    }

    public override AbilityBehavior AbilityBehavior
    {
        get
        {
            var behavior = base.AbilityBehavior;

            if (this.Owner.HasAghanimsScepter)
            {
                behavior = (behavior & ~AbilityBehavior.NoTarget) | AbilityBehavior.UnitTarget;
            }

            return behavior;
        }
    }

    public UnitState AppliesUnitState { get; } = UnitState.AttackImmune;

    public string ShieldModifierName { get; } = "techies_reactive_tazer";

    public bool ShieldsAlly { get; } = false;

    public bool ShieldsOwner { get; } = true;

    public string BuffModifierName { get; } = "techies_reactive_tazer";

    public bool BuffsAlly
    {
        get
        {
            return this.Owner.HasAghanimsScepter;
        }
    }

    public bool BuffsOwner { get; } = true;

    public float GetSpeedBuff(Unit9 unit)
    {
        return (unit.Speed * this.bonusMoveSpeedData.GetValue(this.Level)) / 100;
    }
}