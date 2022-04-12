namespace O9K.Core.Entities.Abilities.Heroes.TrollWarlord;

using Base;
using Base.Components;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Entities.Units;

using Metadata;

using O9K.Core.Helpers;

[AbilityId(AbilityId.troll_warlord_battle_trance)]
public class BattleTrance : ActiveAbility, IHasDamageAmplify, IShield
{
    public BattleTrance(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "range");
    }

    public override AbilityBehavior AbilityBehavior
    {
        get
        {
            return AbilityBehavior.NoTarget; // wrong base behavior
        }
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Physical | DamageType.Magical | DamageType.Pure;

    public string[] AmplifierModifierNames { get; } = { "modifier_troll_warlord_battle_trance" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Incoming;

    public UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = false;

    public string ShieldModifierName { get; } = "modifier_troll_warlord_battle_trance";

    public bool ShieldsAlly { get; } = false;

    public bool ShieldsOwner { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        return -1;
    }
}