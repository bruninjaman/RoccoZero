namespace O9K.Core.Entities.Abilities.Items
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.item_moon_shard)]
    public class MoonShard : PassiveAbility
    {
        public MoonShard(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}