namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Entity;
    using Core.Managers.Particle;
    using Core.Managers.Renderer.Utils;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    using SharpDX;

    [AbilityId(AbilityId.abyssal_underlord_dark_rift)]
    internal class DarkRift : AbilityModule
    {
        private float duration;

        private float endTime;

        private Modifier modifier;

        private Unit9 unit;

        public DarkRift(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            this.EnemyOnly = false;
        }

        protected override void Disable()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            Context9.ParticleManger.ParticleAdded -= this.OnParticleAdded;
            RendererManager.Draw -= this.OnDraw;
        }

        protected override void Enable()
        {
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ModifierManager.ModifierRemoved += this.OnModifierRemoved;
            Context9.ParticleManger.ParticleAdded += this.OnParticleAdded;
        }

        private void OnDraw()
        {
            try
            {
                if (this.modifier?.IsValid != true)
                {
                    RendererManager.Draw -= this.OnDraw;
                    return;
                }

                if (this.unit?.IsVisible != true)
                {
                    return;
                }

                var position = RendererManager.WorldToScreen(this.unit.Position);
                if (position.IsZero)
                {
                    return;
                }

                var ratio = Hud.Info.ScreenRatio;
                var remainingTime = this.endTime - GameManager.RawGameTime;
                var time = Math.Ceiling(remainingTime).ToString("N0");
                var pct = (int)(100 - ((remainingTime / this.duration) * 100));

                var rec = new Rectangle9(position, new Vector2(30 * ratio));
                var outlinePosition = rec * 1.17f;

                RendererManager.DrawTexture(AbilityId.abyssal_underlord_dark_rift, rec, AbilityTextureType.Round);
                RendererManager.DrawTexture("o9k.modifier_bg", rec);
                RendererManager.DrawTexture("o9k.outline_green", outlinePosition);
                RendererManager.DrawTexture("o9k.outline_black" + pct, outlinePosition);
                RendererManager.DrawText(time, rec, Color.White, FontFlags.Center | FontFlags.VerticalCenter, 22 * ratio);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                if (modifier.Name != "modifier_abyssal_underlord_dark_rift")
                {
                    return;
                }

                this.endTime = this.modifier.DieTime;
                this.duration = this.modifier.Duration;

                RendererManager.Draw += this.OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            try
            {
                if (e.Modifier.Name != "modifier_abyssal_underlord_dark_rift")
                {
                    return;
                }

                RendererManager.Draw -= this.OnDraw;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnParticleAdded(Particle9 particle)
        {
            try
            {
                if (/*particle.Released || */particle.Name != "particles/units/heroes/heroes_underlord/abbysal_underlord_darkrift_ambient.vpcf")
                {
                    return;
                }

                this.unit = EntityManager9.GetUnit(particle.Sender.Handle);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}