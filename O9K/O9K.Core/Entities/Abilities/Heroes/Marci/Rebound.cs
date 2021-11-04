using System;
using System.Collections.Generic;
using System.Linq;

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
        DamageData = new SpecialData(baseAbility, "impact_damage");
        MinJumpDistanceData = new SpecialData(baseAbility, "min_jump_distance");
        ManJumpDistanceData = new SpecialData(baseAbility, "max_jump_distance");
        LandingRadiusData = new SpecialData(baseAbility, "landing_radius");
        ExtraRange = new SpecialData(baseAbility.Owner, AbilityId.special_bonus_unique_marci_lunge_range);
    }

    public SpecialData LandingRadiusData { get; set; }

    public SpecialData ManJumpDistanceData { get; set; }

    public SpecialData MinJumpDistanceData { get; set; }

    public SpecialData ExtraRange { get; set; }

    public float MinJumpDistance => MinJumpDistanceData.GetValue(Level);
    public float LandingRadius => LandingRadiusData.GetValue(Level);

    public float MaxJumpDistance => ManJumpDistanceData.GetValue(Level) + ExtraRange.GetValue(0);

    public override float CastRange => base.CastRange + ExtraRange?.GetTalentValue(0) ?? 0;

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
    
    private bool CheckRange(Unit9 baseTargetUnit, Unit9 jumpTarget)
    {
        var dist = baseTargetUnit.Distance(jumpTarget);
        var half = LandingRadius / 2;
        return dist > MinJumpDistance - half - baseTargetUnit.HullRadius - Owner.HullRadius - 75 &&
               dist < MaxJumpDistance + half;
    }

    private Unit9 GetUnitForJump(Unit9 target)
    {
        var startTarget = EntityManager9.Units
            .Where(x => x.IsUnit && x.IsAlive && x.IsVisible && x != target && !x.Equals(Owner) &&
                        CheckRange(x, target))
            .OrderBy(x => x.Distance(Owner))
            .FirstOrDefault();

        return startTarget;
    }

    public override bool CanHit(Unit9 target)
    {
        var canHitBase = base.CanHit(target);
        if (!canHitBase)
            return false;

        var targetForJump = GetUnitForJump(target);
        return targetForJump != null;
    }

    public bool UseAbility(Unit9 startTarget, Vector3 endPosition, bool queue = false, bool bypass = false)
    {
        var result = BaseAbility.Cast(startTarget.BaseUnit, endPosition, queue, bypass);
        if (result)
        {
            ActionSleeper.Sleep(0.1f);
        }

        return result;
    }

    public override bool UseAbility(Unit9 target, bool queue = false, bool bypass = false)
    {
        var position = target.Position;
        var startTarget = GetUnitForJump(target);

        if (startTarget == null)
        {
            return Owner.Move(position);
        }

        return UseAbility(startTarget, position, queue, bypass);
    }
}