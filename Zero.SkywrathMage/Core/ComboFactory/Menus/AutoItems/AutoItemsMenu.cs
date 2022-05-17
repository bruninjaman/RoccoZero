using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.Core.ComboFactory.Menus.AutoItems
{
    public sealed class AutoItemsMenu
    {
        [Menu("Auto Phase Boots"), Texture(@"items\item_phase_boots.png")]
        public AutoPhaseBootsMenu AutoPhaseBootsMenu { get; } = new AutoPhaseBootsMenu();
    }
}