using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Particle;
using Divine.Renderer;
using Divine.Update;
using Farmling.Config;
using Farmling.Extensions;
using Farmling.Interfaces;
using Farmling.LoggerService;
using Farmling.Models;

namespace Farmling.Services;

public class FarmService : IFarmService
{
    private readonly MenuConfig _config;
    private readonly IHitsManager _hitsManager;

    public FarmService(IHitsManager hitsManager, MenuConfig config)
    {
        _hitsManager = hitsManager;
        _config = config;
        Owner = EntityManager.LocalHero!;
        var handler = UpdateManager.CreateIngameUpdate(250, false, MoveToMouseHandler);
        _config.FarmKey.ValueChanged += (key, args) =>
        {
            handler.IsEnabled = args.Value && _config.MoveToMouse;
            if (args.Value)
            {
                UpdateManager.IngameUpdate += DamageCalculation;
                UpdateManager.IngameUpdate += SkamChecking;
                ParticleManager.CreateRangeParticle($"{Owner.Handle} attack-range", Owner, Owner.AttackRange(), Color.White);
                RendererManager.Draw += OnDrawHandler;
            }
            else
            {
                UpdateManager.IngameUpdate -= DamageCalculation;
                UpdateManager.IngameUpdate -= SkamChecking;
                ParticleManager.DestroyParticle($"{Owner.Handle} attack-range");
                RendererManager.Draw -= OnDrawHandler;
            }
        };
    }

    public Vector2 healthBarSize { get; set; }

    public Hero Owner { get; set; }

    public float GetTimeToHitTarget(Unit target)
    {
        return (Owner.IsMelee ? Owner.AttackPoint() : Owner.PredictProjectileArrivalTime(target)) - 0.05f;
    }

    public Dictionary<uint, PredictionData> PredictionData { get; set; } = new();

    private void OnDrawHandler()
    {
        foreach (var (handle, predictionData) in PredictionData)
        {
            var target = EntityManager.GetEntityByHandle(handle) as Creep;

            if (target != null)
            {
                var predictedHealth = predictionData.PredictedHealth;
                var currentHealth = target.Health;
                var maxHealth = target.MaximumHealth;
                var pos = target.GetHealthBarPosition();
                var baseRect = new RectangleF(pos.X, pos.Y, UnitExtension.HealthBarSize.X, UnitExtension.HealthBarSize.Y);
                var percent = predictedHealth / maxHealth;
                baseRect.Width *= Math.Max(percent, 0.01f);
                RendererManager.DrawFilledRectangle(baseRect, new Color(255, 255, 255, 100));
            }
        }
    }

    private void MoveToMouseHandler()
    {
        var mousePos = GameManager.MousePosition;
        if (Owner != null && Owner.IsValid && Owner.IsAlive && !Owner.IsInRange(mousePos, 50)) Owner.Move(mousePos);
    }

    private void SkamChecking()
    {
        // PredictionData.SelectMany(x=>x.)
    }

    private void DamageCalculation()
    {
        var allHits = _hitsManager.HitSources.SelectMany(x => x.Value)
            .Where(x => x.Target != null && x.IsValid)
            .GroupBy(x => x.Target!)
            .ToDictionary(x => x.Key, hits => hits.Select(x => x).ToList());
        var dict = new Dictionary<uint, PredictionData>();
        foreach (var target in _hitsManager.GetUnitsInSystem())
        {
            if (!target.IsValid || !target.IsAlive) continue;
            var hitTime = GetTimeToHitTarget(target);
            var timeBeforeHit = GameManager.RawGameTime + hitTime - 0.01f;
            if (allHits.TryGetValue(target, out var hits))
            {
                if (target is Tower) Logger.Log($"Hits in calculation: [{target.Handle}] {hits.Count}");

                var healthBeforeHit = hits.Where(x => x.HitAfter < timeBeforeHit).ToList();
                var totalDamage = healthBeforeHit.Sum(x => x.Damage);
                var healthBeforeIHit = target.Health - totalDamage;
                var willDie = healthBeforeIHit - GetMyDamage(target) < 0;
                dict.Add(target.Handle, new PredictionData(healthBeforeIHit, hitTime, willDie, hits, healthBeforeHit));

                if (willDie && Owner.CanAttack(target))
                {
                    Logger.Log($"Damage: {GetMyDamage(target)} -> CurrentHealth: {target.Health} HealthWhenHitPanneded: {healthBeforeIHit}");
                    if (Owner.IsInAttackRange(target)) Owner.Attack(target);
                }
            }
            else
            {
                var willDie = target.Health - GetMyDamage(target) < 0;
                dict.Add(target.Handle, new PredictionData(target.Health, hitTime, willDie));
                if (willDie && Owner.CanAttack(target) && Owner.IsInAttackRange(target, 50))
                {
                    Logger.Log($"Damage: {GetMyDamage(target)} -> CurrentHealth: {target.Health} ");
                    Owner.Attack(target);
                }
            }
        }

        PredictionData = dict;
    }

    public float GetMyDamage(Unit target)
    {
        return Owner.GetAttackDamage(target, true);
    }
}
