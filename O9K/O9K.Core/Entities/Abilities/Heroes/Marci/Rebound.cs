namespace O9K.Core.Entities.Abilities.Heroes.Marci;

using System.Linq;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Numerics;

using Metadata;

using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Managers.Entity;

[AbilityId(AbilityId.marci_companion_run)]
public class Rebound : RangedAbility, INuke
{
    public Rebound(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "impact_damage");
    }

    public override bool BreaksLinkens { get; } = false;

    //public override bool CanHit(Unit9 target)
    //{
    //    if (target.IsMagicImmune && ((target.IsEnemy(this.Owner) && !this.CanHitSpellImmuneEnemy)
    //                                 || (target.IsAlly(this.Owner) && !this.CanHitSpellImmuneAlly)))
    //    {
    //        return false;
    //    }

    //    if (this.Owner.Distance(target) > this.CastRange)
    //    {
    //        return false;
    //    }

    //    var grabTarget = EntityManager9.Units
    //        .Where(
    //            x => x.IsUnit && x.IsAlive && x.IsVisible && !x.IsMagicImmune && !x.IsInvulnerable && !x.Equals(this.Owner)
    //                 && x.Distance(this.Owner) < this.RadiusData.GetValue(this.Level))
    //        .OrderBy(x => x.Distance(this.Owner))
    //        .FirstOrDefault();

    //    if (grabTarget == null)
    //    {
    //        return false;
    //    }

    //    if (grabTarget.IsHero && !grabTarget.IsIllusion && grabTarget.IsAlly(this.Owner))
    //    {
    //        return false;
    //    }

    //    return true;
    //}

    public bool UseAbility(Unit9 startTarget, Vector3 endPosition, bool queue = false, bool bypass = false)
    {
        var result = this.BaseAbility.Cast(startTarget.BaseUnit, endPosition, queue, bypass);
        if (result)
        {
            this.ActionSleeper.Sleep(0.1f);
        }

        return result;
    }

    public override bool UseAbility(Unit9 target, bool queue = false, bool bypass = false)
    {
        var position = target.Position;

        var startTarget = EntityManager9.Units
            .Where(x => x.IsUnit && x.IsAlive && x.IsVisible && x != target && !x.Equals(this.Owner) && x.Distance(this.Owner) < this.CastRange)
            .OrderBy(x => x.Distance(this.Owner))
            .FirstOrDefault();

        if (startTarget == null)
        {
            return this.Owner.Move(position);
        }

        return this.UseAbility(startTarget, position, queue, bypass);
    }
}