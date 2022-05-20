namespace Divine.Zeus
{
    using Divine.Entity;
    using Divine.Entity.Entities.Units.Heroes.Components;
    using Divine.Service;

    //[ExportPlugin(name: "Divine.Zeus", author: "YEEEEEEE", version: "", priority: 450, units: HeroId.npc_dota_hero_zuus)]
    internal sealed class Bootstrap : Bootstrapper
    {
        private Common common;

        protected override void OnActivate()
        {
            if (EntityManager.LocalHero?.HeroId != HeroId.npc_dota_hero_zuus)
            {
                return;
            }

            common = new Common();
        }

        protected override void OnDeactivate()
        {
            common?.Dispose();
        }
    }
}
