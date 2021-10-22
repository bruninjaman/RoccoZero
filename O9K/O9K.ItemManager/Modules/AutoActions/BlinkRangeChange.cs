namespace O9K.ItemManager.Modules.AutoActions;

using System;

using Core.Entities.Abilities.Base;
using Core.Extensions;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Update;

using Metadata;

internal class BlinkRangeChange : IModule
{
    private Unit TeleportTarget;

    private Vector3 TeleportPosition;

    private float TeleportTime;

    private readonly MenuSwitcher enabled;

    public BlinkRangeChange(IMainMenu mainMenu)
    {
        var menu = mainMenu.AutoActionsMenu.Add(new Menu(LocalizationHelper.LocalizeName(AbilityId.item_blink), "BlinkDagger"));

        this.enabled = menu.Add(new MenuSwitcher("Maximize blink range"));
        this.enabled.AddTranslation(Lang.Ru, "Максимизировать дальность блинка");
        this.enabled.AddTranslation(Lang.Cn, "最大化眨眼范围");
    }

    public void Activate()
    {
        this.enabled.ValueChange += this.OnValueChange;
    }

    public void Dispose()
    {
        this.enabled.ValueChange -= this.OnValueChange;
        OrderManager.OrderAdding -= this.OnOrderAdding;
        ModifierManager.ModifierAdded -= this.OnModifierAdded;
        ParticleManager.ParticleAdded -= this.OnParticleAdded;
    }

    private void OnOrderAdding(OrderAddingEventArgs e)
    {
        try
        {
            if (e.IsCustom || !e.Process)
            {
                return;
            }

            var order = e.Order;
            if (order.IsQueued)
            {
                return;
            }

            if (order.Type != OrderType.CastPosition || order.Ability.Id != AbilityId.item_blink)
            {
                return;
            }

            var blink = (ActiveAbility)EntityManager9.GetAbility(order.Ability.Handle);
            var hero = blink.Owner;

            var heroPosition = hero.Position;

            if (hero.HasModifier("modifier_teleporting"))
            {
                heroPosition = TeleportTarget?.Position ?? TeleportPosition;
            }

            var blinkRange = blink.Range;
            var blinkPosition = order.Position;

            if (heroPosition.Distance2D(blinkPosition) < blinkRange)
            {
                return;
            }

            var newBlinkPosition = heroPosition.Extend2D(blinkPosition, blinkRange - 50);

            blink.UseAbility(newBlinkPosition);
            e.Process = false;
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    private void OnModifierAdded(ModifierAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        var modifier = e.Modifier;
        if (modifier.Ability?.Id != AbilityId.item_tpscroll && modifier.Caster != EntityManager.LocalHero)
        {
            return;
        }

        var modifierName = modifier.Name;
        if (modifierName == "modifier_teleporting")
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (!modifier.IsValid)
                {
                    return;
                }

                TeleportTarget = null;
                TeleportTime = GameManager.RawGameTime;
            });
        }
        else if (modifierName == "modifier_boots_of_travel_incoming")
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (!modifier.IsValid)
                {
                    return;
                }

                TeleportTarget = (Unit)modifier.Owner;
                TeleportTime = GameManager.RawGameTime;
            });
        }
    }

    private void OnParticleAdded(ParticleAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        var particle = e.Particle;
        if (!particle.Name.Contains("teleport_end"))
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            if (!particle.IsValid || TeleportTime != GameManager.RawGameTime)
            {
                return;
            }

            TeleportPosition = particle.GetControlPoint(0);
        });
    }

    private void OnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            OrderManager.OrderAdding += this.OnOrderAdding;
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ParticleManager.ParticleAdded += this.OnParticleAdded;
        }
        else
        {
            OrderManager.OrderAdding -= this.OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ParticleManager.ParticleAdded -= this.OnParticleAdded;
        }
    }
}