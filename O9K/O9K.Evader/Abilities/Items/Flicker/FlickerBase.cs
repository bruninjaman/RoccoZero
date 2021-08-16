﻿namespace O9K.Evader.Abilities.Items.Flicker
{
    using Base;
    using Base.Usable.BlinkAbility;
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    [AbilityId(AbilityId.item_flicker)]
    internal class FlickerBase : EvaderBaseAbility, IUsable<BlinkAbility>, IUsable<CounterAbility>
    {
        public FlickerBase(Ability9 ability)
            : base(ability)
        {
        }

        public BlinkAbility GetUsableAbility()
        {
            return new BlinkAbility(this.Ability, this.Pathfinder, this.Menu);
        }

        CounterAbility IUsable<CounterAbility>.GetUsableAbility()
        {
            return new CounterAbility(this.Ability, this.Menu);
        }
    }
}