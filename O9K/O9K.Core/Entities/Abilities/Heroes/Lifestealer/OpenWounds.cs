namespace O9K.Core.Entities.Abilities.Heroes.Lifestealer
{
    using Base;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.life_stealer_open_wounds)]
    public class OpenWounds : RangedAbility, IDebuff
    {
        public OpenWounds(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public string DebuffModifierName { get; } = "modifier_life_stealer_open_wounds";
    }
}