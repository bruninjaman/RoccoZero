using Divine.Core.ComboFactory.Menus;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class MoreMenu : BaseMoreMenu
    {
        [Menu("Auto Combo")]
        public AutoComboMenu AutoComboMenu { get; } = new AutoComboMenu();

        [Menu("Smart Arcane Bolt"), Texture(@"spells\skywrath_mage_arcane_bolt.png")]
        public SmartArcaneBoltMenu SmartArcaneBoltMenu { get; } = new SmartArcaneBoltMenu();

        [Menu("Smart Concussive Shot"), Texture(@"spells\skywrath_mage_concussive_shot.png")]
        public SmartConcussiveShotMenu SmartConcussiveShotMenu { get; } = new SmartConcussiveShotMenu();
    }
}