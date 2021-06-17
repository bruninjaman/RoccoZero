using System.Linq;
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
using O9K.AIO.Abilities;
using O9K.AIO.Heroes.Base;
using O9K.AIO.Modes.Permanent;
using O9K.Core.Entities.Abilities.Heroes.Invoker;
using O9K.Core.Helpers;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Heroes.Invoker.Modes
{
    internal class AutoGhostWalkMode : PermanentMode
    {
        private readonly AutoGhostWalkModeMenu modeMenu;
        private MenuSlider HpSlider => modeMenu.hpSlider;
        private readonly Sleeper sleeper = new Sleeper();

        public AutoGhostWalkMode(BaseHero baseHero, AutoGhostWalkModeMenu menu)
            : base(baseHero, menu)
        {
            modeMenu = menu;
        }


        protected override void Execute()
        {
            if (sleeper.IsSleeping)
                return;
            var ghostWalk = Owner.Hero.Abilities.FirstOrDefault(x => x.Id == AbilityId.invoker_ghost_walk) as GhostWalk;
            var wex = Owner.Hero.Abilities.FirstOrDefault(x => x.Id == AbilityId.invoker_wex);
            if (ghostWalk is not null && ghostWalk.CanBeCasted() && (ghostWalk.CanBeInvoked || ghostWalk.IsInvoked))
            {
                if (Owner.Hero.HealthPercentage < HpSlider)
                {
                    if (!ghostWalk.IsInvoked)
                    {
                        if (!ghostWalk.Invoke())
                            return;
                    }

                    if (wex != null)
                    {
                        // wex.BaseAbility.Cast();
                        // wex.BaseAbility.Cast();
                        // wex.BaseAbility.Cast();
                    }
                    
                    ghostWalk.BaseAbility.Cast();
                    sleeper.Sleep(0.500f);
                }
            }
            
            sleeper.Sleep(0.15f);
        }
    }
}