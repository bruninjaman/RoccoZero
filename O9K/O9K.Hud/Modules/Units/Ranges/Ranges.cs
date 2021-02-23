namespace O9K.Hud.Modules.Units.Ranges
{
    using System;
    using System.Collections.Generic;

    using Abilities;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;

    using MainMenu;

    internal class Ranges : IHudModule
    {
        private readonly Menu menu;

        private readonly List<RangeUnit> rangeUnits = new List<RangeUnit>();

        public Ranges(IHudMenu hudMenu)
        {
            this.menu = hudMenu.UnitsMenu.Add(new Menu("Ranges"));
            this.menu.AddTranslation(Lang.Ru, "Радиус способностей");
            this.menu.AddTranslation(Lang.Cn, "范围");
        }

        public void Activate()
        {
            RendererManager.LoadTexture("o9k.attack", @"panorama\images\hud\reborn\ping_icon_attack_psd.vtex_c");
            RendererManager.LoadTexture("o9k.exp_plus", @"panorama\images\hud\reborn\levelup_plus_fill_psd.vtex_c");

            EntityManager9.UnitAdded += this.OnUnitAdded;
            EntityManager9.UnitRemoved += this.OnUnitRemoved;
            EntityManager9.AbilityAdded += this.OnAbilityAdded;
            EntityManager9.AbilityRemoved += this.OnAbilityRemoved;
            UpdateManager.CreateIngameUpdate(3000, this.OnUpdate);
        }

        public void Dispose()
        {
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            EntityManager9.AbilityAdded -= this.OnAbilityAdded;
            EntityManager9.AbilityRemoved -= this.OnAbilityRemoved;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
        }

        private void OnAbilityAdded(Ability9 ability)
        {
            try
            {
                if (ability.IsTalent || ability.IsStolen)
                {
                    return;
                }

                var rangeUnit = this.rangeUnits.Find(x => x.Handle == ability.Owner.Handle);
                if (rangeUnit == null)
                {
                    return;
                }

                rangeUnit.AddAbility(ability);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAbilityRemoved(Ability9 ability)
        {
            try
            {
                if (ability.IsTalent || ability.IsStolen)
                {
                    return;
                }

                var rangeUnit = this.rangeUnits.Find(x => x.Handle == ability.Owner.Handle);
                if (rangeUnit == null)
                {
                    return;
                }

                rangeUnit.RemoveAbility(ability);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitAdded(Unit9 unit)
        {
            if (!unit.IsHero || unit.IsIllusion)
            {
                return;
            }

            this.rangeUnits.Add(new RangeUnit(unit, this.menu));
        }

        private void OnUnitRemoved(Unit9 unit)
        {
            if (!unit.IsHero || unit.IsIllusion)
            {
                return;
            }

            var rangeUnit = this.rangeUnits.Find(x => x.Handle == unit.Handle);
            if (rangeUnit == null)
            {
                return;
            }

            rangeUnit.Dispose();
            this.rangeUnits.Remove(rangeUnit);
        }

        private void OnUpdate()
        {
            try
            {
                foreach (var unit in this.rangeUnits)
                {
                    unit.UpdateRanges();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}