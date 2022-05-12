﻿using Divine.Entity.Entities.Units;
using Divine.Numerics;

namespace InvokerAnnihilation.Abilities.Interfaces;

public interface IPointAbility
{
    bool Cast(Vector3 targetPosition, Unit target);
    bool CanBeCasted(Vector3 targetPosition);
    float CastRange { get; }
}