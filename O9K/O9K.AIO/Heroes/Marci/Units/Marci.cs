namespace O9K.AIO.Heroes.Marci.Units;

using System;
using System.Collections.Generic;
using System.Linq;

using Abilities;

using AIO.Abilities;
using AIO.Abilities.Items;
using AIO.Modes.Combo;

using Base;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Helpers;
using Core.Managers.Entity;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Order;

using O9K.AIO.Heroes.Pangolier.Abilities;

using TargetManager;

[UnitName(nameof(HeroId.npc_dota_hero_marci))]
internal class Marci : ControllableUnit
{
    private BlinkAbility blink;

    private DisableAbility bloodthorn;

    private NukeAbility dagon;

    private EtherealBlade ethereal;

    private DisableAbility orchid;

    private SpeedBuffAbility phase;

    private DebuffAbility veil;

    private DisableAbility dispose;

    private Rebound rebound;

    private BuffAbility sidekick;

    private BuffAbility unleash;

    private ReboundBlink moveRebound;

    private DisableAbility abyssal;

    private ShieldAbility mjollnir;

    public Marci(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
        : base(owner, abilitySleeper, orbwalkSleeper, menu)
    {
        this.ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
        {
            { AbilityId.marci_grapple, x => this.dispose = new DisableAbility(x) },
            { AbilityId.marci_companion_run, x => this.rebound = new Rebound(x) },
            { AbilityId.marci_guardian, x => this.sidekick = new BuffAbility(x) },
            { AbilityId.marci_unleash, x => this.unleash = new BuffAbility(x) },

            { AbilityId.item_abyssal_blade, x => this.abyssal = new DisableAbility(x) },
            { AbilityId.item_phase_boots, x => this.phase = new SpeedBuffAbility(x) },
            { AbilityId.item_mjollnir, x => this.mjollnir = new ShieldAbility(x) },
            { AbilityId.item_ethereal_blade, x => this.ethereal = new EtherealBlade(x) },
            { AbilityId.item_dagon_5, x => this.dagon = new NukeAbility(x) },
            { AbilityId.item_blink, x => this.blink = new BlinkAbility(x) },
            { AbilityId.item_swift_blink, x => this.blink = new BlinkAbility(x) },
            { AbilityId.item_arcane_blink, x => this.blink = new BlinkAbility(x) },
            { AbilityId.item_overwhelming_blink, x => this.blink = new BlinkAbility(x) },
            { AbilityId.item_orchid, x => this.orchid = new DisableAbility(x) },
            { AbilityId.item_veil_of_discord, x => this.veil = new DebuffAbility(x) },
            { AbilityId.item_bloodthorn, x => this.bloodthorn = new Bloodthorn(x) },
        };

        this.MoveComboAbilities.Add(AbilityId.marci_companion_run, x => this.moveRebound = new ReboundBlink(x));
    }

    public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
    {
        var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);

        if (abilityHelper.UseAbility(this.blink, 400, 0))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.abyssal))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.veil))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.bloodthorn))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.orchid))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.rebound))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.ethereal))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.dagon))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.dispose))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.mjollnir))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.sidekick))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.unleash))
        {
            return true;
        }

        if (abilityHelper.UseAbility(this.phase))
        {
            return true;
        }

        return false;
    }

    //public void Toss()
    //{
    //    var tossAbility = this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.tiny_toss) as ActiveAbility;
    //    if (tossAbility?.CanBeCasted() != true)
    //    {
    //        return;
    //    }

    //    var tower = EntityManager9.Units.Where(x => x.IsTower && x.IsAlly(this.Owner) && x.IsAlive)
    //        .OrderBy(x => x.Distance(this.Owner))
    //        .FirstOrDefault(x => x.Distance(this.Owner) < 2000);
    //    if (tower == null)
    //    {
    //        return;
    //    }

    //    var tossTarget = EntityManager9.Units
    //        .Where(
    //            x => x.IsUnit && !x.IsInvulnerable && !x.IsMagicImmune && x.IsAlive && x.IsVisible
    //                 && x.Distance(this.Owner) < tossAbility.CastRange && x.Distance(tower) < tower.GetAttackRange())
    //        .OrderBy(x => x.Distance(tower))
    //        .FirstOrDefault();
    //    if (tossTarget == null)
    //    {
    //        return;
    //    }

    //    var grabUnit = EntityManager9.Units
    //        .Where(
    //            x => x.IsUnit && !x.Equals(this.Owner) && !x.IsInvulnerable && !x.IsMagicImmune && x.IsAlive && x.IsVisible
    //                 && x.Distance(this.Owner) < tossAbility.Radius)
    //        .OrderBy(x => x.Distance(this.Owner))
    //        .FirstOrDefault();

    //    if (grabUnit?.IsHero != true || grabUnit.IsIllusion || grabUnit.IsAlly(this.Owner))
    //    {
    //        return;
    //    }

    //    tossAbility.UseAbility(tossTarget);
    //}

    //protected override bool MoveComboUseDisables(AbilityHelper abilityHelper)
    //{
    //    if (base.MoveComboUseDisables(abilityHelper))
    //    {
    //        return true;
    //    }

    //    if (abilityHelper.UseMoveAbility(this.avalanche))
    //    {
    //        return true;
    //    }

    //    return false;
    //}
}