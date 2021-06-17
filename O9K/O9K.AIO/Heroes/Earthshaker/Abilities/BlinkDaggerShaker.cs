namespace O9K.AIO.Heroes.Earthshaker.Abilities
{
    using System.Collections.Generic;

    using AIO.Abilities;
    using AIO.Modes.Combo;

    using Core.Entities.Abilities.Base;
    using Core.Helpers;
    using Core.Prediction.Data;

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

    using Divine.Numerics;

    using TargetManager;

    internal class BlinkDaggerShaker : BlinkAbility
    {
        private Vector3 blinkPosition;

        public BlinkDaggerShaker(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool ShouldConditionCast(TargetManager targetManager, IComboModeMenu menu, List<UsableAbility> usableAbilities)
        {
            var totem = usableAbilities.Find(x => x.Ability.Id == AbilityId.earthshaker_enchant_totem);
            if (totem != null)
            {
                var inputTotem = totem.Ability.GetPredictionInput(targetManager.Target, targetManager.EnemyHeroes);
                inputTotem.Range += this.Ability.CastRange;
                inputTotem.CastRange = this.Ability.CastRange;
                inputTotem.SkillShotType = SkillShotType.Circle;

                if (this.Owner.HasModifier("modifier_earthshaker_enchant_totem"))
                {
                    inputTotem.AreaOfEffect = false;
                    inputTotem.Delay -= 0.1f;
                }

                var outputTotem = totem.Ability.GetPredictionOutput(inputTotem);
                if (outputTotem.HitChance < HitChance.Low)
                {
                    return false;
                }

                this.blinkPosition = outputTotem.CastPosition;
                if (this.Owner.Distance(this.blinkPosition) > this.Ability.CastRange
                    || this.Owner.Distance(this.blinkPosition) < totem.Ability.Radius - 50)
                {
                    return false;
                }

                return true;
            }

            if (!(usableAbilities.Find(x => x.Ability.Id == AbilityId.earthshaker_echo_slam) is EchoSlam echo))
            {
                return false;
            }

            var input = echo.Ability.GetPredictionInput(targetManager.Target, targetManager.EnemyHeroes);
            input.Range += this.Ability.CastRange;
            input.CastRange = this.Ability.CastRange;
            input.SkillShotType = SkillShotType.Circle;
            var output = echo.Ability.GetPredictionOutput(input);
            if (output.HitChance < HitChance.Low || output.AoeTargetsHit.Count < echo.TargetsToHit(menu))
            {
                return false;
            }

            this.blinkPosition = output.CastPosition;
            if (this.Owner.Distance(this.blinkPosition) > this.Ability.CastRange
                || output.AoeTargetsHit.Count == 1 && this.Owner.Distance(this.blinkPosition) < 350)
            {
                return false;
            }

            return true;
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            if (!this.Ability.UseAbility(this.blinkPosition))
            {
                return false;
            }

            comboSleeper.Sleep(0.3f);
            this.OrbwalkSleeper.Sleep(0.5f);
            this.Sleeper.Sleep(this.Ability.GetCastDelay(targetManager.Target) + 0.5f);
            return true;
        }
    }
}