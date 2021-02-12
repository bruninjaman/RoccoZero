using Divine.Core.ComboFactory.Menus;

using Ensage.SDK.Menu.Items;

namespace Divine.SkywrathMage.Menus
{
    internal sealed class FarmMenu : BaseFarmMenu
    {
        public override Selection<string> FarmItem { get; set; } = new Selection<string>(new[] { "Arcane Bolt & Attack", "Attack" });

        public override Selection<string> HeroHarrasItem { get; set; } = new Selection<string>(new[] { "Attack", "Arcane Bolt & Attack", "Disable" });
    }
}
