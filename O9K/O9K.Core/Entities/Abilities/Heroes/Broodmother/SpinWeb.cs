namespace O9K.Core.Entities.Abilities.Heroes.Broodmother
{
    using Base;

    using Divine;

    using Helpers;

    using Metadata;

    [AbilityId(AbilityId.broodmother_spin_web)]
    public class SpinWeb : CircleAbility
    {
        public SpinWeb(Ability baseAbility)
            : base(baseAbility)
        {
            this.RadiusData = new SpecialData(baseAbility, "radius");
        }
    }
}