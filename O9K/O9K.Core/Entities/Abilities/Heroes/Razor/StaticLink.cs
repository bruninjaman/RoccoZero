namespace O9K.Core.Entities.Abilities.Heroes.Razor
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.razor_static_link)]
    public class StaticLink : RangedAbility, IDebuff
    {
        public StaticLink(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public string DebuffModifierName { get; } = string.Empty;
    }
}