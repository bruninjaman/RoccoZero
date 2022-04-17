using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Divine.Helpers;
using Divine.Numerics;
using Divine.Particle;
using Divine.Prediction;
using InvokerAnnihilation.Abilities.Interfaces;

namespace InvokerAnnihilation.Abilities.MainAbilities;

public class IceWall : BaseInvokableTargetAbstractAbility
{
    public IceWall(Ability ability, AbilityId[] spheres) : base(ability, spheres)
    {
    }

    public override bool HitEnemy => true;

    public override bool CanBeCasted(Unit? target)
    {
        if (base.CanBeCasted() && target != null &&
            (IsInvoked || CanBeInvoked()))
        {
            var distance = Owner.Distance2D(target);
            return distance - 50 > 200 && distance - 50 < 610;
        }

        return false;
    }

    public override bool Cast(Unit target)
    {
        if (AbilitySleeper.Sleeping)
        {
            return true;
        }
        if (IsValid )
        {
            if (!IsInvoked)
            {
                if (CanBeInvoked())
                {
                    var invoked = Invoke();
                    AbilitySleeper.Sleep(150);
                    if (!invoked)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            var distance = Owner.Distance2D(target);
            if (!(distance - 50 > 200 && distance - 50 < 610))
            {
                Owner.Move(target.Position);
                return false;
            }
            var targetPosition = target.Position;
            var ownerPosition = Owner.Position;
            var xyz = target.IsMoving ? target.InFront(target.MovementSpeed * 0.6f) : targetPosition;
            var mepred = (targetPosition - ownerPosition) * 50 / targetPosition.Distance2D(ownerPosition) + ownerPosition;
            var v1 = xyz.X - mepred.X;
            var v2 = xyz.Y - mepred.Y;
            var a = Math.Acos(175 / xyz.Distance(mepred));
            var x1 = v1 * Math.Cos(a) - v2 * Math.Sin(a);
            var y1 = v2 * Math.Cos(a) + v1 * Math.Sin(a);
            var b = Math.Sqrt(x1 * x1 + y1 * y1);
            var k1 = x1 * 50 / b;
            var k2 = y1 * 50 / b;
            var vec1 = new Vector3((float)(k1 + mepred.X), (float)(k2 + mepred.Y), mepred.Z);
            // ParticleManager.CreateCircleParticle("key1", targetPosition, 150, Color.Red);
            // ParticleManager.CreateCircleParticle("key2", xyz, 150, Color.Green);
            // ParticleManager.CreateCircleParticle("key3", mepred, 150, Color.Red);
            // ParticleManager.CreateCircleParticle("key4", vec1, 150, Color.Bisque);
            // ParticleManager.CreateCircleParticle("key5", ownerPosition, 150, Color.Brown);
            if (vec1.Distance2D(mepred) > 0)
            {
                AbilitySleeper.Sleep(1000);
                Owner.Move(mepred);
                Owner.Move(vec1, true);
                return BaseAbility!.Cast(true);
            }
            return false;
        }
        return false;
    }

    public override bool ShouldCast(Unit? target)
    {
        if (target == null)
            return false;
        var distance = Owner.Distance2D(target);
        return distance - 50 > 200 && distance - 50 < 610;
    }
}