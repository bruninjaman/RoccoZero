namespace O9K.AutoUsage.Abilities.HealthRestore.Unique.Bottle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base.Types;
    using Core.Entities.Metadata;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Prediction.Data;

    using Divine.Update;
    using Divine.Entity.Entities.Abilities.Components;

    using ManaRestore;

    using Settings;

    [AbilityId(AbilityId.item_bottle)]
    internal class BottleManaAbility : ManaRestoreAbility
    {
        private readonly Sleeper bottleRefillingSleeper = new Sleeper();

        private readonly ManaRestoreSettings settings;

        public BottleManaAbility(IManaRestore manaRestore, GroupSettings settings)
            : base(manaRestore)
        {
            this.settings = new ManaRestoreSettings(settings.Menu, manaRestore);
        }

        public override bool UseAbility(List<Unit9> heroes)
        {
            var allies = heroes.Where(x => !x.IsInvulnerable && x.IsAlly(this.Owner)).OrderBy(x => x.Mana).ToList();

            foreach (var ally in allies)
            {
                if (!this.settings.IsHeroEnabled(ally.Name) && !this.settings.SelfOnly)
                {
                    continue;
                }

                var selfTarget = ally.Equals(this.Owner);

                if (selfTarget && !this.ManaRestore.RestoresOwner)
                {
                    continue;
                }

                if (!selfTarget && (!this.ManaRestore.RestoresAlly || this.settings.SelfOnly))
                {
                    continue;
                }

                if (!this.Ability.CanHit(ally, allies, this.settings.AlliesCount))
                {
                    continue;
                }

                if (ally.ManaPercentage > this.settings.MpThreshold)
                {
                    continue;
                }

                if (this.Owner.HealthPercentage < this.settings.HpThreshold)
                {
                    continue;
                }
                
                
                if (ally.HasModifier("modifier_bottle_regeneration"))
                {
                    continue;
                }

                return this.Ability.UseAbility(ally, allies, HitChance.Medium, this.settings.AlliesCount);
            }

            return false;
        }
    }
}