namespace O9K.Hud.Modules.Particles.Units
{
    using System;

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

                /*var effect = ParticleManager.CreateParticle(
                    "materials/ensage_ui/particles/illusions_mod_v2.vpcf",
                    ParticleAttachment.CenterFollow,
                    entity.BaseUnit);

                effect.SetControlPoint(1, new Vector3(255));
                effect.SetControlPoint(2, new Vector3(65, 105, 255));

                effect.Release();*/

                entity.BaseEntity.ColorTint = new Color(0, 0, 255, 255);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void ShowOnValueChanging(object sender, SwitcherEventArgs e)
        {
            if (e.NewValue)
            {
                EntityManager9.UnitAdded += this.OnUnitAdded;
            }
            else
            {
                EntityManager9.UnitAdded -= this.OnUnitAdded;
            }
        }
    }
}