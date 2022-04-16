namespace O9K.Core.Entities.Abilities.Heroes.PhantomAssassin;

using Base;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Metadata;

[AbilityId(AbilityId.phantom_assassin_blur)]
public class Blur : ActiveAbility
{
    public Blur(Ability baseAbility)
        : base(baseAbility)
    {
    }

    public override float CastPoint
    {
        get
        {
            if (Owner.HasAghanimsScepter)
            {
                return 0;
            }

            return base.CastPoint;
        }
    }
}