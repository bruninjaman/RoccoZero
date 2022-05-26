using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BAIO.Heroes.Modes.Combo;
using BAIO.Heroes.Modes.Harass;
using BAIO.Interfaces;
using BAIO.Modes;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Threading;
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_storm_spirit;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using Ensage.SDK.Menu.Items;
using Ensage.SDK.Prediction;
using Ensage.SDK.Renderer;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using Color = System.Drawing.Color;

namespace BAIO.Heroes.Base
{
    public enum DrawsTypes
    {
        PercentLeft,
        PercentWillBeUsed,
        RawCost,
        RawLeft
    }

    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_storm_spirit)]
    public class StormSpirit : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Items

        [ItemBinding]
        public item_soul_ring SoulRing { get; private set; }

        [ItemBinding]
        public item_shivas_guard ShivasGuard { get; private set; }


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
        private Item TeleportScrool { get; set; }

        [ItemBinding]
        private item_travel_boots Travel1 { get; set; }

        [ItemBinding]
        private item_travel_boots_2 Travel2 { get; set; }

        public TravelBoots TravelBoots
        {
            get
            {
                if (this.Travel2 != null)
                {
                    return this.Travel2;
                }
                return this.Travel1;
            }
        }

        #endregion

        #region Abilities

        public storm_spirit_static_remnant Remnant { get; private set; }

        public storm_spirit_electric_vortex Vortex { get; private set; }

        public storm_spirit_overload Overload { get; set; }

        public storm_spirit_ball_lightning Ulti { get; private set; }

        #endregion

        #region MenuItems

        public MenuFactory OverloadKeyMenu;

        public MenuItem<KeyBind> EscapeKey;
        public MenuItem<KeyBind> OverloadKey;

        public MenuItem<Slider> DistanceForUlt;
        public MenuItem<Slider> DrawingX;
        public MenuItem<Slider> DrawingY;
        public MenuItem<Slider> OverloadKeyDelay;
        public MenuItem<StringList> DrawingPrefMenu { get; private set; }
        public MenuItem<bool> IgnoreOverload { get; private set; }
        public MenuItem<bool> CheckModifier { get; private set; }

        public MenuItem<HeroToggler> BtAndOrchidHeroes;
        public MenuItem<HeroToggler> SheepHeroes;
        public MenuItem<HeroToggler> NullifierHeroes;

        #endregion

        public TaskHandler EscapeHandler { get; private set; }

        public bool InOverload { get; set; }
        public bool InUlti { get; set; }

        public DrawsTypes DrawType;

        protected override ComboMode GetComboMode()
        {
            return new StormSpiritCombo(this);
        }

        protected override HarassMode GetHarassMode()
        {
            return new StormSpiritHarass(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.OverloadKeyMenu = factory.Menu("Overload Menu", "overloadmenu");
            this.OverloadKey = OverloadKeyMenu.Item("Overload Charge Key", new KeyBind(71));
            this.OverloadKey.Item.Tooltip = "Will use ulti on position to get a overload charge";
            this.CheckModifier = OverloadKeyMenu.Item("Check Overload modifier", true);
            this.CheckModifier.Item.Tooltip = "Will check if you have overload modifier and won't use ulti if you do";
            this.OverloadKeyDelay = OverloadKeyMenu.Item("Overload key delay/cooldown", new Slider(100, 1, 500));

            this.DistanceForUlt = factory.Item("Max Distance for Ult", new Slider(1000, 0, 10000));
            this.DistanceForUlt.Item.Tooltip = "Enemies outside of this range will not be chased.";
            this.EscapeKey = factory.Item("Escape Key", new KeyBind(70));
            this.EscapeKey.Item.Tooltip = "Will use ulti + tp/travel to run away in dangerous situations.";
            this.IgnoreOverload = factory.Item("Ignore Overload for vortex", false);
            this.DrawingPrefMenu = factory.Item("Drawing Type",
                new StringList(new[] {"PercentLeft", "PercentWillBeUsed", "RawCost", "RawLeft"}, 1));
            this.DrawType = this.DrawingPrefMenu.GetEnum<DrawsTypes>();
            this.DrawingX = factory.Item("Drawing X", new Slider(0, 0, 1800));
            this.DrawingY = factory.Item("Drawing Y", new Slider(0, 0, 1800));

            this.BtAndOrchidHeroes = itemMenu.Item("Orchid/Bloodthorn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.SheepHeroes = itemMenu.Item("Hex", new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.NullifierHeroes = itemMenu.Item("Nullifier", new HeroToggler(new Dictionary<string, bool>(), true, false, true));

            this.EscapeHandler = UpdateManager.Run(this.OnUpdate);

            this.Remnant = this.Context.AbilityFactory.GetAbility<storm_spirit_static_remnant>();
            this.Vortex = this.Context.AbilityFactory.GetAbility<storm_spirit_electric_vortex>();
            this.Overload = this.Context.AbilityFactory.GetAbility<storm_spirit_overload>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<storm_spirit_ball_lightning>();

            this.DrawingPrefMenu.PropertyChanged += this.DrawingPrefMenuPropertyChanged;

            Unit.OnModifierAdded += this.OnOverloadAdded;
            Unit.OnModifierRemoved += this.OnOverloadRemoved;
            Unit.OnModifierAdded += this.OnUltiAdded;
            Unit.OnModifierRemoved += this.OnUltiRemoved;
            this.Context.RenderManager.Draw += this.OnDraw;
        }

        protected override void OnDeactivate()
        {
            this.DrawingPrefMenu.PropertyChanged -= this.DrawingPrefMenuPropertyChanged;
            Unit.OnModifierAdded -= this.OnOverloadAdded;
            Unit.OnModifierRemoved -= this.OnOverloadRemoved;
            Unit.OnModifierAdded -= this.OnUltiAdded;
            Unit.OnModifierRemoved -= this.OnUltiRemoved;
            this.Context.RenderManager.Draw -= this.OnDraw;
            this.EscapeHandler.Cancel();
            base.OnDeactivate();
        }

        private void DrawingPrefMenuPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.DrawType = this.DrawingPrefMenu.GetEnum<DrawsTypes>();
        }

        private void OnOverloadAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner == sender && (args.Modifier.Name == "modifier_storm_spirit_overload"))
            {
                //Log.Debug($"InOverload");
                this.InOverload = true;
            }
        }

        private void OnOverloadRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner == sender && (args.Modifier.Name == "modifier_storm_spirit_overload"))
            {
                this.InOverload = false;
            }
        }

        private void OnUltiAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner == sender &&
                (args.Modifier.Name == "modifier_storm_spirit_ball_lightning"))
            {
                //Log.Debug($"OnUlti");
                this.InUlti = true;
            }
        }

        private void OnUltiRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner == sender && (args.Modifier.Name == "modifier_storm_spirit_ball_lightning"))
            {
                this.InUlti = false;
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Ulti.CanBeCasted)
            {
                await Task.Delay(250, token);
                return;
            }

            if (this.EscapeKey.Value.Active)
            {
                try
                {
                    var fountainPosition =
                        EntityManager<Unit>.Entities.First(
                            x => x.Team == this.Owner?.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain).Position;

                    var inUltimate = this.InUlti ||
                                     this.Ulti.Ability.IsInAbilityPhase;
                    TeleportScrool = ObjectManager.GetEntities<Item>()
                        .FirstOrDefault(x => x.Owner == this.Owner && x.IsValid && x.Id == AbilityId.item_tpscroll);
                    if (!inUltimate && this.Ulti.CanBeCasted && !this.Owner.IsChanneling())
                    {
                        if (BAIO.Extensions.PositionCamera(fountainPosition.X, fountainPosition.Y))
                        {
                            if (this.Ulti.UseAbility(fountainPosition))
                            {
                                await Task.Delay(150, token);
                            }
                        }
                    }

                    if (TeleportScrool != null && TeleportScrool.CanBeCasted() && !this.Owner.IsChanneling())
                    {
                        TeleportScrool?.UseAbility(fountainPosition);
                        await Task.Delay(500, token);
                    }
                    await Task.Delay(100, token);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }

            if (this.OverloadKey.Value.Active)
            {
                var pos = this.Owner.Position;
                var onOverload = this.InOverload;
                var checkModif = this.CheckModifier;
                var delay = this.OverloadKeyDelay;
                var inUltimate = this.InUlti ||
                                 this.Ulti.Ability.IsInAbilityPhase;
                if (!inUltimate && this.Ulti.CanBeCasted && !this.Owner.IsChanneling())
                {
                    if (checkModif && onOverload)
                    {
                        return;
                    }
                    else
                    {
                        var closestEnemy = EntityManager<Hero>.Entities
                            .FirstOrDefault(x => x != null && x.IsValid && x.IsAlive && x.Team != this.Owner.Team && x.Distance2D(this.Owner) <= 1000);
                        if (closestEnemy != null)
                        {
                            this.Ulti.UseAbility(UltiPos(closestEnemy));
                            await Task.Delay(delay, token);
                        }
                        else
                        {
                            this.Ulti.UseAbility(UltiPos(Extensions.FacePosition(this.Owner)));
                            await Task.Delay(delay, token);
                        }
                    }
                }
                await Task.Delay(delay, token);
            }
        }

        private void OnDraw(IRenderer renderer)
        {
            if (this.Ulti.Ability.Level <= 0)
            {
                return;
            }

            Vector2 screenPos;
            var drawPos = new Vector2(DrawingX, DrawingY);
            var mousePos = Game.MousePosition;
            var cost = Math.Ceiling(this.Ulti.GetManaCost(mousePos));
            var minusCost = this.Owner.Mana - cost;
            var percentLeft = Math.Ceiling((minusCost / this.Owner.MaximumMana) * 100);
            var percentUsed = Math.Ceiling((cost / this.Owner.Mana) * 100);
            var rawLeft = Math.Ceiling((this.Owner.Mana - cost));

            if (Drawing.WorldToScreen(mousePos, out screenPos))
            {
                if (DrawType == DrawsTypes.PercentLeft)
                {
                    renderer.DrawText(drawPos, $"Left Percent {percentLeft.ToString()}%", Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.PercentWillBeUsed)
                {
                    renderer.DrawText(drawPos, $"Percent Used {percentUsed.ToString()}%", Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.RawLeft)
                {
                    renderer.DrawText(drawPos, $"Raw Left {rawLeft.ToString()}", Color.Red, 30);
                }
                else if (DrawType == DrawsTypes.RawCost)
                {
                    renderer.DrawText(drawPos, $"Raw Cost {cost.ToString()}", Color.Red, 30);
                }
            }
        }

        // Bu method rakibe karşı olan açıyı alıp, en uygun ulti pozisyonunu döndürüyor.
        public Vector3 UltiPos(Unit target)
        {
            var l = (target.Distance2D(this.Owner) - 50) / 50;
            var posA = target.Position;
            var posB = this.Owner.Position;
            var x = (posA.X + (l * posB.X)) / (1 + l);
            var y = (posA.Y + (l * posB.Y)) / (1 + l);
            return new Vector3((int) x, (int) y, posA.Z);
        }
        public Vector3 UltiPos(Vector3 pos)
        {
            var l = (pos.Distance2D(this.Owner) - 50) / 50;
            var posA = pos;
            var posB = this.Owner.Position;
            var x = (posA.X + (l * posB.X)) / (1 + l);
            var y = (posA.Y + (l * posB.Y)) / (1 + l);
            return new Vector3((int)x, (int)y, posA.Z);
        }
    }
}