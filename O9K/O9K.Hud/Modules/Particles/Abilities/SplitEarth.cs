namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;

    using Core.Entities.Metadata;
    using Core.Helpers;
    using Core.Logger;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    using SharpDX;

    [AbilityId(AbilityId.leshrac_split_earth)]
    internal class SplitEarth : AbilityModule
    {
        private readonly Vector3 radius;

        public SplitEarth(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            var radiusData = new SpecialData(AbilityId.leshrac_split_earth, "radius").GetValue(3);
            this.radius = new Vector3(radiusData, -radiusData, -radiusData);
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
                var modifer = e.Modifier;
                var sender = modifer.Owner;
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifer.Name != "modifier_leshrac_split_earth_thinker")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle("particles/units/heroes/hero_leshrac/leshrac_split_projected.vpcf", sender.Position);
                effect.SetControlPoint(1, this.radius);

                effect.Release();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}