using System.Collections.Generic;
using EnsageSharp.Sandbox;
using Newtonsoft.Json;

namespace BAIO
{
    using System;
    using Ensage;
    using Ensage.Common.Menu;
    using Ensage.SDK.Menu;

    public class Config : IDisposable
    {
        private bool disposed;

        public Config(HeroId id)
        {
            this.Factory = MenuFactory.Create("BAIO");
            this.General = new GeneralMenu(this.Factory);
            this.Hero = new HeroMenu(this.Factory, id.ToString());
        }

        public MenuFactory Factory { get; }

        public GeneralMenu General { get; set; }

        public HeroMenu Hero { get; set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.Factory.Dispose();
            }

            this.disposed = true;
        }

        public class GeneralMenu : IDisposable
        {
            private bool disposed;

            public GeneralMenu(MenuFactory factory)
            {
                this.Factory = factory.Menu("General");

                this.Enabled = this.Factory.Item("Enabled", true);

                this.ComboKey = this.Factory.Item("Combo Key", new KeyBind(32));

                this.Killsteal = this.Factory.Item("Killsteal", true);
                this.Killsteal.Item.Tooltip = "Enable Killsteal";

                this.DrawTargetIndicator = this.Factory.Item("Draw Target Indicator", true);
                this.DrawTargetIndicator.Item.Tooltip = "Draws Target Indicator on top of the target";

                this.LockTarget = this.Factory.Item("Lock Target", true);
                this.LockTarget.Item.Tooltip = "Keep your target until it's dead or you release the key";

                this.KiteMode = this.Factory.Item("Kite mode", true);
                this.KiteMode.Item.Tooltip = "Only try to attack when in attack range";
            }

            public MenuItem<KeyBind> ComboKey { get; }

            public MenuItem<bool> DrawTargetIndicator { get; }

            public MenuFactory Factory { get; }

            public MenuItem<bool> Killsteal { get; }

            public MenuItem<bool> KiteMode { get; }

            public MenuItem<bool> LockTarget { get; }

            public MenuItem<bool> Enabled { get; }

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                {
                    return;
                }

                if (disposing)
                {
                    this.Factory.Dispose();
                }

                this.disposed = true;
            }
        }

        public class HeroMenu : IDisposable
        {
            private bool disposed;
            public Dictionary<int, string> Translations { get; set; }

            public HeroMenu(MenuFactory factory, string heroName)
            {
                var lang = SandboxConfig.Language;
                if (lang == "zh-Hans" || lang == "zh-Hant")
                {
                    Translations = JsonConvert.DeserializeObject<Dictionary<int, string>>(Resource1.cn);
                }
                else if (lang == "bg")
                {
                    Translations = JsonConvert.DeserializeObject<Dictionary<int, string>>(Resource1.ru);
                }
                else
                {
                    Translations = JsonConvert.DeserializeObject<Dictionary<int, string>>(Resource1.en);
                }

                this.Factory = factory.MenuWithTexture(Game.Localize(heroName), heroName);
                this.ItemMenu = Factory.Menu("Items", "items");
                this.LinkenBreakerMenu = ItemMenu.Menu("Linken Breakers", "linkenbreakers");
                this.LinkenBreakerPriorityMenu = LinkenBreakerMenu.Item("Priority", new PriorityChanger(LinkenAbilityPriorityList));
                this.LinkenBreakerTogglerMenu = LinkenBreakerMenu.Item("Toggler", new AbilityToggler(LinkenAbilityTogglerDic));

                this.UnitController = Factory.MenuWithTexture(Translations[2], "npc_dota_neutral_centaur_khan");
                this.Bodyblocker = this.UnitController.MenuWithTexture(Translations[3], "earthshaker_fissure");
                var ucMenu = this.UnitController;
                var bb = this.Bodyblocker;
                this.ControlUnits = ucMenu.Item(Translations[4], true);
                this.Enabled = bb.Item(Translations[5], true);
                this.UseUnitAbilities = ucMenu.Item(Translations[6], true);
                this.BlockSensitivity = bb.Item(Translations[7], new Slider(150, 50, 300));
                this.BlockSensitivity.Item.Tooltip = Translations[8];
            }

            public MenuFactory Factory { get; }
            public MenuFactory ItemMenu { get; }
            public MenuFactory LinkenBreakerMenu { get; }
            public MenuFactory UnitController;
            public MenuFactory Bodyblocker;

            public MenuItem<PriorityChanger> LinkenBreakerPriorityMenu;
            public MenuItem<AbilityToggler> LinkenBreakerTogglerMenu;

            public MenuItem<bool> ControlUnits;
            public MenuItem<bool> Enabled;
            public MenuItem<Slider> BlockSensitivity;
            public MenuItem<bool> UseUnitAbilities;


            public Dictionary<string, bool> LinkenAbilityTogglerDic = new Dictionary<string, bool>
        {
                { "item_sheepstick", true},
                { "item_rod_of_atos", true},
                { "item_nullifier", true },
                { "item_bloodthorn", true },
                { "item_orchid", true },
                { "item_cyclone", true },
                { "item_force_staff", true },
                { "item_diffusal_blade", true }
        };

            public List<string> LinkenAbilityPriorityList = new List<string>
        {
                { "item_sheepstick" },
                { "item_rod_of_atos" },
                { "item_nullifier" },
                { "item_bloodthorn" },
                { "item_orchid" },
                { "item_cyclone" },
                { "item_force_staff" },
                { "item_diffusal_blade" }
        };

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
                if (this.disposed)
                {
                    return;
                }

                if (disposing)
                {
                    this.Factory.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}