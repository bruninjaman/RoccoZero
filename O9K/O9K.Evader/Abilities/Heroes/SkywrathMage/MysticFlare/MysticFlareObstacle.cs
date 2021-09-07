namespace O9K.Evader.Abilities.Heroes.SkywrathMage.MysticFlare;

using System.Collections.Generic;
using System.Linq;

using Base.Evadable;

using Core.Entities.Units;
using Divine.Numerics;
using Divine.Modifier.Modifiers;
using Divine.Entity.Entities.Abilities.Components;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal class MysticFlareObstacle : AreaOfEffectModifierObstacle
{
    public MysticFlareObstacle(EvadableAbility ability, Vector3 position, Modifier modifier)
        : base(ability, position, modifier)
    {
    }

    public override IEnumerable<AbilityId> GetBlinks(Unit9 ally)
    {
        if (ally.IsRooted)
        {
            return this.Blinks;
        }

        return Enumerable.Empty<AbilityId>();
    }
}