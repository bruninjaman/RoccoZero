﻿using Divine;
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
using O9K.AIO.Heroes.Base;
using O9K.AIO.Heroes.Invoker.Modes;
using O9K.AIO.Modes.Permanent;
using O9K.Core.Entities.Metadata;

namespace O9K.AIO.Heroes.Invoker
{
    [HeroId(HeroId.npc_dota_hero_invoker)]
    internal class InvokerBase : BaseHero
    {
        private readonly SmartSpheresMode smartSpheresMode;
        private readonly AutoGhostWalkMode autoGhostWalkMode;
        private readonly AutoSunStrikeMode autoSunStrikeMode;

        public InvokerBase()
        {
            smartSpheresMode = new SmartSpheresMode(this, new SmartSpheresModeModeMenu(Menu.RootMenu, "Smart spheres"));
            autoGhostWalkMode = new AutoGhostWalkMode(this, new AutoGhostWalkModeMenu(Menu.RootMenu, "Auto ghostWalk"));
            autoSunStrikeMode = new AutoSunStrikeMode(this, new AutoSunStrikeModeMenu(Menu.RootMenu, "Auto sunStrike"));
        }

        public override void Dispose()
        {
            smartSpheresMode.Disable();
            autoGhostWalkMode.Disable();
            autoSunStrikeMode.Disable();
            base.Dispose();
        }
        
        protected override void DisableCustomModes()
        {
            smartSpheresMode.Disable();
            autoGhostWalkMode.Disable();
            autoSunStrikeMode.Disable();
        }

        protected override void EnableCustomModes()
        {
            smartSpheresMode.Enable();
            autoGhostWalkMode.Enable();
            autoSunStrikeMode.Enable();
        }
    }
}