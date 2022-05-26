using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

namespace BAIO.Heroes.Base
{
    public enum InsecType
    {
        TeamMate,
        Fountain
    }

    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_tusk)]
    public class Tusk : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        #endregion

        #region Abilities

        public Ability IceShards { get; private set; }

        public Ability Snowball { get; private set; }

        public Ability LaunchSnowball { get; private set; }

        public Ability TagTeam { get; private set; }

        public Ability WalrusPunch { get; private set; }

       public Ability WalrusKick { get; private set; }

        #endregion

        #region MenuItems

        public MenuItem<KeyBind> KickCombo;
        public MenuItem<KeyBind> EscapeKey;
        public MenuItem<KeyBind> CliffKey;
        public MenuItem<StringList> InsecPrefMenu { get; private set; }
        public MenuItem<HeroToggler> MedallionCrestHeroes;
        public MenuItem<HeroToggler> HalberdHeroes;
        public MenuItem<HeroToggler> UrnHeroes;
        public MenuItem<HeroToggler> VesselHeroes;
        public MenuItem<HeroToggler> AtosHeroes;
        public MenuItem<HeroToggler> OrchidBloodthornHeroes;

        #endregion

        public TaskHandler AghsHandler { get; private set; }

        public bool InSnowball { get; set; }

        public InsecType InsecType;

        protected override ComboMode GetComboMode()
        {
            return new TuskCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.KickCombo = factory.Item("Kick Combo", new KeyBind(70));
            this.KickCombo.Item.Tooltip = "Will use ulti with available keys on target.";

            this.CliffKey = factory.Item("Shard specialization Key", new KeyBind(71));
            this.CliffKey.Item.Tooltip = "Will use shards to get on cliffs or block enemy";


            this.InsecPrefMenu = factory.Item("Insec Type",
                new StringList(new[] { "TeamMate", "Fountain" }, 0));
            this.InsecType = this.InsecPrefMenu.GetEnum<InsecType>();

            this.MedallionCrestHeroes = itemMenu.Item("Medallion/Solar Crest",
                new HeroToggler(new Dictionary<string, bool>(), true, false, false));
            this.HalberdHeroes = itemMenu.Item("Halberd", new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.UrnHeroes = itemMenu.Item("Urn", new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.VesselHeroes = itemMenu.Item("Vessel", new HeroToggler(new Dictionary<string, bool>(), true, false, true));

            this.AghsHandler = UpdateManager.Run(this.OnUpdate);

            this.InsecPrefMenu.PropertyChanged += this.InsecPrefMenuPropertyChanged;
            Unit.OnModifierAdded += this.OnSnowballAdded;
            Unit.OnModifierRemoved += this.OnSnowballRemoved;

            this.IceShards = this.Owner.GetAbilityById(AbilityId.tusk_ice_shards);
            this.Snowball = this.Owner.GetAbilityById(AbilityId.tusk_snowball);
            this.TagTeam = this.Owner.GetAbilityById(AbilityId.tusk_tag_team);
            this.LaunchSnowball = this.Owner.GetAbilityById(AbilityId.tusk_launch_snowball);
            this.WalrusPunch = this.Owner.GetAbilityById(AbilityId.tusk_walrus_punch);
        }

        protected override void OnDeactivate()
        {
            this.AghsHandler.Cancel();
            this.InsecPrefMenu.PropertyChanged -= this.InsecPrefMenuPropertyChanged;
            Unit.OnModifierAdded -= this.OnSnowballAdded;
            Unit.OnModifierRemoved -= this.OnSnowballRemoved;

            base.OnDeactivate();
        }

        private void InsecPrefMenuPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.InsecType = this.InsecPrefMenu.GetEnum<InsecType>();
        }

        private void OnSnowballAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner == sender &&
                (args.Modifier.Name == "modifier_tusk_snowball_movement"))
            {
                //Log.Debug($"OnUlti");
                this.InSnowball = true;
            }
        }

        private void OnSnowballRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner == sender && (args.Modifier.Name == "modifier_tusk_snowball_movement"))
            {
                this.InSnowball = false;
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive)
            {
                await Task.Delay(250, token);
                return;
            }

            // setting walrus kick on OnUpdate so that it can return null if doesn't have aghanim
            // and can properly set skill if has aghanim.
            if (UnitExtensions.HasAghanimsScepter(this.Owner))
            {
                this.WalrusKick = this.Owner.GetAbilityById(AbilityId.tusk_walrus_kick);
            }
            else
            {
                WalrusKick = null;
            }

            if (this.CliffKey.Value.Active)
            {
                var pos = this.Owner.Position;
                if (this.IceShards != null && this.IceShards.CanBeCasted() && !this.Owner.IsChanneling())
                {
                    if (this.Owner.IsMoving)
                    {
                        this.IceShards.UseAbility(this.Owner.InFront(((float)this.Owner.MovementSpeed / 18) + (Game.Ping / 6))); // magic numbers
                        await Task.Delay((int)this.IceShards.GetCastDelay(this.Owner, this.Owner, true), token);
                    }
                    else
                    {
                        this.IceShards.UseAbility(this.Owner.InFront(5));
                        await Task.Delay((int)this.IceShards.GetCastDelay(this.Owner, this.Owner, true), token);
                    }
                }
                await Task.Delay(100, token);
            }
        }
    }
}