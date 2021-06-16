using BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.Items;

namespace BeAware.MenuManager.ShowMeMore
{
    internal class MoreInformationMenu
    {
        public MoreInformationMenu(Menu showMeMoreMenu)
        {
            var moreInformationMenu = showMeMoreMenu.CreateMenu("More Information");
            SpiritBreakerChargeMenu = new SpiritBreakerChargeMenu(moreInformationMenu);
            MiranaArrowMenu = new MiranaArrowMenu(moreInformationMenu);
            PudgeHookMenu = new PudgeHookMenu(moreInformationMenu);
            WindrunnerPowershotMenu = new WindrunnerPowershotMenu(moreInformationMenu);
            AncientApparitionIceBlastMenu = new AncientApparitionIceBlastMenu(moreInformationMenu);
            InvokerEMPMenu = new InvokerEMPMenu(moreInformationMenu);
            InvokerSunStrikeMenu = new InvokerSunStrikeMenu(moreInformationMenu);
            KunkkaTorrentMenu = new KunkkaTorrentMenu(moreInformationMenu);
            LeshracSplitEarthMenu = new LeshracSplitEarthMenu(moreInformationMenu);
            LinaLightStrikeArrayMenu = new LinaLightStrikeArrayMenu(moreInformationMenu);
            BloodseekerRuptureMenu = new BloodseekerRuptureMenu(moreInformationMenu);

            PhantomAssassinBlurItem = moreInformationMenu.CreateSwitcher("Phantom Assassin Blur").SetAbilityImage(AbilityId.phantom_assassin_blur);
            LifeStealerInfestItem = moreInformationMenu.CreateSwitcher("Life Stealer Infest").SetAbilityImage(AbilityId.life_stealer_infest);
            DrawTimerItem = moreInformationMenu.CreateSlider("Draw Timer", 5, 1, 9);
        }

        public SpiritBreakerChargeMenu SpiritBreakerChargeMenu { get; }

        public MiranaArrowMenu MiranaArrowMenu { get; }

        public PudgeHookMenu PudgeHookMenu { get; }

        public WindrunnerPowershotMenu WindrunnerPowershotMenu { get; }

        public AncientApparitionIceBlastMenu AncientApparitionIceBlastMenu { get; }

        public InvokerEMPMenu InvokerEMPMenu { get; }

        public InvokerSunStrikeMenu InvokerSunStrikeMenu { get; }

        public KunkkaTorrentMenu KunkkaTorrentMenu { get; }

        public LeshracSplitEarthMenu LeshracSplitEarthMenu { get; }

        public LinaLightStrikeArrayMenu LinaLightStrikeArrayMenu { get; }

        public BloodseekerRuptureMenu BloodseekerRuptureMenu { get; }

        public MenuSwitcher PhantomAssassinBlurItem { get; }

        public MenuSwitcher LifeStealerInfestItem { get; }

        public MenuSlider DrawTimerItem { get; }
    }
}