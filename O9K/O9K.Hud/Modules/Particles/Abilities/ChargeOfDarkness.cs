namespace O9K.Hud.Modules.Particles.Abilities;

using System;
using System.Collections.Generic;

using Core.Entities.Metadata;
using Core.Logger;
using Core.Managers.Menu.Items;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Particle.Particles;

using Helpers.Notificator;
using Helpers.Notificator.Notifications;

using MainMenu;

[AbilityId(AbilityId.spirit_breaker_charge_of_darkness)]
internal class ChargeOfDarkness : AbilityModule
{
    private readonly MenuSwitcher notificationsEnabled;

    private readonly Dictionary<int, Particle> effect = new();

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

            if (modifier.Name != "modifier_spirit_breaker_charge_of_darkness_vision")
            {
                return;
            }

            this.effect[sender.Index ^ modifier.Index] = ParticleManager.CreateParticle(
                "particles/units/heroes/hero_spirit_breaker/spirit_breaker_charge_target.vpcf",
                Attachment.OverheadFollow,
                sender);

            if (this.notificationsEnabled && sender is Hero)
            {
                this.Notificator.PushNotification(
                    new AbilityHeroNotification(
                        nameof(HeroId.npc_dota_hero_spirit_breaker),
                        nameof(AbilityId.spirit_breaker_charge_of_darkness),
                        sender.Name));
            }
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

            if (!this.effect.Remove(sender.Index ^ modifier.Index, out var particle))
            {
                return;
            }

            particle.Dispose();
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }
}