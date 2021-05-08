namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Entities.Units;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.item_bullwhip)]
    public class Bullwhip : RangedAbility, IBuff, IDebuff, ISpeedBuff
    {
        private readonly SpecialData bonusMoveSpeedData;

        public Bullwhip(Ability baseAbility)
            : base(baseAbility)
        {
            this.ActivationDelayData = new SpecialData(baseAbility, "bullwhip_delay_time");
            this.bonusMoveSpeedData = new SpecialData(baseAbility, "speed");
        }

        public string BuffModifierName { get; } = "modifier_item_bullwhip_buff";

        public bool BuffsAlly { get; } = true;

        public bool BuffsOwner { get; } = true;

        public string DebuffModifierName { get; } = "modifier_item_bullwhip_buff";

        public float GetSpeedBuff(Unit9 unit)
        {
            return (unit.Speed * this.bonusMoveSpeedData.GetValue(this.Level)) / 100;
        }
    }
}