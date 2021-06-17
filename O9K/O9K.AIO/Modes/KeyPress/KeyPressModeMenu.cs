namespace O9K.AIO.Modes.KeyPress
{
    using System.Collections.Generic;

    using Base;

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
    using Divine.Helpers;

    internal class KeyPressModeMenu : BaseModeMenu
    {
        private readonly Dictionary<string, string> cnLoc = new Dictionary<string, string>
        {
            { "Toss enemy to ally tower", LocalizationHelper.LocalizeName(AbilityId.tiny_toss) + "敌人到防御塔" },
            {
                "Use ball lighting to charge overload",
                "使用" + LocalizationHelper.LocalizeName(AbilityId.storm_spirit_ball_lightning) + "对于"
                + LocalizationHelper.LocalizeName(AbilityId.storm_spirit_overload)
            },
        };

        private readonly Dictionary<string, string> ruLoc = new Dictionary<string, string>
        {
            { "Toss enemy to ally tower", "Тосс врага к союзной вышке" },
            { "Use ball lighting to charge overload", "Использовать ульту для заряда оверлорда" },
        };

        public KeyPressModeMenu(Menu rootMenu, string displayName, string tooltip = null)
            : base(rootMenu, displayName)
        {
            this.Key = new MenuHoldKey("Key", "key" + this.SimplifiedName, System.Windows.Input.Key.None, true);
            this.Key.AddTranslation(Lang.Ru, "Клавиша");
            this.Key.AddTranslation(Lang.Cn, "键");

            if (tooltip != null)
            {
                this.Key.SetTooltip(tooltip);

                if (this.ruLoc.TryGetValue(tooltip, out var loc))
                {
                    this.Key.AddTooltipTranslation(Lang.Ru, loc);
                }

                if (this.cnLoc.TryGetValue(tooltip, out loc))
                {
                    this.Key.AddTooltipTranslation(Lang.Cn, loc);
                }
            }

            this.Menu.Add(this.Key);
            rootMenu.Add(this.Menu);
        }

        public MenuHoldKey Key { get; }
    }
}