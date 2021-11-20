namespace O9K.AIO.Heroes.Brewmaster.Units;

using System;
using System.Collections.Generic;

using AIO.Abilities;
using AIO.Modes.Combo;

using Base;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Helpers;

using Divine.Entity.Entities.Abilities.Components;

using TargetManager;

[UnitName("npc_dota_brewmaster_void_1")]
[UnitName("npc_dota_brewmaster_void_2")]
[UnitName("npc_dota_brewmaster_void_3")]
internal class Void : ControllableUnit
{
    private DebuffAbility pulse;

    public Void(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
        : base(owner, abilitySleeper, orbwalkSleeper, menu)
    {
        this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
        {
            { AbilityId.brewmaster_void_astral_pulse, x => this.pulse = new DebuffAbility(x) },
        };
    }

    public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
    {
        var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

        if (abilityHelper.UseAbility(this.pulse))
        {
            return true;
        }

        return false;
    }

    public bool AstralPulseKeyBind()
    {
        if (this.pulse == null)
        {
            return false;
        }

        var ability = this.pulse.Ability;
        if (!ability.CanBeCasted())
        {
            return false;
        }

        return ability.UseAbility();
    }
}