﻿namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Smoke;

using System.Collections.Generic;
using System.Linq;

using Base;

using Core.Managers.Entity;
using Divine.Extensions;
using Divine.Game;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Components;

using Helpers.Notificator;
using Helpers.Notificator.Notifications;

internal class SmokeAbilityData : AbilityFullData
{
    public override void AddDrawableAbility(
        List<IDrawableAbility> drawableAbilities,
        Particle particle,
        Team allyTeam,
        INotificator notificator)
    {
        var position = particle.GetControlPoint(this.ControlPoint);
        if (EntityManager9.Heroes.Any(x => x.IsUnit && x.Team == allyTeam && x.IsAlive && x.Distance(position) < 800))
        {
            return;
        }

        var drawableAbility = new SimpleDrawableAbility
        {
            AbilityTexture = this.AbilityId.ToString(),
            ShowUntil = GameManager.RawGameTime + this.TimeToShow,
            Position = (this.RawParticlePosition ? particle.Position : particle.GetControlPoint(this.ControlPoint)).SetZ(350)
        };

        if (this.Replace)
        {
            var exist = drawableAbilities.LastOrDefault(
                x => x.AbilityTexture == drawableAbility.AbilityTexture && x.HeroTexture == drawableAbility.HeroTexture);

            if (exist != null)
            {
                drawableAbilities.Remove(exist);
            }
        }

        drawableAbilities.Add(drawableAbility);

        if (this.ShowNotification)
        {
            notificator?.PushNotification(new AbilityNotification(null, this.AbilityId.ToString()));
        }
    }
}