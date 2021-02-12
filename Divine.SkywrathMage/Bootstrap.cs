namespace Divine.SkywrathMage
{
    //[ExportPlugin(name: "Divine.SkywrathMage", author: "YEEEEEEE", version: "", priority: 450, units: HeroId.npc_dota_hero_skywrath_mage)]
    internal sealed class Bootstrap : Bootstrapper
    {
        private Common common;

        protected override void OnActivate()
        {
            common = new Common();
        }

        protected override void OnDeactivate()
        {
            common?.Dispose();
        }
    }
}