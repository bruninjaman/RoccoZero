namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Logger;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.tusk_snowball)]
    internal class Snowball : AbilityModule
    {
        private Particle effect;

        public Snowball(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
        }

        protected override void Disable()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        protected override void Enable()
        {
            ModifierManager.ModifierAdded += this.OnModifierAdded;
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            try
            {
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team != this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_tusk_snowball_target")
                {
                    return;
                }

                this.effect = ParticleManager.CreateParticle(
                    "particles/units/heroes/hero_tusk/tusk_snowball_target.vpcf",
                    ParticleAttachment.OverheadFollow,
                    sender);

                ModifierManager.ModifierRemoved += this.OnModifierRemoved;
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
                if (sender.Team != this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_tusk_snowball_target")
                {
                    return;
                }

                this.effect.Dispose();
                ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}