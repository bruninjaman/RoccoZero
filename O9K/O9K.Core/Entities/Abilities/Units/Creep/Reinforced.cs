namespace O9K.Core.Entities.Abilities.Units.Creep;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Components;
using O9K.Core.Entities.Heroes;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;

[AbilityId(AbilityId.creep_siege)]
public class Reinforced : PassiveAbility, IHasDamageAmplify
{
    private readonly SpecialData bonusDamageData;

    private readonly SpecialData incomingHeroAmplifierData;

    private readonly SpecialData incomingBasicAmplifierData;

    public Reinforced(Ability baseAbility)
        : base(baseAbility)
    {
        this.bonusDamageData = new SpecialData(baseAbility, "bonus_building_damage");
        this.incomingHeroAmplifierData = new SpecialData(baseAbility, "incoming_hero_damage_penalty");
        this.incomingBasicAmplifierData = new SpecialData(baseAbility, "incoming_basic_damage_penalty");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Physical;

    public string[] AmplifierModifierNames { get; } = { "modifier_creep_siege" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.All;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        return AmplifierValue(source, target, AmplifiesDamage);
    }

    public float AmplifierValue(Unit9 source, Unit9 target, AmplifiesDamage amplifiesDamage)
    {
        if ((amplifiesDamage & AmplifiesDamage.Incoming) != 0)
        {
            if (source.HasModifier(AmplifierModifierNames) && target.HasModifier(AmplifierModifierNames))
            {
                return 0;
            }

            if (source is Hero9)
            {
                return this.incomingHeroAmplifierData.GetValue(this.Level) / 100;
            }

            return this.incomingBasicAmplifierData.GetValue(this.Level) / 100;
        }
        else if (source.HasModifier(AmplifierModifierNames))
        {
            return this.bonusDamageData.GetValue(this.Level) / 100;
        }

        return 0;
    }
}