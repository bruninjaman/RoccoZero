﻿namespace O9K.Core.Entities.Abilities.Items;

using Base;
using Base.Components;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

using Helpers;

using Metadata;

//todo id
[AbilityId((AbilityId)273)]
public class KayaAndSange : PassiveAbility, IHasDamageAmplify
{
    private readonly SpecialData amplifierData;

    public KayaAndSange(Ability baseAbility)
        : base(baseAbility)
    {
        this.amplifierData = new SpecialData(baseAbility, "spell_amp");
    }

    public DamageType AmplifierDamageType { get; } = DamageType.Magical;

    public string[] AmplifierModifierNames { get; } = { "modifier_item_kaya_and_sange" };

    public AmplifiesDamage AmplifiesDamage { get; } = AmplifiesDamage.Outgoing;

    public bool IsAmplifierAddedToStats { get; } = false;

    public bool IsAmplifierPermanent { get; } = true;

    public float AmplifierValue(Unit9 source, Unit9 target)
    {
        if (!this.IsUsable)
        {
            return 0;
        }

        return this.amplifierData.GetValue(this.Level) / 100f;
    }
}