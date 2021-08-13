namespace O9K.AIO.Heroes.ShadowFiend.Utils
{
    using System;
    using System.Collections.Generic;

    using ComboModes;

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
            { "Purple", Color.Purple }
        };

        public static void Init()
        {
            ShadowFiendBase.drawRazesSwitcher.ValueChange += OnValueChange;
            ShadowFiendBase.razeToMouseSwitcher.ValueChange += OnValueChangeRazeToMouse;
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
                    ParticleManager.RemoveParticle($"DrawRaze_{i}");
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

            if (owner.HeroId !=  HeroId.npc_dota_hero_nevermore)
            {
                return;
            }

            int[] razes = { 200, 450, 700 };

            for (var i = 0; i < 3; i++)
            {
                var inFront = InFront(owner, razes[i]);

                ParticleManager.CreateOrUpdateParticle(
                    $"DrawRaze_{i}",
                    "materials/ensage_ui/particles/alert_range.vpcf",
                    owner,
                    ParticleAttachment.AbsOrigin,
                    new ControlPoint(0, inFront),
                    new ControlPoint(1, chosen),
                    new ControlPoint(2, 250, 255, 7));
            }
        }

        private static void OnUnitOrder(OrderAddingEventArgs e)
        {
            if (ShadowFiendComboMode.IsUpdateHandlerEnabled())
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
                owner.Hero.BaseUnit.MoveToDirection(GameManager.MousePosition);
                order.Cast();
                e.Process = true;
            }
        }
    }
}