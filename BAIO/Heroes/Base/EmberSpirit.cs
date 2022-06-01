//using BAIO.Heroes.Modes.Combo;
//using BAIO.Interfaces;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.Common.Menu;
//using Ensage.SDK.Abilities.Items;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Inventory.Metadata;
//using Ensage.SDK.Menu;
//using log4net;
//using PlaySharp.Toolkit.Helper.Annotations;
//using PlaySharp.Toolkit.Logging;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Threading.Tasks;
//using Ensage.SDK.Abilities.npc_dota_hero_ember_spirit;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Prediction;

//namespace BAIO.Heroes.Base
//{
//    [PublicAPI]
//    [ExportHero(HeroId.npc_dota_hero_ember_spirit)]
//    internal class EmberSpirit : BaseHero
//    {

//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        #region Abilities

//        public ember_spirit_searing_chains SearingChains { get; private set; }

//        public ember_spirit_sleight_of_fist Fist { get; private set; }

//        public ember_spirit_flame_guard FlameGuard { get; private set; }

//        public ember_spirit_fire_remnant FireRemnant { get; private set; }

//        public ember_spirit_activate_fire_remnant ActiveFireRemnant { get; private set; }

//        #endregion

//        #region Items

//        [ItemBinding]
//        public item_abyssal_blade AbyssalBlade { get; private set; }

//        [ItemBinding]
//        public item_manta Manta { get; private set; }

//        [ItemBinding]
//        public item_nullifier Nullifier { get; private set; }

//        [ItemBinding]
//        public item_cyclone Euls { get; private set; }

//        [ItemBinding]
//        public item_diffusal_blade DiffusalBlade { get; private set; }

//        [ItemBinding]
//        public item_invis_sword ShadowBlade { get; private set; }

//        [ItemBinding]
//        public item_silver_edge SilverEdge { get; private set; }

//        [ItemBinding]
//        public item_blink BlinkDagger { get; private set; }

//        [ItemBinding]
//        public item_bloodthorn BloodThorn { get; private set; }

//        [ItemBinding]
//        public item_black_king_bar BlackKingBar { get; set; }

//        [ItemBinding]
//        public item_orchid Orchid { get; private set; }

//        [ItemBinding]
//        public item_mjollnir Mjollnir { get; private set; }

//        [ItemBinding]
//        public item_force_staff ForceStaff { get; private set; }

//        [ItemBinding]
//        public item_ethereal_blade EtherealBlade { get; private set; }

//        [ItemBinding]
//        public item_veil_of_discord VeilOfDiscord { get; private set; }

//        [ItemBinding]
//        public item_shivas_guard ShivasGuard { get; private set; }

//        [ItemBinding]
//        public item_sheepstick Sheepstick { get; private set; }

//        [ItemBinding]
//        public item_rod_of_atos RodOfAtos { get; private set; }

//        [ItemBinding]
//        public item_urn_of_shadows Urn { get; private set; }

//        [ItemBinding]
//        public item_spirit_vessel Vessel { get; private set; }

//        [ItemBinding]
//        public item_lotus_orb Lotus { get; private set; }

//        [ItemBinding]
//        public item_solar_crest SolarCrest { get; private set; }

//        [ItemBinding]
//        public item_blade_mail BladeMail { get; private set; }

//        [ItemBinding]
//        public item_medallion_of_courage Medallion { get; private set; }

//        [ItemBinding]
//        public item_heavens_halberd HeavensHalberd { get; private set; }

//        [ItemBinding]
//        public item_satanic Satanic { get; private set; }

//        [ItemBinding]
//        public item_mask_of_madness Mom { get; private set; }

//        [ItemBinding]
//        public item_power_treads Treads { get; private set; }

//        #endregion

//        #region MenuItems

//        public MenuItem<HeroToggler> AbyssalBladeHeroes;
//        public MenuItem<HeroToggler> MantaHeroes;
//        public MenuItem<HeroToggler> NullifierHeroes;
//        public MenuItem<HeroToggler> HexHeroes;

//        public MenuItem<KeyBind> FistandChains;
//        public MenuItem<bool> AutoChain;
//        public MenuItem<Slider> LeaveSpirits;

//        #endregion

//        protected override ComboMode GetComboMode()
//        {
//            return new EmberSpiritCombo(this);
//        }

//        protected override void OnActivate()
//        {
//            base.OnActivate();

//            this.SearingChains = this.Context.AbilityFactory.GetAbility<ember_spirit_searing_chains>();
//            this.Fist = this.Context.AbilityFactory.GetAbility<ember_spirit_sleight_of_fist>();
//            this.FlameGuard = this.Context.AbilityFactory.GetAbility<ember_spirit_flame_guard>();
//            this.FireRemnant = this.Context.AbilityFactory.GetAbility<ember_spirit_fire_remnant>();
//            this.ActiveFireRemnant = this.Context.AbilityFactory.GetAbility<ember_spirit_activate_fire_remnant>();

//            var factory = this.Config.Hero.Factory;
//            var itemMenu = this.Config.Hero.ItemMenu;

//            this.FistandChains = factory.Item("Fist N Chains Combo", new KeyBind(70));
//            this.FistandChains.Item.Tooltip = "Will only use Fist and Chains.";

//            this.AutoChain = factory.Item("Auto Chain on W use", true);

//            this.MantaHeroes = itemMenu.Item("Manta",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.NullifierHeroes = itemMenu.Item("Nullifier",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
//            this.HexHeroes = itemMenu.Item("Hex",
//                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));

//            this.LeaveSpirits = factory.Item("Leave X amount of spirits unused", new Slider(1, 0, 7));

//        }

//        protected override async Task KillStealAsync(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Fist.CanBeCasted)
//            {
//                await Task.Delay(125, token);
//                return;
//            }

//            var killstealTarget = EntityManager.GetEntities<Hero>().FirstOrDefault(
//                x => x.IsAlive
//                     && (x.Team != this.Owner.Team)
//                     && !x.IsIllusion
//                     && this.Fist.CanHit(x) && this.Fist.GetDamage(x) >= x.Health);

//            if (killstealTarget != null)
//            {
//                var output = PredictionManager.GetPrediction(FistPredictionInput(killstealTarget));
//                if (this.Fist.Cast(output.CastPosition))
//                {
//                    await this.AwaitKillstealDelay(25, token);
//                }
//            }
//            await Task.Delay(125, token);
//        }

//        public PredictionInput FistPredictionInput(Unit target)
//        {
//            return new PredictionInput(this.Owner, target, 0.001f, float.MaxValue, this.Fist.CastRange, this.Fist.Radius, PredictionSkillshotType.SkillshotCircle);
//        }

//    }
//}
