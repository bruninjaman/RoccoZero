namespace O9K.Evader.Abilities.Items.Radiance
{
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Items;
    using Core.Entities.Units;

    using Divine;

    using Metadata;

    using Pathfinder.Obstacles;

    internal class RadianceUsable : CounterAbility
    {
        private readonly Radiance radiance;

        public RadianceUsable(Ability9 ability, IMainMenu menu)
            : base(ability, menu)
        {
            this.radiance = (Radiance)ability;
        }

        public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            if (!base.CanBeCasted(ally, enemy, obstacle))
            {
                return false;
            }

            if (this.radiance.Enabled)
            {
                return false;
            }

            return true;
        }

        public override bool Use(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            if (!this.radiance.UseAbility(false, true))
            {
                return false;
            }

            UpdateManager.BeginInvoke(
                3000,
                () =>
                    {
                        if (!this.radiance.IsValid || !this.radiance.Enabled)
                        {
                            return;
                        }

                        this.radiance.UseAbility(false, true);
                    });

            return true;
        }
    }
}