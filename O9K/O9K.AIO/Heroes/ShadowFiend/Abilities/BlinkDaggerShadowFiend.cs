using O9K.AIO.Heroes.ShadowFiend.Abilities;
using O9K.Core.Managers.Menu.Items;

namespace O9K.AIO.Heroes.ShadowFiend.Ability
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

    internal class BlinkDaggerShadowFiend : BlinkAbility
    {

        public BlinkDaggerShadowFiend(ActiveAbility ability)
            : base(ability)
        {
        }

        public override UsableAbilityMenu GetAbilityMenu(string simplifiedName)
        {
            return new BlinkDaggerShadowFiendMenu(this.Ability, "Dont use eul in combo");
        }
    }
}