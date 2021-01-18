namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Metadata;
    using Core.Logger;
    using Core.Managers.Context;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.sniper_assassinate)]
    internal class Assassinate : AbilityModule
    {
        private readonly Dictionary<uint, Particle> effects = new Dictionary<uint, Particle>();

        public Assassinate(INotificator notificator, IHudMenu hudMenu)
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

                if (modifier.Name != "modifier_sniper_assassinate")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle(
                    "particles/units/heroes/hero_sniper/sniper_crosshair.vpcf",
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

                if (modifier.Name != "modifier_sniper_assassinate")
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