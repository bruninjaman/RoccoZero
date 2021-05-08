namespace O9K.Core.Entities.Abilities.NeutralItems
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    using SharpDX;

    [AbilityId(AbilityId.item_psychic_headband)]
    public class PsychicHeadband : RangedAbility
    {
        public PsychicHeadband(Ability baseAbility)
            : base(baseAbility)
        {
            this.RangeData = new SpecialData(baseAbility, "push_length");
        }

        public override float Range
        {
            get
            {
                return this.RangeData.GetValue(this.Level);
            }
        }

        public override float Speed { get; } = 1333.33f;

        public override float GetHitTime(Vector3 position)
        {
            return this.GetCastDelay(position) + this.ActivationDelay + (this.Range / this.Speed);
        }
    }
}