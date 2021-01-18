namespace O9K.Core.Entities.Abilities.Heroes.Tinker
{
    using System;

    using Base;
    using Base.Components;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.tinker_rearm)]
    public class Rearm : ActiveAbility, IChanneled
    {
        private readonly SpecialData channelTimeData;

        public Rearm(Ability baseAbility)
            : base(baseAbility)
        {
            this.channelTimeData = new SpecialData(baseAbility, baseAbility.AbilityData.GetChannelMaximumTime);
        }

        public float ChannelTime
        {
            get
            {
                return this.channelTimeData.GetValue(this.Level);
            }
        }

        public bool IsActivatesOnChannelStart { get; } = false;
    }
}