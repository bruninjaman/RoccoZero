namespace O9K.AIO.Heroes.Techies.Units;

using System;
using System.Collections.Generic;
using Abilities;
using AIO.Abilities;
using AIO.Abilities.Items;
using Base;
using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Modes.Combo;
using TargetManager;

[UnitName(nameof(HeroId.npc_dota_hero_techies))]
internal class Techies : ControllableUnit
{
    private DisableAbility hex;
    private EtherealBlade ethereal;
    private NukeAbility stickyBomb;
    private Suicide suicide;
    private BuffAbility reactiveTazer;
    private ProximityMinesAbility landMinesAbility;
    private DebuffAbility veil;
    private NukeAbility dagon;
    private ShieldAbility bkb;
    private DebuffAbility shiva;

    public Techies(Unit9 owner, MultiSleeper abilitySleeper, Sleeper orbwalkSleeper, ControllableUnitMenu menu)
        : base(owner, abilitySleeper, orbwalkSleeper, menu)
    {
        ComboAbilities = new Dictionary<AbilityId, Func<ActiveAbility, UsableAbility>>
        {
            {AbilityId.techies_sticky_bomb, x => stickyBomb = new NukeAbility(x)},
            {AbilityId.techies_suicide, x => suicide = new Suicide(x)},
            {AbilityId.techies_reactive_tazer, x => reactiveTazer = new BuffAbility(x)},
            {AbilityId.techies_land_mines, x => landMinesAbility = new ProximityMinesAbility(x)},

            {AbilityId.item_sheepstick, x => hex = new DisableAbility(x)},
            {AbilityId.item_ethereal_blade, x => ethereal = new EtherealBlade(x)},
            {AbilityId.item_veil_of_discord, x => veil = new DebuffAbility(x)},
            {AbilityId.item_dagon_5, x => dagon = new NukeAbility(x)},
            {AbilityId.item_black_king_bar, x => bkb = new ShieldAbility(x)},
            {AbilityId.item_shivas_guard, x => shiva = new DebuffAbility(x)},
        };
    }

    public override bool Combo(TargetManager targetManager, ComboModeMenu comboModeMenu)
    {
        var abilityHelper = new AbilityHelper(targetManager, comboModeMenu, this);
        var target = targetManager.Target;
        if ((target.CanBecomeMagicImmune || target.CanBecomeInvisible) && abilityHelper.UseAbility(hex))
        {
            return true;
        }

        if (abilityHelper.UseAbility(suicide))
        {
            return true;
        }

        if (!Owner.HasAghanimShard)
        {
            if (abilityHelper.UseAbility(ethereal))
            {
                return true;
            }
        }

        if (abilityHelper.UseAbility(landMinesAbility))
        {
            return true;
        }

        if (abilityHelper.UseAbility(hex))
        {
            return true;
        }

        if (abilityHelper.UseAbility(stickyBomb))
        {
            return true;
        }

        if (abilityHelper.UseAbility(veil))
        {
            return true;
        }

        if (abilityHelper.UseAbility(ethereal))
        {
            return true;
        }

        if (abilityHelper.UseAbility(dagon))
        {
            return true;
        }

        if (abilityHelper.UseAbility(shiva))
        {
            return true;
        }

        if (abilityHelper.UseAbility(reactiveTazer))
        {
            return true;
        }

        if (abilityHelper.UseAbility(bkb))
        {
            return true;
        }

        return false;
    }
}