namespace O9K.Core.Entities.Abilities.Heroes.Hoodwink
{
    using Base;

    using Divine;

    using Metadata;

    using O9K.Core.Entities.Abilities.Base.Types;
    using O9K.Core.Entities.Units;
    using O9K.Core.Helpers;

    [AbilityId(AbilityId.hoodwink_scurry)]
    public class Scurry : ActiveAbility, ISpeedBuff
    {
        private readonly SpecialData bonusMoveSpeedData;

        public Scurry(Ability baseAbility)
            : base(baseAbility)
        {
            this.bonusMoveSpeedData = new SpecialData(baseAbility, "movement_speed_pct");
        }

        public string BuffModifierName { get; } = "modifier_hoodwink_scurry_active";

        public bool BuffsAlly { get; } = false;

        public bool BuffsOwner { get; } = true;

        public float GetSpeedBuff(Unit9 unit)
        {
            return (unit.Speed * this.bonusMoveSpeedData.GetValue(this.Level)) / 100;
        }
    }
}