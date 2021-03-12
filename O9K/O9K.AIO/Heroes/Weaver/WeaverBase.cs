namespace O9K.AIO.Heroes.Weaver
{
    using AIO.Modes.Permanent;

    using Base;

    using Core.Entities.Metadata;
    using Core.Managers.Context;

    using Divine;

    using Modes;

    [HeroId(HeroId.npc_dota_hero_weaver)]
    internal class WeaverBase : BaseHero
    {
        private readonly HealthTrackerMode healthTrackerMode;

        public WeaverBase()
        {
            this.healthTrackerMode = new HealthTrackerMode(this, new PermanentModeMenu(this.Menu.RootMenu, "Health tracker"));
        }

        public override void Dispose()
        {
            base.Dispose();
            this.healthTrackerMode.Dispose();
        }

        protected override void DisableCustomModes()
        {
            this.healthTrackerMode.Disable();
        }

        protected override void EnableCustomModes()
        {
            this.healthTrackerMode.Enable();
        }
    }
}