namespace O9K.Core.Entities.Abilities.Heroes.Chen
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.chen_divine_favor)]
    public class DivineFavor : RangedAbility
    {
        public DivineFavor(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}