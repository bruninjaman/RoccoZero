namespace O9K.AIO.Modes.Combo
{
    using System.Collections.Generic;

    using Abilities;
    using Abilities.Menus;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.Camera;
    using Divine.Entity;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.GameConsole;

    using Divine.Input;
    using Divine.Log;
    using Divine.Map;

    using Divine.Modifier;
    using Divine.Numerics;
    using Divine.Orbwalker;
    using Divine.Order;
    using Divine.Particle;
    using Divine.Projectile;
    using Divine.Renderer;
    using Divine.Service;
    using Divine.Update;
    using Divine.Entity.Entities;
    using Divine.Entity.EventArgs;
    using Divine.Game.EventArgs;
    using Divine.GameConsole.Exceptions;
    using Divine.Input.EventArgs;
    using Divine.Map.Components;
    using Divine.Menu.Animations;
    using Divine.Menu.Components;

    using Divine.Menu.Helpers;

    using Divine.Menu.Styles;
    using Divine.Modifier.EventArgs;
    using Divine.Modifier.Modifiers;
    using Divine.Order.EventArgs;
    using Divine.Order.Orders;
    using Divine.Particle.Components;
    using Divine.Particle.EventArgs;
    using Divine.Particle.Particles;
    using Divine.Plugins.Humanizer;
    using Divine.Projectile.EventArgs;
    using Divine.Projectile.Projectiles;
    using Divine.Renderer.ValveTexture;
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Exceptions;
    using Divine.Entity.Entities.PhysicalItems;
    using Divine.Entity.Entities.Players;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Modifier.Modifiers.Components;
    using Divine.Modifier.Modifiers.Exceptions;
    using Divine.Order.Orders.Components;
    using Divine.Particle.Particles.Exceptions;
    using Divine.Projectile.Projectiles.Components;
    using Divine.Projectile.Projectiles.Exceptions;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Abilities.Spells;
    using Divine.Entity.Entities.Players.Components;
    using Divine.Entity.Entities.Runes.Components;
    using Divine.Entity.Entities.Units.Buildings;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Entity.Entities.Units.Creeps;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Entity.Entities.Units.Wards;
    using Divine.Entity.Entities.Abilities.Items.Components;
    using Divine.Entity.Entities.Abilities.Items.Neutrals;
    using Divine.Entity.Entities.Abilities.Spells.Abaddon;
    using Divine.Entity.Entities.Abilities.Spells.Components;
    using Divine.Entity.Entities.Units.Creeps.Neutrals;
    using Divine.Entity.Entities.Units.Heroes.Components;

    using KeyPress;

    internal class ComboModeMenu : KeyPressModeMenu, IComboModeMenu
    {
        private readonly Dictionary<AbilityId, UsableAbilityMenu> abilitySettings = new Dictionary<AbilityId, UsableAbilityMenu>();

        private readonly MenuAbilityToggler comboAbilityToggler;

        private readonly MenuAbilityToggler comboItemsToggler;

        public ComboModeMenu(Menu rootMenu, string displayName)
            : base(rootMenu, displayName)
        {
            this.IsHarassCombo = displayName == "Harass";

            this.SettingsMenu = this.Menu.Add(new Menu("Settings", "comboSettings" + this.SimplifiedName));
            this.SettingsMenu.AddTranslation(Lang.Ru, "Настройки");
            this.SettingsMenu.AddTranslation(Lang.Cn, "设置");

            this.Attack = this.SettingsMenu.Add(new MenuSwitcher("Attack", "comboAttack" + this.SimplifiedName, true, true));
            this.Attack.AddTranslation(Lang.Ru, "Атаковать");
            this.Attack.AddTranslation(Lang.Cn, "攻击");

            this.Move = this.SettingsMenu.Add(new MenuSwitcher("Move", "comboMove" + this.SimplifiedName, true, true));
            this.Move.AddTranslation(Lang.Ru, "Двигаться");
            this.Move.AddTranslation(Lang.Cn, "移动");

            this.IgnoreInvisibility = this.SettingsMenu.Add(
                new MenuSwitcher("Ignore invisibility", "comboInvis" + this.SimplifiedName, true, true).SetTooltip(
                    "Use abilities when hero is invisible"));
            this.IgnoreInvisibility.AddTranslation(Lang.Ru, "Игнорировать инвиз");
            this.IgnoreInvisibility.AddTooltipTranslation(Lang.Ru, "Использовать способности когда герой невидимый");
            this.IgnoreInvisibility.AddTranslation(Lang.Cn, "忽略隐身");
            this.IgnoreInvisibility.AddTooltipTranslation(Lang.Cn, "英雄不可见时使用技能");

            this.comboAbilityToggler =
                this.Menu.Add(new MenuAbilityToggler("Abilities", "abilities" + this.SimplifiedName, null, true, true));
            this.comboAbilityToggler.AddTranslation(Lang.Ru, "Способности");
            this.comboAbilityToggler.AddTranslation(Lang.Cn, "技能");

            this.comboItemsToggler = this.Menu.Add(new MenuAbilityToggler("Items", "items" + this.SimplifiedName, null, true, true));
            this.comboItemsToggler.AddTranslation(Lang.Ru, "Предметы");
            this.comboItemsToggler.AddTranslation(Lang.Cn, "物品");
        }

        public MenuSwitcher Attack { get; private set; }

        public MenuSwitcher IgnoreInvisibility { get; private set; }

        public bool IsHarassCombo { get; }

        public MenuSwitcher Move { get; private set; }

        protected Menu SettingsMenu { get; }

        public void AddComboAbility(UsableAbility ability)
        {
            this.AddAbilitySettingsMenu(ability);

            if (ability.Ability.IsItem)
            {
                this.AddComboItem(ability.Ability);
                return;
            }

            this.comboAbilityToggler.AddAbility(ability.Ability.Name);
        }

        public void AddComboAbility(Ability9 ability)
        {
            if (ability.IsItem)
            {
                this.AddComboItem(ability);
                return;
            }

            this.comboAbilityToggler.AddAbility(ability.Name);
        }

        public T GetAbilitySettingsMenu<T>(UsableAbility ability)
            where T : UsableAbilityMenu
        {
            return (T)this.abilitySettings[ability.Ability.Id];
        }

        public bool IsAbilityEnabled(IActiveAbility ability)
        {
            if (ability.IsItem)
            {
                return this.comboItemsToggler.IsEnabled(ability.Name);
            }

            return this.comboAbilityToggler.IsEnabled(ability.Name);
        }

        private void AddAbilitySettingsMenu(UsableAbility ability)
        {
            if (this.abilitySettings.ContainsKey(ability.Ability.Id))
            {
                return;
            }

            var abilityMenu = ability.GetAbilityMenu(this.SimplifiedName);
            if (abilityMenu == null)
            {
                return;
            }

            this.SettingsMenu.Add(abilityMenu.Menu);
            this.abilitySettings.Add(ability.Ability.Id, abilityMenu);
        }

        private void AddComboItem(Ability9 ability)
        {
            this.comboItemsToggler.AddAbility(ability.Name);
        }
    }
}