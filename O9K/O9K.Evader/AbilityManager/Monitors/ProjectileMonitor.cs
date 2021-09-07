﻿namespace O9K.Evader.AbilityManager.Monitors;

using System;
using System.Collections.Generic;
using System.Linq;

using Abilities.Base.Evadable;
using Abilities.Base.Evadable.Components;

using Core.Entities.Abilities.Base.Components;
using Core.Extensions;
using Core.Logger;
using Core.Managers.Entity;

using Divine.Entity.Entities.Components;
using Divine.Projectile;
using Divine.Projectile.EventArgs;

internal class ProjectileMonitor : IDisposable
{
    private readonly List<EvadableAbility> evadableAbilities;

    private readonly Team myTeam;

    public ProjectileMonitor(List<EvadableAbility> evadableAbilities)
    {
        this.myTeam = EntityManager9.Owner.Team;
        this.evadableAbilities = evadableAbilities;

        ProjectileManager.TrackingProjectileAdded += this.OnAddTrackingProjectile;
    }

    public void Dispose()
    {
        ProjectileManager.TrackingProjectileAdded -= this.OnAddTrackingProjectile;
    }

    private void OnAddTrackingProjectile(TrackingProjectileAddedEventArgs args)
    {
        try
        {
            var projectile = args.Projectile;
            if (projectile.IsAutoAttackProjectile())
            {
                return;
            }
            
            if (projectile.Target == null)
            {
                return;
            }
            
            var target = EntityManager9.GetUnit(projectile.Target.Handle);
            
            if (target == null || target.Team != this.myTeam)
            {
                return;
            }

            var source = projectile.Source;
            if (source != null)
            {
                //// enemy ability, but added though ally
                //// might add some ability prediction problems...
                //var thoughAlly = target.Team == this.myTeam && source.Team == this.myTeam;

                //var ability = this.evadableAbilities.OfType<IProjectile>()
                //    .FirstOrDefault(
                //        x => (int)x.ActiveAbility.Speed == projectile.Speed && x.ActiveAbility.TimeSinceCasted < 1.5f
                //             && (thoughAlly || x.ActiveAbility.Owner.Handle == source.Handle));

                var ability = this.evadableAbilities
                    .Where(
                        x => x.Ability.Owner.Handle == source.Handle
                             && (x.Ability.TimeSinceCasted < 1.5f + (x.Ability is IChanneled channeled ? channeled.ChannelTime : 0)
                                 || x.Ability.BaseAbility.IsHidden))
                    .OfType<IProjectile>()
                    .FirstOrDefault(x => (int)x.ActiveAbility.Speed == projectile.Speed);

                ability?.AddProjectile(projectile, target);
                return;
            }

            var predictedAbilities = this.evadableAbilities.Where(
                    x => !x.Ability.Owner.IsVisible || x.Ability.TimeSinceCasted < 1.5f
                    /*+ x.AdditionalDelay*/)
                .OfType<IProjectile>()
                .Where(x => (int)x.ActiveAbility.Speed == projectile.Speed)
                .ToList();

            if (predictedAbilities.Count == 1)
            {
                predictedAbilities[0].AddProjectile(projectile, target);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}