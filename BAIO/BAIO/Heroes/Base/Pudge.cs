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
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Abilities.npc_dota_hero_pudge;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using Ensage.SDK.Prediction;
using Ensage.SDK.Renderer;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using Color = System.Drawing.Color;

namespace BAIO.Heroes.Base
{
    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_pudge)]
    internal class Pudge : BaseHero
    {

        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        public pudge_meat_hook Hook { get; private set; }

        public pudge_rot Rot { get; private set; }

        public pudge_dismember Dismember { get; private set; }

        #endregion

        #region Items
        [ItemBinding]
        public item_soul_ring SoulRing { get; private set; }
        
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
        public MenuFactory Drawings { get; private set; }
        public MenuItem<StringList> MinimumHookChanceItem { get; private set; }
        public MenuItem<bool> DrawHookRange { get; private set; }
        public MenuItem<bool> BladeMailUsage;
        public MenuItem<bool> InformationDrawings { get; private set; }
        public MenuItem<KeyBind> KeepPosition { get; private set; }
        public MenuItem<KeyBind> BlinkCombo { get; private set; }
        public MenuItem<KeyBind> AllyHook { get; private set; }
        public MenuItem<Slider> PosX { get; private set; }
        public MenuItem<Slider> PosY { get; private set; }
        public MenuItem<Slider> TextSize { get; private set; }
        public MenuItem<Slider> HealThreshold { get; private set; }

        public MenuItem<HeroToggler> UrnHeroes;
        public MenuItem<HeroToggler> VesselHeroes;
        public MenuItem<HeroToggler> AbyssalBladeHeroes;
        public MenuItem<HeroToggler> HalberdHeroes;
        public MenuItem<HeroToggler> NullifierHeroes;
        public MenuItem<HeroToggler> EbHeroes;
        public MenuItem<HeroToggler> ScepterHealHeroes;
        #endregion

        public bool HasUserEnabledRot { get; private set; }
        public bool HookModifierDetected { get; set; }
        public HitChance MinimumHookChance { get; private set; }
        public TaskHandler OnUpdateHandler { get; private set; }
        public IRenderManager RendererManager { get; set; }

        public Vector3 ClosestTpPoint { get; set; }

        public List<Vector3> TpColors { get; } = new List<Vector3>()
        {
            new Vector3(0.2f, 0.4588236f, 1),
            new Vector3(0.4f, 1, 0.7490196f),
            new Vector3(0.7490196f, 0, 0.7490196f),
            new Vector3(0.9529412f, 0.9411765f, 0.04313726f),
            new Vector3(1, 0.4196079f, 0),
            new Vector3(0.9960785f, 0.5254902f, 0.7607844f),
            new Vector3(0.6313726f, 0.7058824f, 0.2784314f),
            new Vector3(0.3960785f, 0.8509805f, 0.9686275f),
            new Vector3(0, 0.5137255f, 0.1294118f),
            new Vector3(0.6431373f, 0.4117647f, 0)
        };


        protected override ComboMode GetComboMode()
        {
            return new PudgeCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.Hook = this.Context.AbilityFactory.GetAbility<pudge_meat_hook>();
            this.Rot = this.Context.AbilityFactory.GetAbility<pudge_rot>();
            this.Dismember = this.Context.AbilityFactory.GetAbility<pudge_dismember>();

            this.ClosestTpPoint = new Vector3();

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            RendererManager = this.Context.Renderer;

            this.Drawings = factory.Menu("Drawings");

            var drawings = this.Drawings;


            this.UrnHeroes = itemMenu.Item("Urn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.VesselHeroes = itemMenu.Item("Vessel",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HalberdHeroes = itemMenu.Item("Halberd",
                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.NullifierHeroes = itemMenu.Item("Nullifier",
                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.AbyssalBladeHeroes = itemMenu.Item("Abyssal Blade",
                    new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.EbHeroes = itemMenu.Item("Ethereal Blade",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));

            this.MinimumHookChanceItem = factory.Item("Minimum Hook Chance", new StringList(new[] { "Low", "Medium", "High" }, 1));
            this.BladeMailUsage = itemMenu.Item("Use BladeMail?", true);

            this.DrawHookRange = drawings.Item("Draw hook range", true);
            this.InformationDrawings = drawings.Item("Enable Information Drawings", true);
            this.KeepPosition = drawings.Item("Keep position when combo key pressed", new KeyBind('g', KeyBindType.Toggle, false));
            this.BlinkCombo = drawings.Item("Blink Combo", new KeyBind('f', KeyBindType.Toggle, false));
            this.ScepterHealHeroes = factory.Item("Heal Heroes", new HeroToggler(new Dictionary<string, bool>(), false, true, true));
            this.HealThreshold = factory.Item("Heal Threshold", new Slider(1000, 1, 2500));
            this.AllyHook = factory.Item("Hook Ally", new KeyBind('e'));
            this.PosX = drawings.Item("Drawing X", new Slider(1520, 0, 1800));
            this.PosY = drawings.Item("Drawing Y", new Slider(920, 0, 1800));
            this.TextSize = drawings.Item("Text size", new Slider(50, 1, 100));

            this.MinimumHookChance = this.MinimumHookChanceItem.GetEnum<HitChance>();
            this.MinimumHookChanceItem.PropertyChanged += this.MinimumHookChancePropertyChanged;

            this.OnUpdateHandler = UpdateManager.Run(this.OnUpdate);
            Unit.OnModifierAdded += this.OnHookAdded;
            Unit.OnModifierRemoved += this.OnHookRemoved;
            Player.OnExecuteOrder += this.OnExecuteOrder;
            RendererManager.Draw += OnDraw;
        }

        protected override void OnDeactivate()
        {
            this.MinimumHookChanceItem.PropertyChanged -= this.MinimumHookChancePropertyChanged;

            Player.OnExecuteOrder -= this.OnExecuteOrder;
            Unit.OnModifierAdded -= this.OnHookAdded;
            Unit.OnModifierRemoved -= this.OnHookRemoved;
            this.OnUpdateHandler.Cancel();
            RendererManager.Draw -= OnDraw;

            base.OnDeactivate();
        }

        private void MinimumHookChancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MinimumHookChance = this.MinimumHookChanceItem.GetEnum<HitChance>();
        }

        private void OnExecuteOrder(Player sender, ExecuteOrderEventArgs args)
        {
            if (args.IsPlayerInput && (args.OrderId == OrderId.ToggleAbility) && (args.Ability == this.Rot.Ability))
            {
                this.HasUserEnabledRot = !this.Rot.Enabled;
            }
        }

        private void OnHookAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner.IsEnemy(sender) && (args.Modifier.Name == this.Hook.TargetModifierName))
            {
                //Log.Debug($"Hook used");
                this.HookModifierDetected = true;
            }
        }
        private void OnHookRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner.IsEnemy(sender) && (args.Modifier.Name == this.Hook.TargetModifierName))
            {
                this.HookModifierDetected = false;
            }
        }

        private void OnDraw(IRenderer renderer)
        {
            try
            {
                if (this.InformationDrawings)
                {
                    var startPos = new Vector2(this.PosX.Value, this.PosY.Value);
                    var keepPos = this.KeepPosition;
                    renderer.DrawText(startPos,
                        "Keep Position: " + (keepPos ? "ON" : "OFF"), keepPos ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Red, this.TextSize);
                    var blinkCombo = this.BlinkCombo;
                    renderer.DrawText(startPos + new Vector2(0, 30),
                        "Blink Combo: " + (blinkCombo ? "ON" : "OFF"), blinkCombo ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Red, this.TextSize);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{exception}");
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || Dagon == null || !this.Dagon.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTargetDagon = EntityManager<Hero>.Entities.FirstOrDefault(
                x => x.IsAlive
                     && (x.Team != this.Owner.Team)
                     && !x.IsIllusion
                     && this.Dagon.CanHit(x)
                     && this.Dagon.GetDamage(x) > x.Health);

            if (killstealTargetDagon != null)
            {
                if (this.Dagon.UseAbility(killstealTargetDagon))
                {
                    await this.AwaitKillstealDelay(this.Dagon.GetCastDelay(killstealTargetDagon), token);
                }
            }
            await Task.Delay(125, token);
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Rot.CanBeCasted)
            {
                await Task.Delay(250, token);
                return;
            }

            if (DrawHookRange && Hook.Ability.Level > 0)
            {
                this.Context.Particle.DrawRange(
                    Owner,
                    "HookRange",
                    Hook.CastRange,
                    SharpDX.Color.Red);
            }
            else
            {
                this.Context.Particle.Remove("HookRange");
            }

            var rotEnabled = this.Rot.Enabled;
            if (rotEnabled && !this.HasUserEnabledRot && !this.HookModifierDetected)
            {
                var enemyNear = EntityManager<Hero>.Entities.Any(x => x.IsVisible && x.IsAlive && this.Owner.IsEnemy(x) && this.Rot.CanHit(x));
                if (!enemyNear)
                {
                    this.Rot.Enabled = false;
                    rotEnabled = false;
                    await Task.Delay(this.Rot.GetCastDelay(), token);
                }
            }

            if (this.Owner.AghanimState())
            {
                var dismember = this.Dismember;
                var healTarget = EntityManager<Hero>.Entities.FirstOrDefault(x =>
                    x != null && x.IsValid && x.Team == this.Owner.Team && x != this.Owner && x.IsVisible &&
                    x.IsAlive && x.Distance2D(this.Owner) <= this.Dismember.CastRange * 1.2f && x.Health < HealThreshold.Value &&
                    !Ensage.SDK.Extensions.UnitExtensions.HasModifier(x, "modifier_pudge_swallow_hide") &&
                    this.ScepterHealHeroes.Value.IsEnabled(x.Name));

                if (healTarget != null && this.Dismember.CanBeCasted)
                {
                    if (this.Dismember.UseAbility(healTarget))
                    {
                        await Task.Delay(this.Dismember.GetCastDelay(healTarget) + 200, token);
                    }
                }
            }
            await Task.Delay(125, token);
        }
    }
}
