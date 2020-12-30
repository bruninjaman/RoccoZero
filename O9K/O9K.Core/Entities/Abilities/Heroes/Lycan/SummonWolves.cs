namespace O9K.Core.Entities.Abilities.Heroes.Lycan
{
    using Base;
    using Base.Types;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.lycan_summon_wolves)]
    public class SummonWolves : ActiveAbility, IBuff
    {
        public SummonWolves(Ability baseAbility)
            : base(baseAbility)
        {
        }

        public string BuffModifierName { get; } = string.Empty;

        public bool BuffsAlly { get; } = false;

        public bool BuffsOwner { get; } = true;
    }
}