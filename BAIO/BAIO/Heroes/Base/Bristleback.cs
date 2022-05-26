using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;
using Ensage;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_antimage;
using Ensage.SDK.Abilities.npc_dota_hero_bristleback;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;

namespace BAIO.Heroes.Base
{
    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_bristleback)]
    internal class Bristleback : BaseHero
    {

        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        public bristleback_quill_spray QuillSpray { get; private set; }

        public bristleback_viscous_nasal_goo NasalGoo { get; private set; }

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

        #endregion

        #region MenuItems

        public MenuItem<HeroToggler> AbyssalBladeHeroes;
        public MenuItem<HeroToggler> MantaHeroes;
        public MenuItem<HeroToggler> NullifierHeroes;

        public MenuItem<Slider> MinimumBlinkRange;

       // public MenuItem<KeyBind> TurnTheOtherCheek;

        #endregion

        protected override ComboMode GetComboMode()
        {
            return new BristlebackCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.QuillSpray = this.Context.AbilityFactory.GetAbility<bristleback_quill_spray>();
            this.NasalGoo = this.Context.AbilityFactory.GetAbility<bristleback_viscous_nasal_goo>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

           /* this.TurnTheOtherCheek = factory.Item("Turn The Other Cheek", new KeyBind(70));
            this.TurnTheOtherCheek.Item.Tooltip = "Will try to turn back to enemies."; */

            this.NullifierHeroes = itemMenu.Item("Nullifier",
                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.QuillSpray.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager<Hero>.Entities.FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.QuillSpray.CanHit(x)
                     && this.QuillSpray.GetDamage(x) > x.Health);

            if (killstealTarget != null)
            {
                if (this.QuillSpray.UseAbility())
                {
                    await this.AwaitKillstealDelay(this.QuillSpray.GetCastDelay(), token);
                }
            }
            await Task.Delay(125, token);
        }
    }
}
