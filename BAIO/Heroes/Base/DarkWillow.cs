using Ensage.Common.Extensions;
using Ensage.SDK.Abilities.npc_dota_hero_dark_willow;

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
    using Ensage.SDK.Abilities.npc_dota_hero_drow_ranger;
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

    public enum ShadowRealmStatus
    {
        NotSkilled,

        Active,

        Deactivated
    }

    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_dark_willow)]
    public class DarkWillow : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities
        public dark_willow_bedlam Bedlam { get; private set; }
        public dark_willow_bramble_maze BrambleMaze { get; private set; }
        public dark_willow_cursed_crown CursedCrown { get; private set; }
        public dark_willow_shadow_realm ShadowRealm { get; private set; }
        public dark_willow_terrorize Terrorize { get; private set; }
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
        public MenuItem<bool> DrawRange { get; private set; }
        public MenuItem<HeroToggler> NullifierHeroes;
        public MenuItem<HeroToggler> HexHeroes;
        #endregion

        public ShadowRealmStatus RealmStatus { get; private set; }

        protected override ComboMode GetComboMode()
        {
            return new DarkWillowCombo(this);
        }

        public bool ShouldHit(Unit target)
        {
            var hitEnemy = false;
            if (this.Owner.AghanimState() || !this.Owner.HasModifier(this.ShadowRealm.ModifierName))
            {
                hitEnemy = true;
            }
            else
            {
                if (this.Owner.HasModifier(this.ShadowRealm.ModifierName) && target.Distance2D(this.Owner) + 150 <= this.ShadowRealm.CastRange &&
                    this.Owner.FindModifier(this.ShadowRealm.ModifierName).ElapsedTime >= this.ShadowRealm.Ability.GetAbilitySpecialData("max_damage_duration"))
                {
                    hitEnemy = true;
                }
            }
            return hitEnemy;
        }


        private void DrawRangePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.DrawRange)
            {
                this.Context.Particle.DrawRange(this.Owner, "shadow_realm_range_draw", this.ShadowRealm.CastRange, Color.Green);
            }
            else
            {
                this.Context.Particle.Remove("shadow_realm_range_draw");
            }
        }

        private void DrawRange_OnUpdate()
        {
            if (this.ShadowRealm.Ability.Level <= 0)
            {
                this.RealmStatus = ShadowRealmStatus.NotSkilled;
                return;
            }

            var range = this.ShadowRealm.CastRange;

            var enemies = EntityManager<Hero>.Entities
                .Where(x => x != null && x.IsValid && x.IsVisible && x.IsAlive && !x.IsIllusion &&
                            this.Owner.IsEnemy(x)).ToList();

            if (!this.Owner.HasModifier(this.ShadowRealm.ModifierName))
            {
                if (this.DrawRange && this.RealmStatus != ShadowRealmStatus.Deactivated)
                {
                    this.Context.Particle.Remove("shadow_realm_range_draw");
                    this.Context.Particle.DrawRange(this.Owner, "shadow_realm_range_draw", range, Color.Red);
                }

                this.RealmStatus = ShadowRealmStatus.Deactivated;
            }
            else
            {
                if (this.DrawRange && this.RealmStatus != ShadowRealmStatus.Active)
                {
                    this.Context.Particle.Remove("shadow_realm_range_draw");
                    this.Context.Particle.DrawRange(this.Owner, "shadow_realm_range_draw", range, Color.Green);
                }

                this.RealmStatus = ShadowRealmStatus.Active;
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Bedlam = this.Context.AbilityFactory.GetAbility<dark_willow_bedlam>();
            this.Terrorize = this.Context.AbilityFactory.GetAbility<dark_willow_terrorize>();
            this.ShadowRealm = this.Context.AbilityFactory.GetAbility<dark_willow_shadow_realm>();
            this.CursedCrown = this.Context.AbilityFactory.GetAbility<dark_willow_cursed_crown>();
            this.BrambleMaze = this.Context.AbilityFactory.GetAbility<dark_willow_bramble_maze>();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRange = factory.Item("Draw Shadow Realm Range", true);
            this.DrawRange.PropertyChanged += this.DrawRangePropertyChanged;

            this.NullifierHeroes = itemMenu.Item("Nullifier",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HexHeroes = itemMenu.Item("Hex",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));

            if (this.DrawRange && this.ShadowRealm != null && this.ShadowRealm.Ability.Level > 0)
            {
                this.Context.Particle.DrawRange(this.Owner, "shadow_realm_range_draw", this.ShadowRealm.CastRange, Color.Green);
            }

            UpdateManager.Subscribe(this.DrawRange_OnUpdate, 100); // updates every 0.1 seconds

        }

        protected override void OnDeactivate()
        {
            this.DrawRange.PropertyChanged -= this.DrawRangePropertyChanged;
            UpdateManager.Unsubscribe(this.DrawRange_OnUpdate);

            base.OnDeactivate();
        }
    }
}
