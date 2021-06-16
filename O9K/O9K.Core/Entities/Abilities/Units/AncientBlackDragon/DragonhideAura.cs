namespace O9K.Core.Entities.Abilities.Units.AncientBlackDragon
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.black_dragon_dragonhide_aura)]
    public class DragonhideAura : PassiveAbility
    {
        public DragonhideAura(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}