namespace Divine.SkywrathMage.TargetSelector.TargetEffects.EffectType
{
    internal class Default : BaseEffectType, IEffectType
    {
        public void Effect(Unit target)
        {
            if (target == null)
            {
                Remove();
                return;
            }

            ParticleManager.TargetLineParticle("Divine.TargetEffects", Owner, target.Position, Color);
        }
    }
}