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

    [AbilityId(AbilityId.lina_light_strike_array)]
    internal class LightStrikeArray : AbilityModule
    {
        private readonly Vector3 radius;

        public LightStrikeArray(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            var radiusData = new SpecialData(AbilityId.lina_light_strike_array, "light_strike_array_aoe").GetValue(3);
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
                var modifier = e.Modifier;
                var sender = modifier.Owner;
                if (sender.Team == this.OwnerTeam)
                {
                    return;
                }

                if (modifier.Name != "modifier_lina_light_strike_array")
                {
                    return;
                }

                var effect = ParticleManager.CreateParticle("particles/econ/items/lina/lina_ti7/light_strike_array_pre_ti7.vpcf", sender.Position);
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