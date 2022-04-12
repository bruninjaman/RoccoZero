namespace O9K.AIO.Heroes.ShadowFiend.Utils;

using System;
using System.Collections.Generic;

using Core.Managers.Entity;
using Core.Managers.Menu.EventArgs;

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

    public static void Init(TargetManager targetManager)
    {
        ShadowFiendBase.drawRazesSwitcher.ValueChange += OnValueChange;
        ShadowFiendBase.razeToMouseSwitcher.ValueChange += OnValueChangeRazeToMouse;
        OrderManager.OrderAdding += OnUnitOrderRazeToTarget;
        RazeUtils.targetManager = targetManager;
    }

    private static void OnUnitOrderRazeToTarget(OrderAddingEventArgs e)
    {
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
        }
        else
        {
            OrderManager.OrderAdding -= OnUnitOrder;
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
        var chosen = Colours[ShadowFiendBase.colourSelector];
        var owner = EntityManager9.Owner;

        if (owner.HeroId != HeroId.npc_dota_hero_nevermore)
        {
            return;
        }

        int[] razes = { 200, 450, 700 };

        for (var i = 0; i < 3; i++)
        {
            var inFront = InFront(owner, razes[i]);

            ParticleManager.CreateParticle(
                $"DrawRaze_{i}",
                "materials/ensage_ui/particles/alert_range.vpcf",
                Attachment.AbsOrigin,
                owner,
                new ControlPoint(0, inFront),
                new ControlPoint(1, chosen),
                new ControlPoint(2, 250, 255, 7));
        }
    }

    private static void OnUnitOrder(OrderAddingEventArgs e)
    {

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