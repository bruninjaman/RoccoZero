﻿namespace O9K.Hud.Modules.Particles.Abilities;

using System;

using Core.Entities.Metadata;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Context;
using Core.Managers.Particle;
using Divine.Numerics;
using Divine.Particle;
using Divine.Update;
using Divine.Entity.Entities.Abilities.Components;

using Helpers.Notificator;

using MainMenu;

[AbilityId(AbilityId.invoker_emp)]
internal class EMP : AbilityModule
{
    private readonly Vector3 color;

    private readonly int duration;

    private readonly Vector3 radius;

    public EMP(INotificator notificator, IHudMenu hudMenu)
        : base(notificator, hudMenu)
    {
        var radiusData = new SpecialData(AbilityId.invoker_emp, "area_of_effect").GetValue(1);
        this.radius = new Vector3(radiusData, 100, 0);
        this.duration = (int)(new SpecialData(AbilityId.invoker_emp, "delay").GetValue(1) * 1000);
        this.color = new Vector3(255, 192, 200);
    }

    protected override void Disable()
    {
        Context9.ParticleManger.ParticleAdded -= this.OnParticleAdded;
    }

    protected override void Enable()
    {
        Context9.ParticleManger.ParticleAdded += this.OnParticleAdded;
    }

    private void OnParticleAdded(Particle9 particle)
    {
        try
        {
            if (/*particle.Released || */particle.Name != "particles/units/heroes/hero_invoker/invoker_emp.vpcf")
            {
                return;
            }

            var position = particle.GetControlPoint(0);
            var effect = ParticleManager.CreateParticle("particles/ui_mouseactions/range_finder_tower_aoe.vpcf", position);

            effect.SetControlPoint(2, position);
            effect.SetControlPoint(3, this.radius);
            effect.SetControlPoint(4, this.color);

            UpdateManager.BeginInvoke(this.duration, () => effect.Dispose());
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}