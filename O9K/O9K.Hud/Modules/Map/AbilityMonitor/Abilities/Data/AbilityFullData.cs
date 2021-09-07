﻿namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data;

using System;
using System.Collections.Generic;
using System.Linq;

using Base;

using Core.Entities.Abilities.Base;
using Core.Managers.Entity;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;

using Helpers.Notificator;
using Helpers.Notificator.Notifications;

internal class AbilityFullData
{
    public AbilityId AbilityId { get; set; }

    public int ControlPoint { get; set; } = 0;

    public float Duration { get; set; } = 3;

    public bool IgnoreUnitAbility { get; set; } = false;

    public bool ParticleReleaseData { get; set; } = false;

    public float Range { get; set; }

    public Vector3 RangeColor { get; set; }

    public bool RawParticlePosition { get; set; } = false;

    public bool Replace { get; set; } = false;

    public bool SearchOwner { get; set; } = false;

    public bool ShowNotification { get; set; } = false;

    public bool ShowRange { get; set; } = false;

    public bool ShowTimer { get; set; } = true;

    public float TimeToShow { get; set; } = 3;

    public int Vision { get; set; }

    public virtual void AddDrawableAbility(List<IDrawableAbility> drawableAbilities, Unit unit, INotificator notificator)
    {
        var owner = unit.Owner;
        if (owner == null)
        {
            return;
        }

        var ownerName = owner.Name;
        var position = unit.Position;

        var drawableAbility = new DrawableUnitAbility
        {
            AbilityTexture = this.AbilityId.ToString(),
            HeroTexture = ownerName,
            MinimapHeroTexture = ownerName,
            Position = position.SetZ(Math.Min(position.Z, 350)),
            Unit = unit,
            IsShowingRange = this.ShowRange,
            RangeColor = this.RangeColor,
            Range = this.Range,
            Duration = this.Duration,
            ShowUntil = GameManager.RawGameTime + this.Duration,
            ShowHeroUntil = GameManager.RawGameTime + this.TimeToShow,
            ShowTimer = this.ShowTimer,
            Owner = owner,
        };

        if (this.AbilityId != AbilityId.zuus_cloud)
        {
            EntityManager9.GetUnit(unit.Handle)?.ChangeBasePosition(drawableAbility.Position);
        }

        drawableAbility.DrawRange();
        drawableAbilities.Add(drawableAbility);

        if (this.ShowNotification)
        {
            notificator?.PushNotification(new AbilityNotification(ownerName, this.AbilityId.ToString()));
        }
    }

    public virtual void AddDrawableAbility(
        List<IDrawableAbility> drawableAbilities,
        Ability9 ability,
        Unit unit,
        INotificator notificator)
    {
        if (this.IgnoreUnitAbility)
        {
            return;
        }

        var owner = ability.Owner;
        if (owner.IsVisible)
        {
            return;
        }

        var ownerName = owner.Name;

        var drawableAbility = new DrawableAbility
        {
            AbilityTexture = ability.Name,
            HeroTexture = ownerName,
            MinimapHeroTexture = ownerName,
            Position = unit.Position,
            ShowUntil = GameManager.RawGameTime + this.TimeToShow,
        };

        if (ability.Id != AbilityId.zuus_cloud)
        {
            owner.ChangeBasePosition(drawableAbility.Position);
        }

        drawableAbilities.Add(drawableAbility);

        if (this.ShowNotification)
        {
            notificator?.PushNotification(new AbilityNotification(ownerName, ability.Name));
        }
    }

    public virtual void AddDrawableAbility(
        List<IDrawableAbility> drawableAbilities,
        Particle particle,
        Team allyTeam,
        INotificator notificator)
    {
        var owner = this.SearchOwner || !(particle.Owner is Unit)
                        ? EntityManager9.Abilities.FirstOrDefault(
                                x => x.Id == this.AbilityId && x.Owner.Team != allyTeam && x.Owner.CanUseAbilities)
                            ?.Owner
                        : EntityManager9.GetUnit(particle.Owner.Handle);

        if (owner?.IsVisible != false)
        {
            return;
        }

        string ownerName;

        if (owner.IsHero)
        {
            if (owner.Name == nameof(HeroId.npc_dota_hero_rubick)
                && (this.AbilityId != AbilityId.rubick_fade_bolt || this.AbilityId != AbilityId.rubick_telekinesis))
            {
                return;
            }

            ownerName = owner.Name;
        }
        else
        {
            ownerName = owner.Owner?.Name;

            if (ownerName == null)
            {
                return;
            }
        }

        var position = this.RawParticlePosition ? particle.Position : particle.GetControlPoint(this.ControlPoint);
        if (position.IsZero || (owner.IsHero && owner.Distance(position) < 50))
        {
            return;
        }

        var drawableAbility = new DrawableAbility
        {
            AbilityTexture = this.AbilityId.ToString(),
            HeroTexture = ownerName,
            MinimapHeroTexture = ownerName,
            ShowUntil = GameManager.RawGameTime + this.TimeToShow,
            Position = position.SetZ(350)
        };

        owner.ChangeBasePosition(drawableAbility.Position);

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
            notificator?.PushNotification(new AbilityNotification(ownerName, this.AbilityId.ToString()));
        }
    }
}