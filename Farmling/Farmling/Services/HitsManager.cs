using System.Linq;
using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Projectile.Projectiles;
using Divine.Update;
using Farmling.Global;
using Farmling.Interfaces;
using Farmling.LoggerService;
using Farmling.Models;

namespace Farmling.Services;

public class HitsManager : IHitsManager
{
    private readonly IDamageCalculateService _damageCalculateService;

    public HitsManager(IProjectileTrackManager projectileTrackManager, IDamageCalculateService damageCalculateService)
    {
        _damageCalculateService = damageCalculateService;
        projectileTrackManager.Notify += AddProjectileToHit;
        UpdateManager.IngameUpdate += TargetUpdater;
        // Entity.AnimationChanged += AnimationTracker;
        UpdateManager.IngameUpdate += UnitsRegisterer;
        UpdateManager.CreateIngameUpdate(1000, UnitsCleaner);
        Entity.NetworkPropertyChanged += OnEntityOnNetworkPropertyChanged;
    }

    private readonly HashSet<NetworkActivity> attackActivities = new HashSet<NetworkActivity>
    {
        NetworkActivity.Attack,
        NetworkActivity.Attack2,
        NetworkActivity.Crit,
        NetworkActivity.AttackEvent,
        NetworkActivity.AttackEventBash
    };

    private void OnEntityOnNetworkPropertyChanged(Entity entity, NetworkPropertyChangedEventArgs e)
    {
        if (e.PropertyName != "m_NetworkActivity")
        {
            return;
        }

        var activity = (NetworkActivity) e.NewValue.GetInt32();
        var oldActivity = (NetworkActivity) e.OldValue.GetInt32();

        if (activity == oldActivity)
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            if (entity is not Unit unit || !Units.Contains(unit)) return;

            if (attackActivities.Contains(activity))
            {
                AddHit(unit);
            }
            else
            {
                RemoveHit(unit);
            }
        });
    }

    private static Hero Owner => EntityManager.LocalHero!;

    public List<Unit> Units { get; set; } = new();
    public Dictionary<uint, List<Hit>> HitSources { get; set; } = new();
    public event IHitsManager.HitEvent? Notify;

    public void TryToAddEntity(Entity entityToAdd)
    {
        if (!Units.Exists(entity => entity.Handle == entityToAdd.Handle) && entityToAdd is Unit unit)
        {
            Logger.Log($"Registered unit: {unit.Name}");
            Units.Add(unit);
        }
    }

    public IReadOnlyList<Unit> GetUnitsInSystem()
    {
        return Units;
    }

    private void UnitsCleaner()
    {
        foreach (var unit in Units.ToList().Where(unit => !unit.IsValid || !Owner.IsInRange(unit, 1500)))
        {
            Units.Remove(unit);
        }
    }

    private void UnitsRegisterer()
    {
        var units = EntityManager.GetEntities<Unit>().Where(x => x.IsValid && x.IsAlive && x.IsInRange(Owner, 1500) && x.IsSpawned && !x.Equals(Owner) && (x is Creep or Hero or Tower || x.Name.Equals("npc_dota_goodguys_siege")));
        foreach (var unit in units) TryToAddEntity(unit);
    }

    private void AnimationTracker(Entity sender, AnimationChangedEventArgs e)
    {
        var attackName = "attack";
        if (sender is Unit unit)
        {
            if (e.Name.StartsWith(attackName))
            {
                if (Units.Contains(unit))
                {
                    AddHit(unit);
                }
            }
            else if (e.Name.EndsWith(attackName))
            {
                if (Units.Contains(unit))
                {
                    AddHit(unit);
                }
            }
            else
            {
                RemoveHit(unit);
            }
        }
    }

    private void TargetUpdater()
    {
        HitSources.ForEach(pair =>
        {
            var unit = EntityManager.GetEntityByHandle(pair.Key) as Unit;
            if (unit != null /* && unit is not Tower*/)
            {
                var hitsList = pair.Value;
                var lastHit = hitsList.Where(x => x.IsValid).MaxBy(x => x.CreatedAt);
                if (lastHit != null && lastHit.SimpleHitAfter > GameManager.RawGameTime)
                {
                    if (unit is Tower tower)
                    {
                        if (lastHit.Target != tower.AttackTarget)
                        {
                            lastHit.Target = tower.AttackTarget;
                            if (tower.AttackTarget != null)
                                lastHit.Damage = _damageCalculateService.GetDamage(unit, tower.AttackTarget);
                        }
                    }
                    else
                    {
                        var attackEntity = EntityManager.GetEntities<Unit>().Where(x => x.IsValid && x.IsSpawned && !x.Equals(unit) && x.IsAlive && x is Hero or Creep or Tower).MinBy(x => unit.FindRotationAngle(x.Position));
                        if (lastHit.Target != attackEntity)
                        {
                            lastHit.Target = attackEntity;
                            if (attackEntity != null)
                                lastHit.Damage = _damageCalculateService.GetDamage(unit, attackEntity);
                        }
                    }
                }
            }
        });
    }

    private Hit AddHit(Unit unit)
    {
        var handle = unit.Handle;
        var newHit = new Hit(unit);
        if (HitSources.TryGetValue(handle, out var hits))
        {
            hits.Add(newHit);
        }
        else
        {
            var hitsList = new List<Hit> {newHit};
            HitSources.Add(unit.Handle, hitsList);
            Notify?.Invoke(newHit, true);
        }

        return newHit;
    }

    private void RemoveHit(Unit unit)
    {
        var handle = unit.Handle;
        if (HitSources.TryGetValue(handle, out var hits))
        {
            var hit = hits.Where(x => x.IsValid && x.Owner.Handle.Equals(handle)).MaxBy(x => x.CreatedAt);
            if (hit != null)
            {
                if (hit.IsMelee)
                {
                    Logger.Log("Remove hit from list");
                    hits.Remove(hit);
                    Notify?.Invoke(hit, false);
                }
                else if (hit.Projectile == null)
                {
                    Logger.Log("Remove hit from list [RANGE]");
                    hits.Remove(hit);
                    Notify?.Invoke(hit, false);
                }
            }
        }
    }

    private void AddProjectileToHit(TrackingProjectile projectile, bool isCreated)
    {
        var unit = projectile.Source as Unit;
        if (unit == null)
            // TODO: throw
            return;
        if (!attackActivities.Contains(unit.NetworkActivity))
        {
            return;
        }

        GlobalStatsService.SaveProjectileToStatistics(projectile);
        var handle = unit.Handle;
        if (HitSources.TryGetValue(handle, out var hits))
        {
            if (isCreated)
            {
                var hit = hits.FirstOrDefault(x => x.Owner.Handle.Equals(handle) && Math.Abs(x.CreatedAt - GameManager.RawGameTime) <= 100);
                if (hit != null)
                    hit.Projectile = projectile;
                else
                    //TODO: unexpected projectile. Throw
                    Logger.Log($"Unexpected projectile: {unit.Name} | {unit.Handle} -> Proj: {projectile.Handle}");
            }
            else
            {
                var hitWithProjectile = hits.FirstOrDefault(x => x.Projectile != null && x.Projectile.Handle.Equals(projectile.Handle));
                if (hitWithProjectile != null)
                {
                    Logger.Log($"Removed hit: {unit.Name} | {unit.Handle} -> Proj: {projectile.Handle}");
                    hits.Remove(hitWithProjectile);
                }
            }
        }
    }
}
