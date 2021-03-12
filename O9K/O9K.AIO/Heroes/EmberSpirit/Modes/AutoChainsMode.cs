namespace O9K.AIO.Heroes.EmberSpirit.Modes
{
    using System;
    using System.Linq;

    using AIO.Modes.Permanent;

    using Base;

    using Core.Entities.Abilities.Base;
    using Core.Logger;

    using Divine;

    internal class AutoChainsMode : PermanentMode
    {
        private readonly AutoChainsModeMenu menu;

        private bool useChains;

        public AutoChainsMode(BaseHero baseHero, AutoChainsModeMenu menu)
            : base(baseHero, menu)
        {
            this.menu = menu;
            OrderManager.OrderAdding += OnOrderAdding;
        }

        public override void Dispose()
        {
            base.Dispose();
            OrderManager.OrderAdding -= OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
        }

        protected override void Execute()
        {
            if (!this.useChains)
            {
                return;
            }

            var hero = this.Owner.Hero;

            if (!hero.IsValid || !hero.IsAlive)
            {
                return;
            }

            var chains = hero.Abilities.FirstOrDefault(x => x.Id == AbilityId.ember_spirit_searing_chains) as ActiveAbility;
            if (chains?.IsValid != true || !chains.CanBeCasted())
            {
                return;
            }

            var enemy = this.TargetManager.EnemyHeroes.Any(x => chains.CanHit(x));
            if (!enemy)
            {
                return;
            }

            if (this.menu.fistKey)
            {
                chains.UseAbility();
            }

            this.useChains = false;
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                if (e.IsCustom || !e.Process)
                {
                    return;
                }

                var order = e.Order;
                if (order.Type != OrderType.CastPosition)
                {
                    return;
                }

                if (order.Ability.Id == AbilityId.ember_spirit_sleight_of_fist)
                {
                    ModifierManager.ModifierAdded += this.OnModifierAdded;
                    ModifierManager.ModifierRemoved += this.OnModifierRemoved;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_ember_spirit_sleight_of_fist_caster")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Owner.HeroHandle)
                    {
                        return;
                    }

                    this.useChains = true;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Name != "modifier_ember_spirit_sleight_of_fist_caster")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                try
                {
                    if (modifier.Owner.Handle != this.Owner.HeroHandle)
                    {
                        return;
                    }

                    ModifierManager.ModifierAdded -= this.OnModifierAdded;
                    ModifierManager.ModifierRemoved -= this.OnModifierRemoved;
                    this.useChains = false;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            });
        }
    }
}