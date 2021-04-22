namespace O9K.Hud.Modules.Particles.Units
{
    using System;
    using System.Collections.Generic;

    using Core.Entities.Units;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.EventArgs;
    using Core.Managers.Menu.Items;

    using Divine;

    using MainMenu;

    using SharpDX;

    internal class Illusion : IHudModule
    {
        private readonly MenuSwitcher show;

        private Team ownerTeam;

        private readonly HashSet<Unit9> illusions = new HashSet<Unit9>();

        public Illusion(IHudMenu hudMenu)
        {
            this.show = hudMenu.ParticlesMenu.Add(new MenuSwitcher("Illusion", "illusion"));
            this.show.AddTranslation(Lang.Ru, "Иллюзии");
            this.show.AddTranslation(Lang.Cn, "幻象");
        }

        public void Activate()
        {
            this.ownerTeam = EntityManager9.Owner.Team;
            this.show.ValueChange += this.ShowOnValueChanging;
        }

        public void Dispose()
        {
            this.show.ValueChange -= this.ShowOnValueChanging;
            EntityManager9.UnitAdded -= this.OnUnitAdded;
            EntityManager9.UnitRemoved -= this.OnUnitRemoved;
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
        }

        private void OnUnitAdded(Unit9 entity)
        {
            try
            {
                if (!entity.IsIllusion || entity.Team == this.ownerTeam || entity.CanUseAbilities)
                {
                    return;
                }

                if ((entity.UnitState & (UnitState.Unselectable | UnitState.CommandRestricted))
                    == (UnitState.Unselectable | UnitState.CommandRestricted))
                {
                    return;
                }

                this.illusions.Add(entity);
                entity.BaseEntity.ColorTint = new Color(0, 0, 255, 255);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitRemoved(Unit9 entity)
        {
            this.illusions.Remove(entity);
        }

        private void OnUpdate()
        {
            foreach (var unit in this.illusions)
            {
                if (!unit.IsValid)
                {
                    continue;
                }

                unit.BaseEntity.ColorTint = new Color(0, 0, 255, 255);
            }
        }

        private void ShowOnValueChanging(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.UnitAdded += this.OnUnitAdded;
                EntityManager9.UnitRemoved += this.OnUnitRemoved;
                UpdateManager.CreateIngameUpdate(300, this.OnUpdate);
            }
            else
            {
                EntityManager9.UnitAdded -= this.OnUnitAdded;
                EntityManager9.UnitRemoved -= this.OnUnitRemoved;
                UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            }
        }
    }
}