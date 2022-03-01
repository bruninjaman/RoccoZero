namespace O9K.Core.Entities.Abilities.Heroes.Dazzle;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Units;

[AbilityId(AbilityId.dazzle_bad_juju)]
public class BadJuju : ActiveAbility, IBuff, IHealthRestore
{
    public BadJuju(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.DamageData = new SpecialData(baseAbility, "heal_damage");
    }

    public string BuffModifierName { get; } = "modifier_dazzle_bad_juju_armor";

    public bool BuffsAlly { get; } = true;

    public bool BuffsOwner { get; } = true;

    public bool InstantRestore { get; } = true;

    public string RestoreModifierName { get; } = string.Empty;

    public bool RestoresAlly { get; } = true;

    public bool RestoresOwner { get; } = true;

    public int GetHealthRestore(Unit9 unit)
    {
        return (int)this.DamageData.GetValue(this.Level);
    }
}