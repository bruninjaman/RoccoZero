using System.Collections.Generic;
using Divine.Entity.Entities.Units.Components;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Units;
using O9K.Core.Extensions;
using O9K.Core.Helpers;
using O9K.Core.Prediction.Collision;
using O9K.Core.Prediction.Data;

namespace O9K.Core.Entities.Abilities.Heroes.Kunkka;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

[AbilityId(AbilityId.kunkka_tidal_wave)]
public class TidalWave : LineAbility, IDisable
{
    public TidalWave(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.SpeedData = new SpecialData(baseAbility, "speed");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public UnitState AppliesUnitState { get; } = UnitState.Disarmed;
    
    public override CollisionTypes CollisionTypes { get; } = CollisionTypes.None;
    
    public override PredictionInput9 GetPredictionInput(Unit9 target, List<Unit9> aoeTargets = null)
    {
        var distance = target.Distance(Owner);
        var baseInput = base.GetPredictionInput(target, aoeTargets);

        if (distance >= 900)
        {
            baseInput.ExtraRangeFromCaster = -900;
        }

        return baseInput;
    }

    public override bool CanHit(Unit9 target)
    {
        return base.CanHit(target);
    }
}
