namespace O9K.Core.Entities.Abilities.Heroes.Lifestealer
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.life_stealer_infest)]
    public class Infest : RangedAbility
    {
        public Infest(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public override float CastRange
        {
            get
            {
                return base.CastRange + 100;
            }
        }

        public override bool TargetsEnemy { get; } = false;
    }
}