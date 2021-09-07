namespace O9K.Core.Entities.Abilities.Heroes.Visage.Familiar;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.visage_summon_familiars_stone_form)]
public class StoneForm : AreaOfEffectAbility, IDisable
{
    public StoneForm(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "stun_radius");
        this.ActivationDelayData = new SpecialData(baseAbility, "stun_delay");
        this.DamageData = new SpecialData(baseAbility, "stun_damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Stunned;
}