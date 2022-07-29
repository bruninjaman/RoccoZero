using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Projectile.Projectiles;
using Farmling.Extensions;
using Farmling.LoggerService;

namespace Farmling.Models;

public record Hit
{
    private float _damage;
    private TrackingProjectile? _projectile;
    private Unit? _target;

    public Hit(Unit owner)
    {
        Owner = owner;
        CreatedAt = GameManager.RawGameTime;
        CanBeCancelled = owner is Hero;
    }

    public Unit Owner { get; init; }

    public Unit? Target
    {
        get => _target;
        set
        {
            _target = value;
            if (value == null) return;
            // Damage = Owner.GetAttackDamage2(value, true);
            Logger.Log($"New hit from {Owner.Name} | {Owner.Handle}| AttackPoint: {Owner.AttackPoint()}. HitIn: {HitTime - GameManager.RawGameTime}");
        }
    }

    public float Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            Logger.Log($"Calculate damage for hit: {value}. [{Owner.Name}] -> [{Target?.Name}]");
        }
    }

    public bool IsMelee => Owner.IsMelee;
    public float CreatedAt { get; init; }
    public float GetAttackPoint => Owner.AttackPoint();

    public float SimpleHitAfter => CreatedAt + Owner.AttackPoint();

    public bool IsValid
    {
        get
        {
            try
            {
                if (Owner == null || !Owner.IsValid || Target != null && (!Target.IsValid || !Target.IsAlive)) return false;

                // Logger.Log($"Check is Melee {Owner?.Name}");
                if (IsMelee) return GameManager.RawGameTime <= SimpleHitAfter;

                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error in IsValid check {Owner?.Name}");
                return false;
            }
        }
    }

    public float HitTime
    {
        get
        {
            // Logger.Log($"AttackIn: {GameManager.RawGameTime - (CreatedAt + Owner.AttackPoint())} ");
            if (IsMelee || Projectile == null && Target == null)
                // Logger.Log($"Hit in: {GameManager.RawGameTime - (CreatedAt + Owner.AttackPoint())} ITS MELEE {!(Projectile == null && Target == null)}");
                return CreatedAt + Owner.AttackPoint() + 0.05f;

            if (Projectile == null) return CreatedAt + Owner.AttackPoint() + Owner.PredictProjectileArrivalTime(Target!);

            var dist = Projectile.Position.Distance2D(Projectile.TargetPosition) - (Projectile.Target as Unit)!.HullRadius;
            var hitDelay = dist / Projectile.Speed;
            var result = GameManager.RawGameTime + hitDelay + 0.05f;
            // Logger.Log($"Dist: {Projectile.Position.Distance(Projectile.TargetPosition)} Hit in - {hitDelay}. AP: {Owner.AttackPoint()} FinalResult: {result - GameManager.RawGameTime}");
            // if (Owner is Tower)
            // {
            //     result += 1.3f;
            // }
            return result;
        }
    }

    public bool CanBeCancelled { get; init; }

    public TrackingProjectile? Projectile
    {
        get => _projectile is not {IsValid: true} ? null : _projectile;
        set
        {
            _projectile = value;
            if (value != null) Logger.Log($"Set projectile from {Owner.Name} | {Owner.Handle} to {value.Target} | AttackPoint: {Owner.AttackPoint()}. HitIn: {HitTime - GameManager.RawGameTime}");
        }
    }
}
