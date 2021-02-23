namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.slark_dark_pact)]
    internal class DarkPact : AbilityModule
    {
        private Particle effect;

        private UpdateHandler handler;

        private Unit9 unit;

        public DarkPact(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
        }

        protected override void Disable()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            UpdateManager.DestroyIngameUpdate(this.handler);
        }

        protected override void Enable()
        {
            this.handler = UpdateManager.CreateIngameUpdate(0, false, this.OnUpdate);
            ModifierManager.ModifierAdded += this.OnModifierAdded;
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_slark_dark_pact")
                {
                    return;
                }

                this.unit = EntityManager9.GetUnit(sender.Handle);
                if (this.unit == null)
                {
                    return;
                }

                this.effect = ParticleManager.CreateParticle(
                    "particles/units/heroes/hero_slark/slark_dark_pact_start.vpcf",
                    ParticleAttachment.AbsOriginFollow,
                    sender);

                ModifierManager.ModifierRemoved += this.OnModifierRemoved;
                this.handler.IsEnabled = true;
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
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_slark_dark_pact")
                {
                    return;
                }

                this.handler.IsEnabled = false;
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                this.effect.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnUpdate()
        {
            try
            {
                if (!this.effect.IsValid || !this.unit.IsAlive || !this.unit.IsVisible)
                {
                    this.handler.IsEnabled = false;
                    return;
                }

                this.effect.SetControlPoint(1, this.unit.Position);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}