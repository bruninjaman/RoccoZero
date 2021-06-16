namespace O9K.Core.Entities.Abilities.Heroes.Leshrac
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.leshrac_diabolic_edict)]
    public class DiabolicEdict : AreaOfEffectAbility
    {
        public DiabolicEdict(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }
    }
}