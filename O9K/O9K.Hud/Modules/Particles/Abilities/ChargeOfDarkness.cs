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

    [AbilityId(AbilityId.spirit_breaker_charge_of_darkness)]
    internal class ChargeOfDarkness : AbilityModule
    {
        private readonly MenuSwitcher notificationsEnabled;

        private Particle effect;

        public ChargeOfDarkness(INotificator notificator, IHudMenu hudMenu)
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
                if (sender.Team != this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_spirit_breaker_charge_of_darkness_vision")
                {
                    return;
                }

                this.effect = ParticleManager.CreateParticle(
                    "particles/units/heroes/hero_spirit_breaker/spirit_breaker_charge_target.vpcf",
                    ParticleAttachment.OverheadFollow,
                    sender);

                if (this.notificationsEnabled && sender is Hero)
                {
                    this.Notificator.PushNotification(
                        new AbilityHeroNotification(
                            nameof(HeroId.npc_dota_hero_spirit_breaker),
                            nameof(AbilityId.spirit_breaker_charge_of_darkness),
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
                if (sender.Team != this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_spirit_breaker_charge_of_darkness_vision")
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