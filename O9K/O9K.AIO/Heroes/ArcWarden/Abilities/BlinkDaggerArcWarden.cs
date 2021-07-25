using Divine.Entity.Entities.Abilities.Components;
using Divine.Modifier.Modifiers;
using O9K.Core.Entities.Abilities.Heroes.ArcWarden;
using O9K.Core.Extensions;
using O9K.Core.Managers.Entity;

namespace O9K.AIO.Heroes.ArcWarden.Abilities
{
    using System;
    using System.Collections.Generic;
    using AIO.Abilities;
    using AIO.Modes.Combo;
    using Core.Entities.Abilities.Base;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using O9K.AIO.Abilities.Menus;
    using O9K.AIO.Heroes.Pudge.Abilities;
    using O9K.Core.Helpers;
    using TargetManager;

    internal class BlinkDaggerArcWarden : BlinkAbility
    {
        public BlinkDaggerArcWarden(ActiveAbility ability)
            : base(ability)
        {
        }

        public override bool UseAbility(TargetManager targetManager, Sleeper comboSleeper, bool aoe)
        {
            var target = targetManager.Target;
            var distance = target.Distance(this.Owner);
            var delay = this.Ability.GetCastDelay(target);


            var isBuffBlink = this.Ability.Id is AbilityId.item_swift_blink or AbilityId.item_arcane_blink;

            var isDamageBlink = this.Ability.Id is AbilityId.item_overwhelming_blink;

            var isMainHero = (this.Owner == EntityManager9.Owner);

            if (isMainHero)
            {
                if (!isBuffBlink && !isDamageBlink)
                {
                    return false;
                }

                if (isDamageBlink && distance > 750)
                {
                    return false;
                }

                var samePosition = this.Owner.Position;

                
                if (!this.Ability.UseAbility(samePosition))
                {
                    return false;
                }

                comboSleeper.Sleep(delay);
                this.Sleeper.Sleep(delay + 0.5f);
                this.OrbwalkSleeper.Sleep(delay);
                return true;
            }

            var hasModifierShield = this.Owner.HasModifier("modifier_arc_warden_magnetic_field_evasion");
            
            if (hasModifierShield)

            {
                return false;
            }
            
            if (distance > 1800)
            {
                return false;
            }

            var position = distance > this.Ability.CastRange
                ? this.Owner.Position.Extend2D(target.Position, this.Ability.CastRange)
                : target.GetPredictedPosition(this.Ability.CastPoint + 0.3f);

            

            if (!this.Ability.UseAbility(position))
            {
                return false;
            }

            comboSleeper.Sleep(delay);
            this.Sleeper.Sleep(delay + 0.5f);
            this.OrbwalkSleeper.Sleep(delay);
            return true;
        }
    }
}