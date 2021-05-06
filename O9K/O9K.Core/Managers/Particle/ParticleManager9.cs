namespace O9K.Core.Managers.Particle
{
    using System;
    using System.Collections.Generic;

    using Divine;

    using Logger;

    public class ParticleManager9 : IParticleManager9, IDisposable
    {
        private readonly HashSet<string> ignoredParticles = new HashSet<string>
        {
            "particles/ui/hud/levelupburst.vpcf",
            "particles/ui_mouseactions/range_finder_tower_aoe.vpcf",
            "particles/ui_mouseactions/bounding_area_view.vpcf",
            "particles/ui_mouseactions/shop_highlight.vpcf",
            "particles/ui_mouseactions/select_unit.vpcf",
            "particles/dire_fx/dire_building_damage_minor.vpcf",
            "particles/dire_fx/dire_building_damage_major.vpcf",
            "particles/radiant_fx2/radiant_building_damage_minor.vpcf",
            "particles/radiant_fx2/radiant_building_damage_major.vpcf",
            "particles/world_environmental_fx/radiant_creep_spawn.vpcf",
            "particles/world_environmental_fx/dire_creep_spawn.vpcf",
        };

        public ParticleManager9()
        {
            ParticleManager.ParticleAdded += this.OnParticleEffectAdded;
            //Entity.OnParticleEffectReleased += this.OnParticleEffectReleased;
        }

        public delegate void EventHandler(Particle9 particle);

        public event EventHandler ParticleAdded;

        public void Dispose()
        {
            ParticleManager.ParticleAdded -= this.OnParticleEffectAdded;
            //Entity.OnParticleEffectReleased -= this.OnParticleEffectReleased;
        }

        private void OnParticleEffectAdded(ParticleAddedEventArgs e)
        {
            try
            {
                var particle = e.Particle;
                if (!particle.IsValid)
                {
                    return;
                }

                if (this.ignoredParticles.Contains(particle.Name))
                {
                    return;
                }

                UpdateManager.BeginInvoke(
                    () =>
                    {
                        try
                        {
                            if (!particle.IsValid)
                            {
                                return;
                            }


                            this.ParticleAdded?.Invoke(new Particle9(particle, particle.Name, particle.Owner, false));
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    });
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        /*private void OnParticleEffectReleased(Entity sender, ParticleEffectReleasedEventArgs args)
        {
            try
            {
                var particleEffect = args.ParticleEffect;
                if (!particleEffect.IsValid)
                {
                    return;
                }

                var name = particleEffect.Name;

                if (this.ignoredParticles.Contains(name))
                {
                    return;
                }

                var particle = new Particle(particleEffect, name, sender, true);

                this.ParticleAdded?.Invoke(particle);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }*/
    }
}