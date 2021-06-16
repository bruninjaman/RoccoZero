namespace O9K.Core.Entities.Abilities.Heroes.Timbersaw
{
    using Base;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Metadata;

    [AbilityId(AbilityId.shredder_return_chakram)]
    [AbilityId(AbilityId.shredder_return_chakram_2)]
    public class ReturnChakram : ActiveAbility
    {
        public ReturnChakram(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}