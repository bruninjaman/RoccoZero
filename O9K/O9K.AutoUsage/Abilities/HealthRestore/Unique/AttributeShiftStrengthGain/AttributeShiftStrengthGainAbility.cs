namespace O9K.AutoUsage.Abilities.HealthRestore.Unique.AttributeShiftStrengthGain
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Abilities.Base.Types;
    using Core.Entities.Abilities.Heroes.Morphling;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;

    using Divine;

    using Settings;

    [AbilityId(AbilityId.morphling_morph_str)]
    internal class AttributeShiftStrengthGainAbility : HealthRestoreAbility
    {
        private readonly AttributeShiftStrengthGainSettings settings;

        private readonly AttributeShiftAgilityGain shiftAgi;

        private readonly AttributeShiftStrengthGain shiftStr;

        private readonly Sleeper sleeper = new Sleeper();

        private float balanceHealth;

        private bool manualToggle;

        public AttributeShiftStrengthGainAbility(IHealthRestore healthRestore, GroupSettings settings)
            : base(healthRestore)
        {
            this.settings = new AttributeShiftStrengthGainSettings(settings.Menu, healthRestore);
            this.shiftStr = (AttributeShiftStrengthGain)healthRestore;
            this.shiftAgi = this.shiftStr.AttributeShiftAgilityGain;

            this.balanceHealth = this.Owner.Health;
        }

        public override void Enabled(bool enabled)
        {
            base.Enabled(enabled);

            if (enabled)
            {
                OrderManager.OrderAdding += this.OnOrderAdding;
                ModifierManager.ModifierAdded += this.OnModifierAdded;
                UpdateManager.CreateIngameUpdate(100, this.OnUpdate);
            }
            else
            {
                OrderManager.OrderAdding -= this.OnOrderAdding;
                ModifierManager.ModifierAdded -= this.OnModifierAdded;
                UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            }
        }

        public override bool UseAbility(List<Unit9> heroes)
        {
            return false;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (!e.Process || e.IsCustom)
                {
                    return;
                }

                var order = e.Order;
                if (order.Type != OrderType.CastToggle)
                {
                    return;
                }

                if (order.Ability.Id != this.shiftStr.Id && order.Ability.Id != this.shiftAgi.Id)
                {
                    return;
                }

                if (!order.Ability.IsToggled)
                {
                    this.manualToggle = true;
                    return;
                }

                var delay = (int)GameManager.Ping + 100;
                UpdateManager.BeginInvoke(delay, () =>
                {
                    this.balanceHealth = this.Owner.Health;
                    this.manualToggle = false;
                });

                this.sleeper.Sleep(delay / 1000f);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_pudge_meat_hook")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (!modifier.IsValid || modifier.Owner.Handle != this.Owner.Handle)
                    {
                        return;
                    }

                    this.sleeper.Sleep(2f);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private void OnUpdate()
        {
            try
            {
                if (this.manualToggle || this.sleeper.IsSleeping || !this.settings.AutoBalance)
                {
                    return;
                }

                if (!this.shiftStr.CanBeCasted())
                {
                    return;
                }

                if (!this.Owner.CanBeHealed)
                {
                    if (this.shiftAgi.Enabled)
                    {
                        this.shiftAgi.Enabled = false;
                        this.sleeper.Sleep(0.1f);
                    }
                    else if (this.shiftStr.Enabled)
                    {
                        this.shiftAgi.Enabled = false;
                        this.sleeper.Sleep(0.1f);
                    }

                    return;
                }

                var health = this.Owner.Health;

                if (health > this.balanceHealth + 150)
                {
                    if (!this.shiftAgi.Enabled)
                    {
                        this.shiftAgi.Enabled = true;
                        this.sleeper.Sleep(0.1f);
                    }
                }
                else if (health < this.balanceHealth)
                {
                    if (!this.shiftStr.Enabled)
                    {
                        this.shiftStr.Enabled = true;
                        this.sleeper.Sleep(0.1f);
                    }
                }
                else
                {
                    if (this.shiftAgi.Enabled)
                    {
                        this.shiftAgi.Enabled = false;
                        this.sleeper.Sleep(0.1f);
                    }
                    else if (this.shiftStr.Enabled)
                    {
                        this.shiftStr.Enabled = false;
                        this.sleeper.Sleep(0.1f);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}