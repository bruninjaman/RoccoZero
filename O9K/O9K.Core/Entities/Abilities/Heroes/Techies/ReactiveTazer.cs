namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Helpers;

[AbilityId(AbilityId.techies_reactive_tazer)]
public class ReactiveTazer : ActiveAbility, IShield
{
    public ReactiveTazer(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "stun_radius");
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
}