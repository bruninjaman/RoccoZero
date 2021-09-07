﻿namespace O9K.Evader.Abilities.Items.BootsOfTravel;

using Base;

using Core.Entities.Units;

using Divine.Game;
using Divine.Modifier.Modifiers;

using Pathfinder.Obstacles.Modifiers;

internal class BootsOfTravelObstacle : ModifierEnemyObstacle
{
    public BootsOfTravelObstacle(IModifierCounter ability, Modifier modifier, Unit9 modifierOwner, float range)
        : base(ability, modifier, modifierOwner, range)
    {
        this.CreateTime = GameManager.RawGameTime + 0.5f;
    }
}