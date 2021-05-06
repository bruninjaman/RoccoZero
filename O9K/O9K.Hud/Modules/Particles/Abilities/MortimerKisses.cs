namespace O9K.Hud.Modules.Particles.Abilities
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Metadata;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Context;
    using Core.Managers.Particle;

    using Divine;

    using Helpers.Notificator;

    using MainMenu;

    using SharpDX;

    [AbilityId(AbilityId.snapfire_mortimer_kisses)]
    internal class MortimerKisses : AbilityModule
    {
        private readonly Queue<Particle> effects = new Queue<Particle>();

        private readonly Vector3 radius;

        public MortimerKisses(INotificator notificator, IHudMenu hudMenu)
            : base(notificator, hudMenu)
        {
            var radiusData = new SpecialData(AbilityId.snapfire_mortimer_kisses, "impact_radius").GetValue(1);
            this.radius = new Vector3(radiusData, -radiusData, -radiusData);
        }

        protected override void Disable()
        {
            Context9.ParticleManger.ParticleAdded -= this.ParticleMangerOnParticleAdded;
        }

        protected override void Enable()
        {
            Context9.ParticleManger.ParticleAdded += this.ParticleMangerOnParticleAdded;
        }

        private void ParticleMangerOnParticleAdded(Particle9 particle)
        {
            try
            {
                //if (particle.Released)
                //{
                //    return;
                //}

                switch (particle.Name)
                {
                    case "particles/units/heroes/hero_snapfire/hero_snapfire_ultimate_linger.vpcf":
                    {
                        if (this.effects.Count == 0)
                        {
                            return;
                        }

                        var effect = this.effects.Dequeue();
                        if (effect.IsValid)
                        {
                            effect.Dispose();
                        }

                        break;
                    }
                    case "particles/units/heroes/hero_snapfire/snapfire_lizard_blobs_arced.vpcf":
                    {
                        var effect = ParticleManager.CreateParticle(
                            "particles/units/heroes/hero_snapfire/hero_snapfire_ultimate_calldown.vpcf",
                            particle.GetControlPoint(1));
                        effect.SetControlPoint(1, this.radius);
                        effect.SetControlPoint(2, new Vector3(0.8f, 0, 0));

                        this.effects.Enqueue(effect);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}