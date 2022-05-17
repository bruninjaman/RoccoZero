﻿using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Menu.Items;

using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Attributes;

namespace Divine.SkywrathMage.Menus.Combo
{
    internal sealed class ComboMenu : BaseComboMenu
    {
        public override BaseSpellsMenu SpellsMenu { get; } = new SpellsMenu();

        public override BaseItemsMenu ItemsMenu { get; } = new ItemsMenu();

        [Menu("Mystic Flare"), Texture(@"spells\skywrath_mage_mystic_flare.png")]
        public MysticFlareMenu MysticFlareMenu { get; } = new MysticFlareMenu();
    }
}