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
using Ensage.Common.Threading;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_antimage;
using Ensage.SDK.Abilities.npc_dota_hero_bristleback;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

namespace BAIO.Heroes.Base
{
    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_pugna)]
    internal class Pugna : BaseHero
    {

        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        public Ability Decrepify { get; set; }

        public Ability Ward { get; set; }

        public Ability Drain { get; set; }

        public Ability Blast { get; set; }

        public bool IsHealing { get; set; }

        public Unit HealTarget { get; set; }

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
        public item_dagon Dagon1 { get; set; }

        [ItemBinding]
        public item_dagon_2 Dagon2 { get; set; }

        [ItemBinding]
        public item_dagon_3 Dagon3 { get; set; }

        [ItemBinding]
        public item_dagon_4 Dagon4 { get; set; }

        [ItemBinding]
        public item_dagon_5 Dagon5 { get; set; }

        public Dagon Dagon => Dagon1 ?? Dagon2 ?? Dagon3 ?? Dagon4 ?? (Dagon)Dagon5;

        #endregion

        #region MenuItems


        public MenuItem<Slider> UseBlinkPrediction { get; set; }

        public MenuItem<HeroToggler> HealTargetHeroes { get; set; }

        public MenuItem<Slider> DrainHP { get; set; }

        public MenuItem<Slider> WardTargets { get; set; }

        public MenuItem<Slider> SelfHPDrain { get; set; }

        public MenuItem<Slider> PostDrainHP { get; set; }

        public MenuItem<Slider> HealAllyTo { get; set; }

        #endregion

        protected override ComboMode GetComboMode()
        {
            return new PugnaCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Decrepify = UnitExtensions.GetAbilityById(this.Owner, AbilityId.pugna_decrepify);
            this.Blast = UnitExtensions.GetAbilityById(this.Owner, AbilityId.pugna_nether_blast);
            this.Ward = UnitExtensions.GetAbilityById(this.Owner, AbilityId.pugna_nether_ward);
            this.Drain = UnitExtensions.GetAbilityById(this.Owner, AbilityId.pugna_life_drain);


            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.UseBlinkPrediction = factory.Item("Blink Prediction", new Slider(200, 0, 600));
            this.UseBlinkPrediction.Item.Tooltip = "Will blink to set distance. Set to 0 if you want to disable it.";
            this.DrainHP = factory.Item("Heal ally HP", new Slider(30, 0, 100));
            this.DrainHP.Item.Tooltip = "Allies HP to begin healing";
            this.SelfHPDrain = factory.Item("Min HP to Heal", new Slider(60, 0, 100));
            this.SelfHPDrain.Item.Tooltip = "HP threshold to start healing";
            this.PostDrainHP = factory.Item("Post drain HP", new Slider(30, 0, 100));
            this.PostDrainHP.Item.Tooltip =
                "HP threshold to stop healing. (this value must be higher than post drain HP)";
            this.HealAllyTo = factory.Item("Post drain HP for ally", new Slider(100, 0, 100));
            this.HealAllyTo.Item.Tooltip = "Heal ally to this hp (this value must be higher than heal ally HP)";
            this.WardTargets = factory.Item("Targets for ward", new Slider(0, 1, 5));
            this.WardTargets.Item.Tooltip = "Targets in range of ward for usage";
            this.HealTargetHeroes = factory.Item("Heal Targets", new HeroToggler(new Dictionary<string, bool>(), false, true, false));
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !AbilityExtensions.CanBeCasted(this.Blast))
            {
                await Task.Delay(125, token);
                return;
            }

            var damageBlast = Blast.GetAbilitySpecialData("blast_damage");
            damageBlast *= Ensage.SDK.Extensions.UnitExtensions.GetSpellAmplification(this.Owner);

            bool comboMana = Blast.GetManaCost(Blast.Level - 1) + Decrepify.GetManaCost(Decrepify.Level - 1) <
                             Owner.Mana;

            var decrepifyKillable =
                EntityManager<Hero>.Entities.FirstOrDefault(
                    x =>
                        x.IsAlive && x.Team != this.Owner.Team && !x.IsIllusion
                        && x.Health < damageBlast * (1 - x.MagicDamageResist)
                        && Blast != null && Blast.IsValid && x.Distance2D(this.Owner) <= 900
                        && Ensage.Common.Extensions.AbilityExtensions.CanBeCasted(Decrepify, x) && Ensage.Common.Extensions.AbilityExtensions.CanBeCasted(Blast, x)
                        && !UnitExtensions.IsMagicImmune(x) && comboMana);

            var blastKillable =
                EntityManager<Hero>.Entities.FirstOrDefault(
                    x =>
                        x.IsAlive && x.Team != this.Owner.Team && !x.IsIllusion
                        && x.Health < damageBlast * (1 - x.MagicDamageResist)
                        && Ensage.Common.Extensions.AbilityExtensions.CanBeCasted(Blast, x) && !UnitExtensions.IsMagicImmune(x) && Blast.CanHit(x)
                        && Ensage.SDK.Extensions.EntityExtensions.Distance2D(Owner, x.NetworkPosition) <= 800);

            if (!UnitExtensions.IsChanneling(this.Owner))
            {

                if (decrepifyKillable != null)
                {
                    if (Decrepify.UseAbility(decrepifyKillable))
                    {
                        await this.AwaitKillstealDelay((int)AbilityExtensions.GetCastDelay(this.Decrepify, this.Owner, decrepifyKillable, true), token); // decrepify
                    }

                    if (Blast.CanHit(decrepifyKillable))
                    {
                        if (Blast.UseAbility(decrepifyKillable))
                        {
                            await this.AwaitKillstealDelay((int)AbilityExtensions.GetCastDelay(this.Blast, this.Owner, decrepifyKillable, true), token);
                        }
                    }
                }

                if (blastKillable != null)
                {
                    if (Blast.UseAbility(blastKillable.NetworkPosition))
                    {
                        await this.AwaitKillstealDelay((int)AbilityExtensions.GetCastDelay(this.Blast, this.Owner, blastKillable, true), token);
                    }
                }

            }
        }
    }
}
