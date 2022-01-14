namespace O9K.Farm.Utils;

using System;
using System.Collections.Generic;

using Divine.Particle;
using Divine.Particle.Components;
using Divine.Particle.Particles;

using Units.Base;

internal class ControlEffect : IDisposable
{
    private readonly List<Particle> effects = new List<Particle>();

    public ControlEffect(FarmUnit unit)
    {
        //this.effects.Add(
        //    ParticleManager.CreateParticle(
        //        "particles/econ/events/ti7/ti7_hero_effect_light_aura.vpcf",
        //        Attachment.AbsOriginFollow,
        //        unit.Unit.BaseUnit));

        //this.effects.Add(
        //    ParticleManager.CreateParticle(
        //        "particles/econ/events/ti7/ti7_hero_effect_aegis_back.vpcf",
        //        Attachment.AbsOriginFollow,
        //        unit.Unit.BaseUnit));

        //this.effects.Add(
        //    ParticleManager.CreateParticle(
        //        "particles/econ/events/ti7/ti7_hero_effect_aegis_top.vpcf",
        //        Attachment.AbsOriginFollow,
        //        unit.Unit.BaseUnit));

        //this.effects.Add(
        //    ParticleManager.CreateParticle(
        //        "particles/econ/events/ti8/ti8_hero_effect_base_detail.vpcf",
        //        Attachment.AbsOriginFollow,
        //        unit.Unit.BaseUnit));
    }

    public void Dispose()
    {
        //foreach (var particleEffect in this.effects)
        //{
        //    particleEffect.Dispose();
        //}
    }
}