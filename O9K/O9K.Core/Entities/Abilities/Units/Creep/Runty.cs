namespace O9K.Core.Entities.Abilities.Units.Creep;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Entities.Heroes;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

[AbilityId(AbilityId.creep_irresolute)]
public class Runty : PassiveAbility, IHasDamageAmplify
{
    private readonly SpecialData heroDamageAmplifierData;

    public Runty(Ability baseAbility)
        : base(baseAbility)
    {
        this.heroDamageAmplifierData = new SpecialData(baseAbility, "hero_damage_penalty");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Physical;

    public string[] AmplifierModifierNames { get; } = { "modifier_creep_irresolute" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Outgoing;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        if (source is Hero9)
        {
            return this.heroDamageAmplifierData.GetValue(this.Level) / 100;
        }

        return 0f;
    }
}