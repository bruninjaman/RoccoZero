using System.Diagnostics;
using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle;
using Divine.Renderer;
using Divine.Update;
using Farmling.Config;
using Farmling.Interfaces;
using Farmling.LoggerService;

namespace Farmling.Services;

public class Debugger
{
    private readonly IFarmService _farmService;
    private readonly IHitsManager _hitsManager;

    public Debugger(MenuConfig config, IHitsManager hitsManager, IFarmService farmService)
    {
        _hitsManager = hitsManager;
        _farmService = farmService;
        config.Debugger.ValueChanged += (switcher, args) =>
        {
            Logger.IsEnabled = args.Value;
            if (args.Value)
            {
                UpdateManager.IngameUpdate += UpdateManagerOnIngameUpdate;
                RendererManager.Draw += RendererManagerOnDraw;
            }
            else
            {
                UpdateManager.IngameUpdate -= UpdateManagerOnIngameUpdate;
                RendererManager.Draw -= RendererManagerOnDraw;
            }
        };

        _hitsManager.Notify += (hit, added) =>
        {
            if (!added)
            {
                var handle = hit.Owner.Handle;
                ParticleManager.DestroyParticle($"{hit.Owner.Handle} - lastAttack - target");
                ParticleManager.DestroyParticle($"{handle} - projectile");
            }
        };
    }

    private void RendererManagerOnDraw()
    {
        _hitsManager.HitSources.ForEach(data =>
        {
            var handle = data.Key;
            var hits = data.Value;
            var target = EntityManager.GetEntityByHandle(handle) as Unit;
            if (target != null)
                foreach (var hit in hits)
                    if (hit.Projectile != null)
                        RendererManager.DrawText($"{GameManager.RawGameTime - hit.HitTime}", RendererManager.WorldToScreen(target.Position) + new Vector2(0, 50), Color.White, 15f);
                    else if (hit.IsMelee) RendererManager.DrawText($"{GameManager.RawGameTime - hit.HitTime}", RendererManager.WorldToScreen(target.Position) + new Vector2(0, 50), Color.White, 15f);
        });

        var predictionDataDictionary = _farmService.PredictionData;
        foreach (var (handle, predictionData) in predictionDataDictionary)
        {
            var healthBeforeIHit = predictionData.PredictedHealth;
            var targetDie = predictionData.WillTargetDie;
            var timeBeforeHit = predictionData.TimeBeforeHit;
            var target = EntityManager.GetEntityByHandle(handle) as Unit;
            if (target != null)
            {
                RendererManager.DrawText($"Health Predicted: {healthBeforeIHit} Hit in: {timeBeforeHit} Die? {targetDie}", RendererManager.WorldToScreen(target.Position) + new Vector2(-50, 10), Color.White, 20);
                var pos = RendererManager.WorldToScreen(target.Position) + new Vector2(-50, 30);
                foreach (var predictionDataAllHit in predictionData.AllHits)
                    if (predictionDataAllHit.IsValid && predictionDataAllHit.Owner.IsValid)
                    {
                        RendererManager.DrawText($"Hit: {predictionDataAllHit.Owner.Name} - {predictionDataAllHit.Damage} - {predictionDataAllHit.HitTime - GameManager.RawGameTime}", pos, Color.White, 15);
                        pos.Y += 20;
                    }
            }
        }
    }

    private void UpdateManagerOnIngameUpdate()
    {
        _hitsManager.HitSources.ForEach(data =>
        {
            var handle = data.Key;
            var hits = data.Value;
            var entity = EntityManager.GetEntityByHandle(handle) as Unit;
            if (entity != null)
                // Logger.Log($"{entity.Name} Hits: {hits.Count}");
                foreach (var hit in hits)
                {
                    if (hit.Target != null && hit.Target.IsValid && hit.Target.IsAlive)
                        ParticleManager.CreateTargetLineParticle($"{handle} - lastAttack - target", entity, hit.Target.Position, Color.Red);
                    else
                        ParticleManager.DestroyParticle($"{handle} - lastAttack - target");

                    if (hit.Projectile != null)
                        ParticleManager.CreateCircleParticle($"{handle} - projectile - {hit.Projectile.Handle}", hit.Projectile.Position, 50, Color.Aqua);
                    else if (!hit.IsMelee)
                        ParticleManager.DestroyParticle($"{handle} - projectile");
                }
        });
    }
}
