namespace Debugger.Tools.Cheats
{
    using System;
    using System.ComponentModel.Composition;
    using System.Linq;
    using System.Windows.Input;

    using Debugger.Menus;

    using Divine;
    using Divine.Menu.EventArgs;
    using Divine.Menu.Items;
    using Divine.SDK.Managers.Update;

    internal class CreateUnits : IDebuggerTool
    {
        private readonly Random random = new();

        private readonly IMainMenu mainMenu;

        private MenuHoldKey meleeAllyCreep;

        private MenuHoldKey meleeEnemyCreep;

        private Menu menu;

        private MenuHoldKey randomAlly;

        private MenuHoldKey randomEnemy;

        private MenuHoldKey rangedAllyCreep;

        private MenuHoldKey rangedEnemyCreep;

        public CreateUnits(IMainMenu mainMenu)
        {
            this.mainMenu = mainMenu;
        }

        public int LoadPriority { get; } = 5;

        public void Activate()
        {
            this.menu = this.mainMenu.CheatsMenu.CreateMenu("Create unit");

            this.randomAlly = this.menu.CreateHoldKey("Random ally hero", Key.NumPad3);
            this.randomAlly.ValueChanged += this.RandomAllyOnPropertyChanged;

            this.meleeAllyCreep = this.menu.CreateHoldKey("Melee ally creep", Key.NumPad8);
            this.meleeAllyCreep.ValueChanged += this.MeleeAllyCreepOnPropertyChanged;

            this.rangedAllyCreep = this.menu.CreateHoldKey("Ranged ally creep", Key.NumPad9);
            this.rangedAllyCreep.ValueChanged += this.RangedAllyCreepOnPropertyChanged;

            this.randomEnemy = this.menu.CreateHoldKey("Random enemy hero", Key.NumPad4);
            this.randomEnemy.ValueChanged += this.RandomEnemyOnPropertyChanged;

            this.meleeEnemyCreep = this.menu.CreateHoldKey("Melee enemy creep", Key.NumPad5);
            this.meleeEnemyCreep.ValueChanged += this.MeleeEnemyCreepOnPropertyChanged;

            this.rangedEnemyCreep = this.menu.CreateHoldKey("Ranged enemy creep", Key.NumPad6);
            this.rangedEnemyCreep.ValueChanged += this.RangedEnemyCreepOnPropertyChanged;
        }

        public void Dispose()
        {
            this.randomAlly.ValueChanged -= this.RandomAllyOnPropertyChanged;
            this.meleeAllyCreep.ValueChanged -= this.MeleeAllyCreepOnPropertyChanged;
            this.rangedAllyCreep.ValueChanged -= this.RangedAllyCreepOnPropertyChanged;
            this.randomEnemy.ValueChanged -= this.RandomEnemyOnPropertyChanged;
            this.meleeEnemyCreep.ValueChanged -= this.MeleeEnemyCreepOnPropertyChanged;
            this.rangedEnemyCreep.ValueChanged -= this.RangedEnemyCreepOnPropertyChanged;
        }

        private string GetRandomHero()
        {
            var alreadyAdded = EntityManager.GetEntities<Hero>().Select(x => x.HeroId);
            var heroes = Enum.GetValues(typeof(HeroId)).Cast<HeroId>().Except(alreadyAdded).ToList();
            var randomHero = heroes[this.random.Next(1, heroes.Count - 1)];

            return randomHero.ToString();
        }

        private void MeleeAllyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.meleeAllyCreep)
                {
                    GameManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee");
                }
            });
        }

        private void MeleeEnemyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.meleeEnemyCreep)
                {
                    GameManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_melee enemy");
                }
            });
        }

        private void RandomAllyOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.randomAlly)
                {
                    GameManager.ExecuteCommand("dota_create_unit " + this.GetRandomHero());
                }
            });
        }

        private void RandomEnemyOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.randomEnemy)
                {
                    GameManager.ExecuteCommand("dota_create_unit " + this.GetRandomHero() + " enemy");
                }
            });
        }

        private void RangedAllyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.rangedAllyCreep)
                {
                    GameManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged");
                }
            });
        }

        private void RangedEnemyCreepOnPropertyChanged(MenuHoldKey holdKey, HoldKeyEventArgs e)
        {
            UpdateManager.BeginInvoke(() =>
            {
                if (this.rangedEnemyCreep)
                {
                    GameManager.ExecuteCommand("dota_create_unit npc_dota_creep_goodguys_ranged enemy");
                }
            });
        }
    }
}