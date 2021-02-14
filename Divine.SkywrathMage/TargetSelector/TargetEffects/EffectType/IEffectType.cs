using Divine.Core.Entities;

namespace Divine.SkywrathMage.TargetSelector.TargetEffects.EffectType
{
    public interface IEffectType
    {
        void Effect(Unit target);

        void Remove();

        int Red { get; set; }

        int Green { get; set; }

        int Blue { get; set; }
    }
}
