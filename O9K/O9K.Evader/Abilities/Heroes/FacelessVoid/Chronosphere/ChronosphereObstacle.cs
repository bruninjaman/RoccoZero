namespace O9K.Evader.Abilities.Heroes.FacelessVoid.Chronosphere;

using System.Collections.Generic;
using System.Linq;

using Base.Evadable;

using Core.Entities.Units;
using Divine.Numerics;
using Divine.Modifier.Modifiers;
using Divine.Entity.Entities.Abilities.Components;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal class ChronosphereObstacle : AreaOfEffectModifierObstacle
{
    public ChronosphereObstacle(EvadableAbility ability, Vector3 position, Modifier modifier)
        : base(ability, position, modifier)
    {
    }

    public override IEnumerable<AbilityId> GetBlinks(Unit9 ally)
    {
        return Enumerable.Empty<AbilityId>();
    }

    public override IEnumerable<AbilityId> GetCounters(Unit9 ally)
    {
        return Enumerable.Empty<AbilityId>();
    }

    public override IEnumerable<AbilityId> GetDisables(Unit9 ally)
    {
        return Enumerable.Empty<AbilityId>();
    }
}