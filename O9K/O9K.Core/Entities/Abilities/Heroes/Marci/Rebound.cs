namespace O9K.Core.Entities.Abilities.Heroes.Marci;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.marci_companion_run)]
public class Rebound : RangedAbility, INuke
{
    public Rebound(Ability baseAbility)
        : base(baseAbility)
    {
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
}