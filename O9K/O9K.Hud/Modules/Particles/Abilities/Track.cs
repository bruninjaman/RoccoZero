namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Metadata;
    using Core.Logger;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.bounty_hunter_track)]
    internal class Track : AbilityModule
    {
        private readonly Dictionary<uint, Particle> effects = new Dictionary<uint, Particle>();

        public Track(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
        }

        protected override void Disable()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;

            foreach (var effect in this.effects.Values)
            {
                effect.Dispose();
            }

            this.effects.Clear();
        }

        protected override void Enable()
        {
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ModifierManager.ModifierRemoved += this.OnModifierRemoved;
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

                if (modifier.Name != "modifier_bounty_hunter_track")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle(
                    "particles/econ/items/bounty_hunter/bounty_hunter_hunters_hoard/bounty_hunter_hoard_shield.vpcf",
                    ParticleAttachment.OverheadFollow,
                    sender);

                this.effects.Add(sender.Handle, effect);
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

                if (modifier.Name != "modifier_bounty_hunter_track")
                {
                    return;
                }

                if (!this.effects.TryGetValue(sender.Handle, out var effect))
                {
                    return;
                }

                effect.Dispose();
                this.effects.Remove(sender.Handle);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}