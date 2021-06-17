namespace O9K.AIO.Heroes.Disruptor.Modes
{
    using System.Collections.Generic;
    using System.Linq;

    using AIO.Modes.Permanent;

    using Base;

    using Core.Entities.Units;

    using Divine;
    using Divine.Camera;
    using Divine.Entity;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.GameConsole;

    using Divine.Input;
    using Divine.Log;
    using Divine.Map;

    using Divine.Modifier;
    using Divine.Numerics;
    using Divine.Orbwalker;
    using Divine.Order;
    using Divine.Particle;
    using Divine.Projectile;
    using Divine.Renderer;
    using Divine.Service;
    using Divine.Update;
    using Divine.Entity.Entities;
    using Divine.Entity.EventArgs;
    using Divine.Game.EventArgs;
    using Divine.GameConsole.Exceptions;
    using Divine.Input.EventArgs;
    using Divine.Map.Components;
    using Divine.Menu.Animations;
    using Divine.Menu.Components;

    using Divine.Menu.Helpers;

    using Divine.Menu.Styles;
    using Divine.Modifier.EventArgs;
    using Divine.Modifier.Modifiers;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders;
    using Divine.Particle.Components;
    using Divine.Particle.EventArgs;
    using Divine.Particle.Particles;
    using Divine.Plugins.Humanizer;
    using Divine.Projectile.EventArgs;
    using Divine.Projectile.Projectiles;
    using Divine.Renderer.ValveTexture;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Exceptions;
    using Divine.Entity.Entities.PhysicalItems;
    using Divine.Entity.Entities.Players;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Modifier.Modifiers.Components;
    using Divine.Modifier.Modifiers.Exceptions;
    using Divine.Order.Orders.Components;
    using Divine.Particle.Particles.Exceptions;
    using Divine.Projectile.Projectiles.Components;
    using Divine.Projectile.Projectiles.Exceptions;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Abilities.Spells;
    using Divine.Entity.Entities.Players.Components;
    using Divine.Entity.Entities.Runes.Components;
    using Divine.Entity.Entities.Units.Buildings;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Wards;
    using Divine.Entity.Entities.Abilities.Items.Components;
    using Divine.Entity.Entities.Abilities.Items.Neutrals;
    using Divine.Entity.Entities.Abilities.Spells.Abaddon;
    using Divine.Entity.Entities.Abilities.Spells.Components;
    using Divine.Entity.Entities.Units.Creeps.Neutrals;
    using Divine.Entity.Entities.Units.Heroes.Components;

    using Divine.Numerics;

    internal class GlimpseTrackerMode : PermanentMode
    {
        private readonly Vector3 color = new Vector3(Color.Blue.R, Color.Blue.G, Color.Blue.B);

        private readonly Dictionary<Unit9, Dictionary<float, Vector3>> positions = new Dictionary<Unit9, Dictionary<float, Vector3>>();

        private Particle targetParticleEffect;

        public GlimpseTrackerMode(BaseHero baseHero, PermanentModeMenu menu)
            : base(baseHero, menu)
        {
        }

        protected override void Execute()
        {
            var time = GameManager.RawGameTime;

            foreach (var unit in this.TargetManager.EnemyHeroes)
            {
                if (!this.positions.TryGetValue(unit, out var unitPositions))
                {
                    unitPositions = new Dictionary<float, Vector3>();
                    this.positions[unit] = unitPositions;
                }
                else
                {
                    foreach (var unitPosition in unitPositions.ToList())
                    {
                        var key = unitPosition.Key;
                        if (key + 4.1f < time)
                        {
                            unitPositions.Remove(key);
                        }
                    }
                }

                unitPositions[time] = unit.Position;
            }

            if (this.TargetManager.HasValidTarget)
            {
                if (!this.positions.TryGetValue(this.TargetManager.Target, out var unitPositions))
                {
                    return;
                }

                var glimpsePosition = unitPositions.OrderBy(x => x.Key).FirstOrDefault(x => time - x.Key > 4f).Value;
                if (glimpsePosition.IsZero)
                {
                    this.RemoveGlimpseParticle();
                    return;
                }

                this.DrawGlimpseParticle(glimpsePosition);
            }
            else
            {
                this.RemoveGlimpseParticle();
            }
        }

        private void DrawGlimpseParticle(Vector3 position)
        {
            if (this.targetParticleEffect == null)
            {
                this.targetParticleEffect = ParticleManager.CreateParticle(@"materials\ensage_ui\particles\target.vpcf", position);
                this.targetParticleEffect.SetControlPoint(6, new Vector3(255));
            }

            this.targetParticleEffect.SetControlPoint(2, this.TargetManager.Target.Position);
            this.targetParticleEffect.SetControlPoint(5, this.color);
            this.targetParticleEffect.SetControlPoint(7, position);
        }

        private void RemoveGlimpseParticle()
        {
            if (this.targetParticleEffect == null)
            {
                return;
            }

            this.targetParticleEffect.Dispose();
            this.targetParticleEffect = null;
        }
    }
}