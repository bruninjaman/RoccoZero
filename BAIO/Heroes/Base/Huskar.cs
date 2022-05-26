using System.Collections.Generic;

namespace BAIO.Heroes.Base
{
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using log4net;
    using Ensage;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_huskar;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Menu;
    using Ensage.Common.Menu;
    using PlaySharp.Toolkit.Helper.Annotations;
    using PlaySharp.Toolkit.Logging;
    using SharpDX;
    using BAIO.Interfaces;
    using BAIO.Modes;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Heroes.Modes.Harass;


    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_huskar)]
    public class Huskar : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        public huskar_inner_fire InnerFire { get; private set; }

        public huskar_burning_spear FireSpear { get; private set; }

        public huskar_life_break Ulti { get; private set; }

        #endregion

        #region Items

        [ItemBinding]
        public item_abyssal_blade AbyssalBlade { get; private set; }

        [ItemBinding]
        public item_manta Manta { get; private set; }

        [ItemBinding]
        public item_nullifier Nullifier { get; private set; }

        [ItemBinding]
        public item_cyclone Euls { get; private set; }

        [ItemBinding]
        public item_diffusal_blade DiffusalBlade { get; private set; }

        [ItemBinding]
        public item_invis_sword ShadowBlade { get; private set; }

        [ItemBinding]
        public item_silver_edge SilverEdge { get; private set; }

        [ItemBinding]
        public item_blink BlinkDagger { get; private set; }

        [ItemBinding]
        public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding]
        public item_black_king_bar BlackKingBar { get; set; }

        [ItemBinding]
        public item_orchid Orchid { get; private set; }

        [ItemBinding]
        public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding]
        public item_force_staff ForceStaff { get; private set; }

        [ItemBinding]
        public item_ethereal_blade EtherealBlade { get; private set; }

        [ItemBinding]
        public item_veil_of_discord VeilOfDiscord { get; private set; }

        [ItemBinding]
        public item_shivas_guard ShivasGuard { get; private set; }

        [ItemBinding]
        public item_sheepstick Sheepstick { get; private set; }

        [ItemBinding]
        public item_rod_of_atos RodOfAtos { get; private set; }

        [ItemBinding]
        public item_urn_of_shadows Urn { get; private set; }

        [ItemBinding]
        public item_spirit_vessel Vessel { get; private set; }

        [ItemBinding]
        public item_lotus_orb Lotus { get; private set; }

        [ItemBinding]
        public item_solar_crest SolarCrest { get; private set; }

        [ItemBinding]
        public item_blade_mail BladeMail { get; private set; }

        [ItemBinding]
        public item_medallion_of_courage Medallion { get; private set; }

        [ItemBinding]
        public item_heavens_halberd HeavensHalberd { get; private set; }

        [ItemBinding]
        public item_satanic Satanic { get; private set; }

        [ItemBinding]
        public item_mask_of_madness Mom { get; private set; }

        [ItemBinding]
        public item_power_treads Treads { get; private set; }

        [ItemBinding]
        public item_armlet Armlet { get; private set; }

        [ItemBinding]
        public item_hurricane_pike HurricanePike { get; private set; }

        #endregion

        #region MenuItems

        public MenuItem<bool> DrawRangeItem { get; private set; }
        public MenuItem<Slider> SatanicPercent { get; private set; }
        public MenuItem<Slider> FireSpearThreshold { get; private set; }
        public MenuItem<Slider> InnerFireThreshold { get; private set; }
        public MenuItem<bool> InformationTab { get; private set; }

        public MenuItem<HeroToggler> HalberdHeroes;
        public MenuItem<HeroToggler> MedallionHeroes;
        public MenuItem<HeroToggler> BloodthornHeroes;
        public MenuItem<HeroToggler> HurricaneHeroes;

        #endregion

        public float UltiCastRange
        {
            get
            {
                return this.Ulti.CastRange;
            }
        }

        public bool IsArmletEnabled { get; set; }

        public bool IsFireSpearGucci(Unit target)
        {
            bool useSpears = this.Owner.HealthPercent() >= FireSpearThreshold.Item.GetValue<Slider>().Value / 100f;

            return useSpears;
        }

        public async Task UseFireSpear(Unit target, CancellationToken token = default(CancellationToken))
        {
            this.FireSpear.UseAbility(target);
            await Task.Delay(this.FireSpear.GetCastDelay(target), token);
        }

        protected override ComboMode GetComboMode()
        {
            return new HuskarCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new HuskarHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.InnerFire = this.Context.AbilityFactory.GetAbility<huskar_inner_fire>();
            this.FireSpear = this.Context.AbilityFactory.GetAbility<huskar_burning_spear>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<huskar_life_break>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRangeItem = factory.Item("Draw Ulti Range", true);
            this.SatanicPercent = factory.Item("Satanic Percent", new Slider(20, 1, 100));
            this.FireSpearThreshold = factory.Item("Stop using Burning Spear", new Slider(10, 0, 100));
            this.FireSpearThreshold.Item.Tooltip =
                "Will stop attacking with Burning Spear if HP is lower than that percent";
            this.InnerFireThreshold = factory.Item("Inner Fire Threshold", new Slider(0, 20, 100));
            this.InnerFireThreshold.Item.Tooltip = "Set this to 100 if you want it to be always used";

            this.HalberdHeroes = itemMenu.Item("Halberd",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.MedallionHeroes = itemMenu.Item("Medallion/SolarCrest",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.BloodthornHeroes = itemMenu.Item("Bloodthorn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.InformationTab = itemMenu.Item("Hover over for info", true);
            this.InformationTab.Item.Tooltip = "Hurricane pike will be used if guaranteed kill. Select heroes you want to use when they are in attack range.";
            this.HurricaneHeroes = itemMenu.Item("HurricanePike",
                new HeroToggler(new Dictionary<string, bool>(), true, false, false));

            if (this.DrawRangeItem)
            {
                UpdateManager.Subscribe(this.UpdateDrawRange, 25);
            }

            Player.OnExecuteOrder += this.OnExecuteOrder;
        }

        protected override void OnDeactivate()
        {
            UpdateManager.Unsubscribe(this.UpdateDrawRange);
            Player.OnExecuteOrder -= this.OnExecuteOrder;

            base.OnDeactivate();
        }

        private void UpdateDrawRange()
        {
            if (this.DrawRangeItem && this.Ulti.Ability.Level > 0)
            {
                this.Context.Particle.DrawRange(this.Owner, "huskarUltiRange", this.UltiCastRange, Color.Red);
            }
            else
            {
                this.Context.Particle.Remove("huskarUltiRange");
            }
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if ((this.Armlet != null) && args.IsPlayerInput && (args.OrderId == OrderId.ToggleAbility) &&
                (args.Ability == this.Armlet.Ability))
            {
                Log.Debug($"Armlet activated by you");
                this.IsArmletEnabled = !this.Armlet.Enabled;
            }
        }
    }
}