namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Entities.Units;
using O9K.Core.Helpers;
using O9K.Core.Prediction.Data;
using System.Collections.Generic;

[AbilityId(AbilityId.techies_sticky_bomb)]
public class StickyBomb : CircleAbility, INuke, IDebuff
{
    public StickyBomb(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.SpeedData = new SpecialData(baseAbility, "speed");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public string DebuffModifierName { get; } = "modifier_techies_sticky_bomb_slow";

    public virtual PredictionInput9 GetPredictionInput(Unit9 target, List<Unit9> aoeTargets = null)
    {
        var input = new PredictionInput9
        {
            Caster = this.Owner,
            Target = target,
            CollisionTypes = this.CollisionTypes,
            Delay = this.CastPoint + this.ActivationDelay + InputLag + 1.2f,
            Speed = float.MaxValue,
            Range = this.Range,
            Radius = this.Radius,
            CastRange = this.CastRange,
            SkillShotType = this.SkillShotType,
            RequiresToTurn = !this.NoTargetCast
        };

        if (aoeTargets != null)
        {
            input.AreaOfEffect = this.HasAreaOfEffect;
            input.AreaOfEffectTargets = aoeTargets;
        }

        return input;
    }
}