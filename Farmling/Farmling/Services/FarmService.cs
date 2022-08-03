using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
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
    private readonly IDamageCalculateService _damageCalculateService;
    private readonly IHitsManager _hitsManager;
    private readonly Color CanKillColor = new(50, 255, 50, 100);
    private readonly Color NoKillColor = new(255, 255, 255, 100);

    public FarmService(IHitsManager hitsManager, MenuConfig config, IDamageCalculateService damageCalculateService)
    {
        _hitsManager = hitsManager;
        _config = config;
        _damageCalculateService = damageCalculateService;
        Owner = EntityManager.LocalHero!;
        var handler = UpdateManager.CreateIngameUpdate(250, false, MoveToMouseHandler);
        _config.FarmKey.ValueChanged += (key, args) =>
        {
            handler.IsEnabled = args.Value && _config.MoveToMouse;
            if (args.Value)
            {
                ParticleManager.CreateRangeParticle($"{Owner.Handle} attack-range", Owner, Owner.AttackRange(), Color.White);
            }
            else
            {
                ParticleManager.DestroyParticle($"{Owner.Handle} attack-range");
            }
        };
        _config.Enabled.ValueChanged += (key, args) =>
        {
            if (args.Value)
            {
                RendererManager.Draw += OnDrawHandler;
                UpdateManager.Update += DamageCalculation;
                UpdateManager.IngameUpdate += SkamChecking;
            }
            else
            {
                UpdateManager.IngameUpdate -= SkamChecking;
                UpdateManager.Update -= DamageCalculation;
                RendererManager.Draw -= OnDrawHandler;
            }
        };
    }

    public Hero Owner { get; set; }

    public float GetTimeToHitTarget(Unit target)
    {
        // Console.WriteLine($"Ping {GameManager.Ping} {GameManager.Ping / 2000} HitTIme{Owner.PredictProjectileArrivalTime(target)}");
        return (Owner.IsMelee ? Owner.AttackPoint() : Owner.PredictProjectileArrivalTime(target)) - 0.05f + GameManager.Ping / 2000;
    }

    public Dictionary<uint, PredictionData> PredictionData { get; set; } = new();

    private void OnDrawHandler()
    {
        foreach (var target in _hitsManager.GetUnitsInSystem().Where(x => x != null && x.IsValid && x.IsAlive))
        {
            try
            {
                if (PredictionData.TryGetValue(target.Handle, out var predictionData))
                {
                    var pos = target.GetHealthBarPosition();
                    if (pos.IsZero)
                        continue;
                    var predictedHealth = predictionData.PredictedHealth;
                    var myDamage = predictionData.MyDamage;
                    if (myDamage <= 0)
                    {
                        myDamage = _damageCalculateService.GetDamage(Owner, target);
                    }

                    var maxHealth = target.MaximumHealth;
                    var baseRect = new RectangleF(pos.X, pos.Y, UnitExtension.HealthBarSize.X, UnitExtension.HealthBarSize.Y);
                    var percent = predictedHealth / maxHealth;
                    var myDamagePercent = myDamage / maxHealth;
                    baseRect.Width *= Math.Max(percent, 0.01f);
                    RendererManager.DrawFilledRectangle(baseRect, predictionData.WillTargetDie ? CanKillColor : NoKillColor);
                    if (percent > myDamagePercent)
                    {
                        baseRect = new RectangleF(pos.X, pos.Y, UnitExtension.HealthBarSize.X, UnitExtension.HealthBarSize.Y);
                        baseRect.Width *= Math.Max(Math.Min(myDamagePercent, percent), 0.01f);
                        RendererManager.DrawFilledRectangle(baseRect, Color.Orange);
                        if (_config.Debugger)
                            RendererManager.DrawText($"{myDamage:F1}", new Vector2(pos.X, pos.Y - 15), Color.White, 15);
                    }
                }
                else
                {
                    var myDamage = _damageCalculateService.GetDamage(Owner, target);
                    var pos = target.GetHealthBarPosition();
                    if (pos.IsZero)
                        continue;
                    var maxHealth = target.MaximumHealth;
                    var baseRect = new RectangleF(pos.X, pos.Y, UnitExtension.HealthBarSize.X, UnitExtension.HealthBarSize.Y);
                    var myDamagePercent = myDamage / maxHealth;
                    baseRect.Width *= Math.Max(myDamagePercent, 0.01f);
                    RendererManager.DrawFilledRectangle(baseRect, Color.Orange);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
        try
        {
            var allHits = _hitsManager.HitSources.SelectMany(x => x.Value)
                .Where(x => x.Target != null && x.IsValid)
                .GroupBy(x => x.Target!)
                .ToDictionary(x => x.Key, hits => hits.Select(x => x).ToList());
            var dict = new Dictionary<uint, PredictionData>();
            foreach (var target in _hitsManager.GetUnitsInSystem().Where(x => x != null && x.IsValid && x.IsAlive).Where(x => _config.DenyCreeps && x.IsAlly(Owner) || _config.FarmCreeps && x.IsEnemy(Owner)))
            {
                var hitTime = GetTimeToHitTarget(target);
                var timeBeforeHit = GameManager.RawGameTime + hitTime - 0.01f;

                if (allHits.TryGetValue(target, out var hits))
                {
                    var hitsThatCanHitBeforeHeroCanHit = hits.Where(x => x.HitTime < timeBeforeHit).ToList();
                    var totalDamage = hitsThatCanHitBeforeHeroCanHit.Sum(x => x.Damage);
                    var healthBeforeIHit = target.Health - totalDamage;
                    var myDamage = _damageCalculateService.GetDamage(Owner, target);
                    var willDie = healthBeforeIHit - myDamage < 0;
                    dict.Add(target.Handle, new PredictionData(healthBeforeIHit, hitTime, willDie, hits, hitsThatCanHitBeforeHeroCanHit, myDamage));

                    if (willDie && Owner.CanAttack(target) && Owner.IsInAttackRange(target, 50) && _config.FarmKey.Value && !MultiSleeper<string>.Sleeping("Farmlink"))
                    {
                        if (Owner.IsInAttackRange(target))
                        {
                            Logger.Log($"Damage: {myDamage} -> CurrentHealth: {target.Health} HealthWhenHitPanneded: {healthBeforeIHit}");
                            Owner.Attack(target);
                            MultiSleeper<string>.Sleep("Farmlink", Owner.AttackPoint() * 2000);
                        }
                    }
                }
                else
                {
                    var willDie = target.Health - GetMyDamage(target) < 0;
                    dict.Add(target.Handle, new PredictionData(target.Health, hitTime, willDie));
                    if (willDie && Owner.CanAttack(target) && Owner.IsInAttackRange(target, 50) && _config.FarmKey.Value && !MultiSleeper<string>.Sleeping("Farmlink"))
                    {
                        Logger.Log($"Damage: {_damageCalculateService.GetDamage(Owner, target)} -> CurrentHealth: {target.Health} ");
                        Owner.Attack(target);
                        MultiSleeper<string>.Sleep("Farmlink", Owner.AttackPoint() * 1000);
                    }
                }
            }

            PredictionData = dict;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public float GetMyDamage(Unit target)
    {
        return Owner.GetAttackDamage(target, true);
    }
}
