using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;


using Ensage.SDK.Extensions;
using Ensage.SDK.Menu.ValueBinding;

namespace Divine.SkywrathMage.Features
{
    internal sealed class Disable : BaseTaskHandler
    {
        private BaseDisableMenu DisableMenu { get; }

        private Abilities Abilities { get; }

        public Disable(Common common)
        {
            DisableMenu = common.MenuConfig.DisableMenu;

            Abilities = (Abilities)common.Abilities;

            if (DisableMenu.EnableItem)
            {
                RunAsync();
            }

            DisableMenu.EnableItem.Changed += EnableChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DisableMenu.EnableItem.Changed -= EnableChanged;
        }

        private void EnableChanged(object sender, ValueChangingEventArgs<bool> e)
        {
            if (e.Value)
            {
                RunAsync();
            }
            else
            {
                Cancel();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (IsStopped || Owner.IsInvisible())
                {
                    return;
                }

                var target = UnitManager<CHero, Enemy, NoIllusion>.Units.FirstOrDefault(x => x.IsVisible && x.IsAlive && IsInAbilityPhase(x));  //TODO Target

                if (IsNullTarget(target))
                {
                    return;
                }

                // Hex
                var hex = Abilities.Hex;
                if (hex != null
                    && DisableMenu.AbilitiesSelection.PictureStates[hex.Name]
                    && hex.CanBeCasted
                    && hex.CanHit(target))
                {
                    hex.Cast(target);
                    await Task.Delay(hex.GetCastDelay(target), token);
                    return;
                }

                // Orchid
                var orchid = Abilities.Orchid;
                if (orchid != null
                    && DisableMenu.AbilitiesSelection.PictureStates[orchid.Name]
                    && orchid.CanBeCasted
                    && orchid.CanHit(target))
                {
                    orchid.Cast(target);
                    await Task.Delay(orchid.GetCastDelay(target), token);
                    return;
                }

                // Bloodthorn
                var bloodthorn = Abilities.Bloodthorn;
                if (bloodthorn != null
                    && DisableMenu.AbilitiesSelection.PictureStates[bloodthorn.Name]
                    && bloodthorn.CanBeCasted
                    && bloodthorn.CanHit(target))
                {
                    bloodthorn.Cast(target);
                    await Task.Delay(bloodthorn.GetCastDelay(target), token);
                    return;
                }

                // AncientSeal
                var ancientSeal = Abilities.AncientSeal;
                if (DisableMenu.AbilitiesSelection.PictureStates[ancientSeal.Name]
                    && ancientSeal.CanBeCasted
                    && ancientSeal.CanHit(target))
                {
                    ancientSeal.Cast(target);
                    await Task.Delay(ancientSeal.GetCastDelay(target), token);
                    return;
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        private bool IsInAbilityPhase(CHero target)
        {
            if (target.Spells.Any(x => DisableAbilities.Contains(x.Id) && x.IsInAbilityPhase))
            {
                return true;
            }

            return false;
        }

        private static AbilityId[] DisableAbilities { get; } =
        {
            AbilityId.queenofpain_blink,
            AbilityId.antimage_blink,
            AbilityId.antimage_mana_void,
            AbilityId.legion_commander_duel,
            AbilityId.doom_bringer_doom,
            AbilityId.faceless_void_time_walk,
            AbilityId.faceless_void_chronosphere,
            AbilityId.witch_doctor_death_ward,
            AbilityId.rattletrap_power_cogs,
            AbilityId.tidehunter_ravage,
            AbilityId.axe_berserkers_call,
            AbilityId.brewmaster_primal_split,
            AbilityId.omniknight_guardian_angel,
            AbilityId.queenofpain_sonic_wave,
            AbilityId.slardar_slithereen_crush,
            AbilityId.lion_finger_of_death,
            AbilityId.lina_laguna_blade
        };
    }
}
