namespace O9K.AIO.Heroes.Dynamic.Abilities.Specials.Unique
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Logger;

    using Divine;

    [AbilityId(AbilityId.item_hurricane_pike)]
    internal class HurricanePikeSpecial : OldSpecialAbility, IDisposable
    {
        private readonly List<AbilityId> enabledOrbs = new List<AbilityId>();

        public HurricanePikeSpecial(IActiveAbility ability)
            : base(ability)
        {
        }

        public void Dispose()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        public override bool ShouldCast(Unit9 target)
        {
            if (target.IsLinkensProtected || target.IsSpellShieldProtected)
            {
                return false;
            }

            if (this.Ability.Owner.Distance(target) < target.GetAttackRange() + 100)
            {
                return true;
            }

            return false;
        }

        public override bool Use(Unit9 target)
        {
            if (!this.Ability.UseAbility(target))
            {
                return false;
            }

            ModifierManager.ModifierAdded += this.OnModifierAdded;

            this.OrbwalkSleeper.Sleep(this.Ability.Owner.Handle, this.Ability.GetCastDelay(target));
            this.AbilitySleeper.Sleep(this.Ability.Handle, this.Ability.GetHitTime(target) + 0.5f);

            return true;
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_item_hurricane_pike_range")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Ability.Owner.Handle)
                    {
                        return;
                    }

                    foreach (var orb in this.Ability.Owner.Abilities.OfType<OrbAbility>())
                    {
                        if (orb.Enabled || !orb.CanBeCasted())
                        {
                            continue;
                        }

                        this.enabledOrbs.Add(orb.Id);
                        orb.Enabled = true;
                    }

                    ModifierManager.ModifierRemoved += this.OnModifierRemoved;
                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                }
            });
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_item_hurricane_pike_range")
            {
                return;
            }

            try
            {
                if (modifier.Owner.Handle != this.Ability.Owner.Handle)
                {
                    return;
                }

                foreach (var orb in this.Ability.Owner.Abilities.OfType<OrbAbility>().Where(x => this.enabledOrbs.Contains(x.Id)))
                {
                    orb.Enabled = false;
                }

                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                Logger.Error(ex);
            }
        }
    }
}