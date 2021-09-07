﻿namespace O9K.Core.Entities.Abilities.Talents;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;

using Metadata;

[AbilityId(AbilityId.special_bonus_spell_amplify_3)]
[AbilityId(AbilityId.special_bonus_spell_amplify_4)]
[AbilityId(AbilityId.special_bonus_spell_amplify_5)]
[AbilityId(AbilityId.special_bonus_spell_amplify_6)]
[AbilityId(AbilityId.special_bonus_spell_amplify_7)]
[AbilityId(AbilityId.special_bonus_spell_amplify_8)]
[AbilityId(AbilityId.special_bonus_spell_amplify_9)]
[AbilityId(AbilityId.special_bonus_spell_amplify_10)]
[AbilityId(AbilityId.special_bonus_spell_amplify_11)]
[AbilityId(AbilityId.special_bonus_spell_amplify_12)]
[AbilityId(AbilityId.special_bonus_spell_amplify_14)]
[AbilityId(AbilityId.special_bonus_spell_amplify_15)]
[AbilityId(AbilityId.special_bonus_spell_amplify_16)]
[AbilityId(AbilityId.special_bonus_spell_amplify_18)]
[AbilityId(AbilityId.special_bonus_spell_amplify_20)]
[AbilityId(AbilityId.special_bonus_spell_amplify_25)]
public class SpellAmplifyTalent : Talent, IHasDamageAmplify
{
    private readonly SpecialData amplify;

    public SpellAmplifyTalent(Ability baseAbility)
        : base(baseAbility)
    {
        this.amplify = new SpecialData(baseAbility, "value");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Magical;

    public string[] AmplifierModifierNames { get; } = { "modifier_special_bonus_spell_amplify" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Outgoing;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        return this.amplify.GetValue(this.Level) / 100;
    }
}