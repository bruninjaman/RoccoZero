namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Logger;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    [AbilityId(AbilityId.kunkka_torrent)]
    internal class Torrent : AbilityModule
    {
        public Torrent(INotificator notificator, IHudMenu hudMenu)
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

                if (modifier.Name != "modifier_kunkka_torrent_thinker")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle(
                    "particles/econ/items/kunkka/divine_anchor/hero_kunkka_dafx_skills/kunkka_spell_torrent_bubbles_fxset.vpcf",
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