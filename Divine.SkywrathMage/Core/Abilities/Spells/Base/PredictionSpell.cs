using System.Linq;

using Divine.SDK.Prediction;
using Divine.SDK.Prediction.Collision;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class PredictionSpell : RangedSpell
    {
        protected PredictionSpell(Ability ability)
            : base(ability)
        {
        }

        public virtual CollisionTypes CollisionTypes { get; } = CollisionTypes.None;

        public virtual bool HasAreaOfEffect
        {
            get
            {
                return CollisionTypes == CollisionTypes.None;
            }
        }

        public abstract PredictionSkillshotType PredictionSkillshotType { get; }

        public virtual float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        public virtual float Range
        {
            get
            {
                return CastRange;
            }
        }

        public override float Speed
        {
            get
            {
                return GetAbilitySpecialData("speed");
            }
        }

        public virtual PredictionInput GetPredictionInput(params Unit[] targets)
        {
            var input = new PredictionInput
            {
                Owner = Owner as Hero,   // TODO
                AreaOfEffect = HasAreaOfEffect,
                CollisionTypes = CollisionTypes,
                Delay = CastPoint + ActivationDelay,
                Speed = Speed,
                Range = CastRange,
                Radius = Radius,
                PredictionSkillshotType = PredictionSkillshotType
            };

            if (HasAreaOfEffect)
            {
                input.AreaOfEffectTargets = targets;
            }

            return input.WithTarget(targets.First());
        }

        public virtual PredictionOutput GetPredictionOutput(PredictionInput input)
        {
            return PredictionManager.GetPrediction(input);
        }

        public bool UseAbility(Unit target, HitChance minimChance)
        {
            if (!CanBeCasted)
            {
                return false;
            }

            var predictionInput = GetPredictionInput(target);
            var output = GetPredictionOutput(predictionInput);
            if (output.HitChance == HitChance.OutOfRange || output.HitChance == HitChance.Impossible)
            {
                return false;
            }

            if (predictionInput.CollisionTypes != CollisionTypes.None && output.HitChance == HitChance.Collision)
            {
                return false;
            }

            if (output.HitChance < minimChance)
            {
                return false;
            }

            return Cast(output.CastPosition);
        }
    }
}