using Ensage.Common.Extensions;
using Ensage.SDK.Handlers;

namespace BAIO.Heroes.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using Ensage;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_obsidian_destroyer;
    using Ensage.SDK.Extensions;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Utils;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Menu;
    using log4net;
    using PlaySharp.Toolkit.Helper.Annotations;
    using PlaySharp.Toolkit.Logging;
    using SharpDX;
    using BAIO.Interfaces;
    using Ensage.Common.Menu;
    using BAIO.Modes;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Heroes.Modes.Harass;

    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_obsidian_destroyer)]
    public class ObsidianDestroyer : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities
        public obsidian_destroyer_arcane_orb ArcaneOrb { get; private set; }
        public obsidian_destroyer_astral_imprisonment Astral { get; private set; }
        public obsidian_destroyer_equilibrium Equilibrium { get; private set; }
        public obsidian_destroyer_sanity_eclipse SanityEclipse { get; private set; }
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
        public item_hurricane_pike HurricanePike { get; private set; }

        #endregion
        #region MenuItems
        public MenuItem<HeroToggler> NullifierHeroes;
        public MenuItem<HeroToggler> HexHeroes;
        public MenuItem<Slider> UseBlinkPrediction { get; set; }
        public MenuItem<Slider> MinimumTargetToUlti { get; set; }
        public MenuItem<Slider> HurricanePercentage { get; set; }
        public MenuItem<bool> AutoEqu { get; set; }
        public MenuItem<Slider> EquThreshold { get; set; }

        #endregion

        public float ComboMana
        {
            get
            {
                var totalMana = 0.0f;

                if (this.ShadowBlade != null)
                {
                    totalMana += this.ShadowBlade.Ability.ManaCost;
                }
                else if (this.SilverEdge != null)
                {
                    totalMana += this.SilverEdge.Ability.ManaCost;
                }

                if (this.Orchid != null)
                {
                    totalMana += this.Orchid.Ability.ManaCost;
                }
                else if (this.BloodThorn != null)
                {
                    totalMana += this.BloodThorn.Ability.ManaCost;
                }

                if (this.VeilOfDiscord != null)
                {
                    totalMana += this.VeilOfDiscord.Ability.ManaCost;
                }
                if (this.RodOfAtos != null)
                {
                    totalMana += this.RodOfAtos.Ability.ManaCost;
                }
                if (this.HurricanePike != null)
                {
                    totalMana += this.HurricanePike.Ability.ManaCost;
                }
                if (this.Sheepstick != null)
                {
                    totalMana += this.Sheepstick.Ability.ManaCost;
                }
                if (this.ShivasGuard != null)
                {
                    totalMana += this.ShivasGuard.Ability.ManaCost;
                }

                return totalMana;
            }
        }

        protected TaskHandler EquilibriumCastHandler { get; private set; }

        protected override ComboMode GetComboMode()
        {
            return new ObsidianDestroyerCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new ObsidianDestroyerHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.ArcaneOrb = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_arcane_orb>();
            this.Astral = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_astral_imprisonment>();
            this.Equilibrium = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_equilibrium>();
            this.SanityEclipse = this.Context.AbilityFactory.GetAbility<obsidian_destroyer_sanity_eclipse>();

            this.EquilibriumCastHandler = UpdateManager.Run(this.EquilibriumCaster);

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.UseBlinkPrediction = factory.Item("Blink Prediction", new Slider(200, 0, 600));
            this.UseBlinkPrediction.Item.Tooltip = "Will blink to set distance. Set to 0 if you want to disable it.";

            this.MinimumTargetToUlti = factory.Item("Ulti Target Count", new Slider(1, 1, 5));
            this.MinimumTargetToUlti.Item.Tooltip =
                "Minimum required enemy heroes to cast ulti. Atleast 1 of them should die too.";

            this.EquThreshold = factory.Item("Equilibrium Threshold", new Slider(75, 1, 100));
            this.EquThreshold.Item.Tooltip =
                "Will use Equilibrium only if % mana is lower than value.";

            this.AutoEqu = factory.Item("Auto Equilibrium", true);

            this.HurricanePercentage = itemMenu.Item("Hurricane Usage Percent", new Slider(20, 0, 100));
            this.NullifierHeroes = itemMenu.Item("Nullifier",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HexHeroes = itemMenu.Item("Hex",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.EquilibriumCastHandler.Cancel();
        }

        private async Task EquilibriumCaster(CancellationToken token)
        {
            var astralled = EntityManager<Unit>.Entities.Where(
                x => x.Team != this.Owner.Team
                     && !x.IsIllusion
                     && x.HasModifier("modifier_obsidian_destroyer_astral_imprisonment_prison")
                     && x.GetModifierByName("modifier_obsidian_destroyer_astral_imprisonment_prison").RemainingTime <= 0.5f).ToList();
            try
            {
                if (astralled.Count() >= 1 && this.AutoEqu)
                {
                    this.Equilibrium.UseAbility();
                    await Task.Delay(this.Equilibrium.GetCastDelay(), token);
                }
                else
                {
                    await Task.Delay(150, token);
                }
            }
            catch (TaskCanceledException)
            {
                //
            }
            catch (Exception e)
            {
                Log.Debug($"{e}");
            }
        }


        public bool ShouldUseArcaneOrb(Unit target)
        {
            var useArcaneOrb = false;
            if ((this.ComboMana <= (this.Owner.Mana - this.ArcaneOrb.Ability.ManaCost)) &&
                this.ArcaneOrb.CanBeCasted && this.Owner.CanAttack() && this.Context.Orbwalker.CanAttack(target))
            {
                useArcaneOrb = true;
            }
            return useArcaneOrb;
        }

        public async Task UseArcaneOrb(Unit target, CancellationToken token = default(CancellationToken))
        {
            this.ArcaneOrb.UseAbility(target);
            await Task.Delay(this.ArcaneOrb.GetCastDelay(target), token);
        }

        

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Astral.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager<Hero>.Entities.FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.Astral.CanHit(x)
                     && this.Astral.GetDamage(x) - (x.HealthRegeneration * 5) > x.Health);

            if (killstealTarget != null)
            {
                if (this.Astral.UseAbility(killstealTarget))
                {
                    await this.AwaitKillstealDelay(this.Astral.GetCastDelay(killstealTarget), token);
                }
            }
            await Task.Delay(125, token);
        }
    }
}
