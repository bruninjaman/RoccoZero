namespace Divine.Core.ComboFactory.AutoItems;

using System;
using System.Linq;
using System.Threading.Tasks;

using Divine.Core.ComboFactory.Menus.AutoItems;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Items;
using Divine.Core.Extensions;
using Divine.Core.Managers;
using Divine.Core.Managers.Unit;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;

internal sealed class AutoPhaseBoots
{
    public MenuSwitcher EnableItem { get; }

    public MenuSlider DistanceCheckItem { get; }

    private readonly CHero owner = UnitManager.Owner;

    public AutoPhaseBoots(AutoPhaseBootsMenu autoPhaseBootsMenu)
    {
        EnableItem = autoPhaseBootsMenu.EnableItem;
        DistanceCheckItem = autoPhaseBootsMenu.DistanceCheckItem;

        EnableItem.ValueChanged += EnableChanged;
    }

    private PhaseBoots phaseBoots;

    private PhaseBoots PhaseBoots
    {
        get
        {
            return phaseBoots;
        }

        set
        {
            if (phaseBoots == value)
            {
                return;
            }

            if (value != null)
            {
                OrderManager.OrderAdding += OnOrderAdding;
            }
            else
            {
                OrderManager.OrderAdding -= OnOrderAdding;
            }

            phaseBoots = value;
        }
    }

    public void Dispose()
    {
        EnableItem.ValueChanged -= EnableChanged;

        if (EnableItem)
        {
            ActionManager.ActionRemove<PhaseBoots>();
            PhaseBoots = null;
        }
    }

    private void EnableChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        if (e.Value)
        {
            ActionManager.ActionAdd<PhaseBoots>(x => PhaseBoots = x);
        }
        else
        {
            ActionManager.ActionRemove<PhaseBoots>();
            PhaseBoots = null;
        }
    }

    private async void OnOrderAdding(OrderAddingEventArgs e)
    {
        var order = e.Order;
        if (!order.Units.Contains(owner.Base) || order.IsQueued || !e.Process)
        {
            return;
        }

        await Task.Delay(80);

        if (!PhaseBoots.CanBeCasted || !owner.IsAlive || owner.IsChanneling() || !owner.CanUseAbilitiesInInvisibility && owner.IsUnsafeInvisible || owner.IsInvisible())
        {
            return;
        }

        switch (order.Type)
        {
            case OrderType.AttackPosition:
            case OrderType.AttackTarget:
                {
                    var position = order.Target?.Position ?? order.Position;
                    if (Math.Max(owner.Distance2D(position) - owner.AttackRange(), 0) >= DistanceCheckItem.Value)
                    {
                        PhaseBoots.UseAbility();
                    }

                    break;
                }
            case OrderType.MoveTarget:
            case OrderType.MovePosition:
                {
                    var position = order.Target?.Position ?? order.Position;
                    if (owner.Distance2D(position) >= DistanceCheckItem.Value)
                    {
                        PhaseBoots.UseAbility();
                    }

                    break;
                }
        }
    }
}
