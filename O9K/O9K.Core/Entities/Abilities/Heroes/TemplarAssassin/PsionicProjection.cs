namespace O9K.Core.Entities.Abilities.Heroes.TemplarAssassin
{
    using Base;
    using Base.Components;

    using Divine;

    using Metadata;

    [AbilityId(AbilityId.templar_assassin_trap_teleport)]
    public class PsionicProjection : RangedAbility, IChanneled
    {
        public PsionicProjection(Ability baseAbility)
            : base(baseAbility)
        {
            this.ChannelTime = baseAbility.AbilityData.GetChannelMaximumTime(0);
        }

        public float ChannelTime { get; }

        public bool IsActivatesOnChannelStart { get; } = false;
    }
}