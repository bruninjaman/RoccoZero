namespace O9K.AIO.Modes.MoveCombo;

using Abilities;
using Abilities.Menus;

using Combo;

using Core.Entities.Abilities.Base.Components.Base;
using Core.Managers.Menu;
using Core.Managers.Menu.Items;

using KeyPress;

internal class MoveComboModeMenu : KeyPressModeMenu, IComboModeMenu
{
    private readonly MenuAbilityToggler comboAbilityToggler;

    private readonly MenuAbilityToggler comboItemsToggler;

    public MoveComboModeMenu(Menu rootMenu, string displayName)
        : base(rootMenu, displayName)
    {
        this.SettingsMenu = this.Menu.Add(new Menu("Settings", "comboSettings" + this.SimplifiedName));
        this.SettingsMenu.AddTranslation(Lang.Ru, "Настройки");
        this.SettingsMenu.AddTranslation(Lang.Cn, "设置");

        this.IgnoreInvisibility = this.SettingsMenu.Add(
            new MenuSwitcher("Ignore invisibility", "comboInvis" + this.SimplifiedName, false, true).SetTooltip(
                "Use abilities when hero is invisible"));
        this.IgnoreInvisibility.AddTranslation(Lang.Ru, "Игнорировать инвиз");
        this.IgnoreInvisibility.AddTooltipTranslation(Lang.Ru, "Использовать способности когда герой невидимый");
        this.IgnoreInvisibility.AddTranslation(Lang.Cn, "忽略隐身");
        this.IgnoreInvisibility.AddTooltipTranslation(Lang.Cn, "英雄不可见时使用技能");

        this.IgnoreInvisibilityIfVisible = this.SettingsMenu.Add(
            new MenuSwitcher("Ignore invisibility if visible", "comboInvisIfVisible" + this.SimplifiedName, false, true).SetTooltip(
                "Use abilities when the hero is invisible but you are visible"));
        this.IgnoreInvisibilityIfVisible.AddTranslation(Lang.Ru, "Игнорировать инвиз если видна");
        this.IgnoreInvisibilityIfVisible.AddTooltipTranslation(Lang.Ru, "Использовать способности когда герой невидимый но вась видно");
        this.IgnoreInvisibilityIfVisible.AddTranslation(Lang.Cn, "如果可见则忽略不可见");
        this.IgnoreInvisibilityIfVisible.AddTooltipTranslation(Lang.Cn, "当英雄隐形但你可见时使用技能");

        this.comboAbilityToggler =
            this.Menu.Add(new MenuAbilityToggler("Abilities", "abilities" + this.SimplifiedName, null, true, true));
        this.comboAbilityToggler.AddTranslation(Lang.Ru, "Способности");
        this.comboAbilityToggler.AddTranslation(Lang.Cn, "技能");

        this.comboItemsToggler = this.Menu.Add(new MenuAbilityToggler("Items", "items" + this.SimplifiedName, null, true, true));
        this.comboItemsToggler.AddTranslation(Lang.Ru, "Предметы");
        this.comboItemsToggler.AddTranslation(Lang.Cn, "物品");
    }

    public MenuSwitcher IgnoreInvisibility { get; private set; }

    public MenuSwitcher IgnoreInvisibilityIfVisible { get; private set; }

    protected Menu SettingsMenu { get; }

    public void AddComboAbility(UsableAbility ability)
    {
        if (ability.Ability.IsItem)
        {
            this.AddComboItem(ability);
            return;
        }

        this.comboAbilityToggler.AddAbility(ability.Ability.Id);
    }

    public T GetAbilitySettingsMenu<T>(UsableAbility ability)
        where T : UsableAbilityMenu
    {
        return null;
    }

    public bool IsAbilityEnabled(IActiveAbility ability)
    {
        if (ability.IsItem)
        {
            return this.comboItemsToggler.IsEnabled(ability.Name);
        }

        return this.comboAbilityToggler.IsEnabled(ability.Name);
    }

    private void AddComboItem(UsableAbility ability)
    {
        this.comboItemsToggler.AddAbility(ability.Ability.Id);
    }
}