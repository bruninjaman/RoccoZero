namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Logger;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.invoker_sun_strike)]
    internal class SunStrike : AbilityModule
    {
        public SunStrike(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
        }

        protected override void Disable()
        {
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
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

                if (modifier.Name != "modifier_invoker_sun_strike")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle(
                    "particles/econ/items/invoker/invoker_apex/invoker_sun_strike_team_immortal1.vpcf",
                    sender.Position);
                effect.Release();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}