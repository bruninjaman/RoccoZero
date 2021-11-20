using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.Game;

namespace TinkerEW
{
    internal class LinkenBreaker
    {
        private Combo Combo;

        public LinkenBreaker(Combo combo)
        {
            Combo = combo;
        }

        internal void Activate()
        {
            if (Combo.Target.GetItemById(AbilityId.item_sphere)?.Cooldown == 0 || Combo.Target.HasModifier("modifier_item_sphere_target"))
            {
                switch (Combo.Menu.ComboLinkenBreakerMode.Value)
                {
                    case "Cheapest":
                        CheapestMode();
                        break;
                    case "First what can be used (not Hex)":
                        AnyExHex();
                        break;
                    case "Laser":
                        LaserMode();
                        break;
                }
            }
        }

        private void CheapestMode()
        {

            if (!Combo.UsedAbils.ContainsKey(AbilityId.tinker_laser)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Abilities.laser.CanBeCasted())
            {
                Combo.Abilities.laser.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Abilities.laser.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.tinker_laser, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_rod_of_atos)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.rodOfAtos.CanBeCasted())
            {
                Combo.Items.rodOfAtos.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.rodOfAtos.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_rod_of_atos, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_orchid)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.orchid.CanBeCasted())
            {
                Combo.Items.orchid.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.orchid.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_orchid, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_ethereal_blade)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.etherealBlade.CanBeCasted())
            {
                Combo.Items.etherealBlade.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.etherealBlade.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_ethereal_blade, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_nullifier)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.nullifier.CanBeCasted())
            {
                Combo.Items.nullifier.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.nullifier.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_nullifier, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_bloodthorn)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.bloodthorn.CanBeCasted())
            {
                Combo.Items.bloodthorn.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.bloodthorn.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_bloodthorn, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_dagon)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.dagon.CanBeCasted())
            {
                Combo.Items.dagon.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.dagon.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_dagon, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_sheepstick)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.scytheOfVyse.CanBeCasted())
            {
                Combo.Items.scytheOfVyse.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.scytheOfVyse.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_sheepstick, true);
                return;
            }
        }

        private void AnyExHex()
        {
            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_ethereal_blade)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.etherealBlade.CanBeCasted())
            {
                Combo.Items.etherealBlade.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.etherealBlade.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_ethereal_blade, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_dagon)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.dagon.CanBeCasted())
            {
                Combo.Items.dagon.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.dagon.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_dagon, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_orchid)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.orchid.CanBeCasted())
            {
                Combo.Items.orchid.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.orchid.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_orchid, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_bloodthorn)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.bloodthorn.CanBeCasted())
            {
                Combo.Items.bloodthorn.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.bloodthorn.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_bloodthorn, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_rod_of_atos)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.rodOfAtos.CanBeCasted())
            {
                Combo.Items.rodOfAtos.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.rodOfAtos.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_rod_of_atos, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.item_nullifier)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Items.nullifier.CanBeCasted())
            {
                Combo.Items.nullifier.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Items.nullifier.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.item_nullifier, true);
                return;
            }

            if (!Combo.UsedAbils.ContainsKey(AbilityId.tinker_laser)
                && !Combo.ComboSleeper.Sleeping
                && Combo.Abilities.laser.CanBeCasted())
            {
                Combo.Abilities.laser.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Abilities.laser.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                Combo.UsedAbils.Add(AbilityId.tinker_laser, true);
                return;
            }
        }

        private void LaserMode()
        {
            if (!Combo.ComboSleeper.Sleeping
                && Combo.Abilities.laser.CanBeCasted())
            {
                Combo.Abilities.laser.Cast(Combo.Target);
                Combo.ComboSleeper.Sleep((Combo.Abilities.laser.GetAbility().CastPoint) * 1000 + 80 + GameManager.AvgPing);
                return;
            }
        }
    }
}