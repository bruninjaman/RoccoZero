namespace Divine.SkywrathMage.TargetSelector.TargetEffects.EffectType
{
    internal class WithoutCircle : BaseEffectType, IEffectType
    {
        public void Effect(Unit target)
        {
            if (target == null)
            {
                Remove();
                return;
            }

            ParticleManager.CreateOrUpdateParticle(
                "Divine.TargetEffects",
                "materials/ensage_ui/particles/target_d.vpcf",
                Owner,
                ParticleAttachment.AbsOriginFollow,
                ParticleRestartType.None,
                new ControlPoint(5, Color),
                new ControlPoint(6, 255, 255, 255),
                new ControlPoint(2, Owner.Position),
                new ControlPoint(7, target.Position));
        }
    }
}