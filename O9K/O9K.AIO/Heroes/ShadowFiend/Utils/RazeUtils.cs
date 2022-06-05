namespace O9K.AIO.Heroes.ShadowFiend.Utils;

using System;
using System.Collections.Generic;
using System.Linq;

using Core.Entities.Abilities.Base;
using Core.Managers.Entity;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Order;
using Divine.Order.EventArgs;
using Divine.Order.Orders.Components;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Renderer;

using Menu;

using TargetManager;

internal class RazeUtils
{
    public static readonly Dictionary<string, Color> Colours = new()
    {
        { "White", Color.White },
        { "Red", Color.Red },
        { "Green", Color.Green },
        { "Blue", Color.Blue },
        { "Yellow", Color.Yellow },
        { "Pink", Color.Pink },
        { "Purple", Color.Purple },
    };

    private static TargetManager targetManager;

    private static MenuSwitcher Enabled;

    private static AbilityId[] razes = { AbilityId.nevermore_shadowraze1,  AbilityId.nevermore_shadowraze2,  AbilityId.nevermore_shadowraze3, };

    private static List<Ability9> StaticRazes
    {
        get
        {
            return EntityManager9.Owner.Hero.Abilities.Where(ability9 => ability9.Id.In(razes)).OrderBy(x=> x.Name).ToList();
        }
    }

    public static void Init(TargetManager targetManager, MenuManager menu)
    {
        Enabled = menu.Enabled;
        ShadowFiendBase.drawRazesSwitcher.ValueChange += OnValueChange;
        ShadowFiendBase.razeToMouseSwitcher.ValueChange += OnValueChangeRazeToMouse;
        RazeUtils.targetManager = targetManager;
    }

    private static void OnUnitOrderRazeToTarget(OrderAddingEventArgs e)
    {
        if (!Enabled)
        {
            return;
        }

        if (!e.IsCustom)
        {
            return;
        }

        var order = e.Order;
        if (order.Type != OrderType.Cast)
        {
            return;
        }

        var ability = order.Ability;
        if (ability == null)
        {
            return;
        }

        var owner = EntityManager9.Owner;
        if (ability.Owner != owner)
        {
            return;
        }

        if (ability.Id is not AbilityId.nevermore_shadowraze1 and not AbilityId.nevermore_shadowraze2 and not AbilityId.nevermore_shadowraze3)
        {
            return;
        }

        var predictedPosition = targetManager.Target.GetPredictedPosition(ability.GetCastPoint());

        var additionalDelay = owner.Hero.GetTurnTime(predictedPosition);

        var targetPredictedPositionWithDelay =
            targetManager.Target.GetPredictedPosition(ability.GetCastPoint() + additionalDelay);

        if (owner.Hero.GetAngle(targetPredictedPositionWithDelay) > 0.2)
        {
            owner.Hero.MoveToDirection(targetPredictedPositionWithDelay);
            e.Process = true;
        }
        else
        {
            e.Process = true;
        }
    }

    private static void OnValueChangeRazeToMouse(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            OrderManager.OrderAdding += OnUnitOrder;
            OrderManager.OrderAdding += OnUnitOrderRazeToTarget;
        }
        else
        {
            OrderManager.OrderAdding -= OnUnitOrder;
            OrderManager.OrderAdding -= OnUnitOrderRazeToTarget;
        }
    }

    private static void OnValueChange(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += OnDraw;
        }
        else
        {
            RendererManager.Draw -= OnDraw;

            for (var i = 0; i < 3; i++)
            {
                ParticleManager.DestroyParticle($"DrawRaze_{i}");
            }
        }
    }

    private static Vector2 FromPolarCoordinates(float radial, float polar)
    {
        return new((float)Math.Cos(polar) * radial, (float)Math.Sin(polar) * radial);
    }

    internal void Dispose()
    {
        //TODO
    }

    public static Vector3 InFront(Unit unit, float distance)
    {
        var alpha = unit.RotationRad;
        var vector2FromPolarAngle = FromPolarCoordinates(1f, alpha);

        var v = unit.Position + vector2FromPolarAngle.ToVector3() * distance;

        return new Vector3(v.X, v.Y, 0);
    }

    private static void OnDraw()
    {
        if (!Enabled)
        {
            return;
        }

        var owner = EntityManager9.Owner;

        if (owner.HeroId != HeroId.npc_dota_hero_nevermore)
        {
            return;
        }

        int[] razesRanges = { 200, 450, 700 };

        for (var i = 0; i < 3; i++)
        {
            var chosenColour = Colours[ShadowFiendBase.colourSelector];
            if (StaticRazes[i]?.IsReady == false)
            {
                chosenColour = Colours[ShadowFiendBase.colourAfterUsedSelector];
            }
            
            var inFront = InFront(owner, razesRanges[i]);
            ParticleManager.CreateParticle(
                $"DrawRaze_{i}",
                "materials/ensage_ui/particles/alert_range.vpcf",
                Attachment.AbsOrigin,
                owner,
                new ControlPoint(0, inFront),
                new ControlPoint(1, chosenColour),
                new ControlPoint(2, 250, 255, 7));
        }
    }

    private static void OnUnitOrder(OrderAddingEventArgs e)
    {
        if (!Enabled)
        {
            return;
        }

        var owner = EntityManager9.Owner;
        var order = e.Order.Ability;

        if (owner.HeroId != HeroId.npc_dota_hero_nevermore || e.IsCustom)
        {
            return;
        }

        if (order is not null && e.Order.Type == OrderType.Cast && (order.Id == AbilityId.nevermore_shadowraze1
                                                                    || order.Id == AbilityId.nevermore_shadowraze2
                                                                    || order.Id == AbilityId.nevermore_shadowraze3))
        {
            if (owner.Hero.GetAngle(GameManager.MousePosition) > 0.2)
            {
                owner.Hero.MoveToDirection(GameManager.MousePosition);
                e.Process = true;

            }
            else
            {
                e.Process = true;
            }
        }
    }
}