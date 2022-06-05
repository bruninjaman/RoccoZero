namespace O9K.Core.Entities.Abilities.Units.Creep;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Entities.Heroes;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

[AbilityId(AbilityId.creep_piercing)]
public class Piercing : PassiveAbility, IHasDamageAmplify
{
    private readonly SpecialData creepDamageAmplifierData;

    private readonly SpecialData heroDamageAmplifierData;

    private readonly SpecialData heavyDamageAmplifierData;

    public Piercing(Ability baseAbility)
        : base(baseAbility)
    {
        this.creepDamageAmplifierData = new SpecialData(baseAbility, "creep_damage_bonus");
        this.heroDamageAmplifierData = new SpecialData(baseAbility, "hero_damage_penalty");
        this.heavyDamageAmplifierData = new SpecialData(baseAbility, "heavy_damage_penalty");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Physical;

    public string[] AmplifierModifierNames { get; } = { "modifier_creep_piercing" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Outgoing;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        if (source.Name == "npc_dota_goodguys_siege")
        {
            return this.heavyDamageAmplifierData.GetValue(this.Level) / 100;
        }

        if (source is Hero9)
        {
            return this.heroDamageAmplifierData.GetValue(this.Level) / 100;
        }

        return this.creepDamageAmplifierData.GetValue(this.Level) / 100;
    }
}