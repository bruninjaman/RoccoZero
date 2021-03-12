namespace O9K.AIO.Heroes.Tusk
{
    using AIO.Modes.Combo;

    using Base;

    using Core.Entities.Metadata;
    using Core.Managers.Context;

    using Divine;

    [HeroId(HeroId.npc_dota_hero_tusk)]
    internal class TuskBase : BaseHero
    {
        public TuskBase()
        {
        }

        protected override void CreateComboMenus()
        {
            base.CreateComboMenus();
            this.ComboMenus.Add(new ComboModeMenu(this.Menu.RootMenu, "Kick to ally combo"));
        }
    }
}