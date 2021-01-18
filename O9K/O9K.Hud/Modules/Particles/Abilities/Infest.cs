namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Menu.Items;

    using Divine;

    using Helpers.Notificator;
    using Helpers.Notificator.Notifications;

    using MainMenu;

    [AbilityId(AbilityId.life_stealer_infest)]
    internal class Infest : AbilityModule
    {
        private readonly MenuSwitcher notificationsEnabled;

        private Particle effect;

        public Infest(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            this.notificationsEnabled = hudMenu.NotificationsMenu.GetOrAdd(new Menu("Abilities"))
                .GetOrAdd(new Menu("Used"))
                .GetOrAdd(new MenuSwitcher("Enabled"));
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
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_life_stealer_infest_effect")
                {
                    return;
                }

                this.effect = ParticleManager.CreateParticle(
                    "particles/units/heroes/hero_life_stealer/life_stealer_infested_unit.vpcf",
                    ParticleAttachment.OverheadFollow,
                    sender);

                if (this.notificationsEnabled && sender is Hero)
                {
                    this.Notificator.PushNotification(
                        new AbilityHeroNotification(
                            nameof(HeroId.npc_dota_hero_life_stealer),
                            nameof(AbilityId.life_stealer_infest),
                            sender.Name));
                }

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
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_life_stealer_infest_effect")
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