namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.item_philosophers_stone)]
    public class PhilosophersStone : PassiveAbility
    {
        public PhilosophersStone(Ability baseAbility)
            : base(baseAbility)
        {
        }
    }
}