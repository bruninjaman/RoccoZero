using System.Collections.Generic;
using Ensage.SDK.Extensions;
using Ensage.SDK.Renderer;
using SharpDX;

namespace BAIO.Heroes.Base
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using BAIO.Heroes.Modes.Combo;
    using BAIO.Interfaces;
    using BAIO.Modes;
    using Ensage;
    using Ensage.Common.Extensions;
    using Ensage.Common.Menu;
    using Ensage.SDK.Abilities.Items;
    using Ensage.SDK.Abilities.npc_dota_hero_rattletrap;
    using Ensage.SDK.Handlers;
    using Ensage.SDK.Helpers;
    using Ensage.SDK.Inventory.Metadata;
    using Ensage.SDK.Menu;
    using Ensage.SDK.Prediction;
    using log4net;
    using PlaySharp.Toolkit.Helper.Annotations;
    using PlaySharp.Toolkit.Logging;


    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_rattletrap)]
    public class Clockwerk : BaseHero
    {
        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        public rattletrap_battery_assault BatteryAssault { get; private set; }

        public rattletrap_power_cogs Cogs { get; private set; }

        public rattletrap_rocket_flare Rocket { get; private set; }

        public rattletrap_hookshot Ulti { get; private set; }

        public Ability Overclocking { get; private set; }

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

        public MenuItem<HeroToggler> UrnHeroes;
        public MenuItem<HeroToggler> VesselHeroes;
        public MenuItem<HeroToggler> NullifierHeroes;
        public MenuItem<HeroToggler> HalberdHeroes;
        public MenuItem<HeroToggler> OrchidHeroes;
        public MenuItem<bool> BladeMailUsage;
        public MenuItem<bool> LotusUsage;

        public MenuFactory Drawings { get; private set; }

        public MenuItem<bool> InformationDrawings { get; private set; }
        public MenuItem<bool> InsecDrawings { get; private set; }
        public MenuItem<Slider> PosX { get; private set; }
        public MenuItem<Slider> PosY { get; private set; }
        public MenuItem<Slider> TextSize { get; private set; }
        public MenuItem<KeyBind> Insec { get; private set; }
        public MenuItem<StringList> MinimumHookChanceMenu { get; private set; }
        public MenuItem<bool> RocketPush { get; private set; }

        #endregion

        public bool IsHookModifier { get; set; }
        public HitChance MinimumHookChance { get; private set; }

        public TaskHandler RocketHandler { get; private set; }

        public IRenderManager RendererManager { get; set; }

        protected override ComboMode GetComboMode()
        {
            return new ClockwerkCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            this.BatteryAssault = this.Context.AbilityFactory.GetAbility<rattletrap_battery_assault>();
            this.Cogs = this.Context.AbilityFactory.GetAbility<rattletrap_power_cogs>();
            this.Rocket = this.Context.AbilityFactory.GetAbility<rattletrap_rocket_flare>();
            this.Ulti = this.Context.AbilityFactory.GetAbility<rattletrap_hookshot>();
            this.Overclocking = this.Owner.GetAbilityById(AbilityId.rattletrap_overclocking);

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            this.Drawings = factory.Menu("Drawings");
            var drawings = this.Drawings;

            RendererManager = this.Context.Renderer;

            this.InformationDrawings = drawings.Item("Enable Information Drawings", true);
            this.InsecDrawings = drawings.Item("Draw insec indicator", true);
            this.PosX = drawings.Item("Drawing X", new Slider(1520, 0, 1800));
            this.PosY = drawings.Item("Drawing Y", new Slider(920, 0, 1800));
            this.TextSize = drawings.Item("Text size", new Slider(50, 1, 100));

            this.MinimumHookChanceMenu = factory.Item("Minimum Hook Chance",
                new StringList(new[] {"Low", "Medium", "High"}, 1));
            this.RocketPush = factory.Item("Rocket Pusher", false);
            this.RocketPush.Item.Tooltip =
                "Will use rocket on creepwaves to push.";
            this.Insec = factory.Item("Insec controller", new KeyBind(70, KeyBindType.Toggle));
            this.Insec.Item.Tooltip =
                "If enabled, will try to push target with cogs rather than trap them inside of cogs.";
            this.MinimumHookChance = this.MinimumHookChanceMenu.GetEnum<HitChance>();

            this.UrnHeroes = itemMenu.Item("Urn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.VesselHeroes = itemMenu.Item("Vessel",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.NullifierHeroes = itemMenu.Item("Nullifier",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HalberdHeroes = itemMenu.Item("Halberd",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.OrchidHeroes = itemMenu.Item("Orchid Bloodthorn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.BladeMailUsage = itemMenu.Item("Use BladeMail?", true);
            this.LotusUsage = itemMenu.Item("Use Lotus?", true);


            this.RocketHandler = UpdateManager.Run(this.OnUpdate);
            this.MinimumHookChanceMenu.PropertyChanged += this.MinimumHookChancePropertyChanged;
           // this.RocketPush.PropertyChanged += this.RocketPushPropertyChanged;
            Unit.OnModifierAdded += this.OnHookAdded;
            Unit.OnModifierRemoved += this.OnHookRemoved;
            RendererManager.Draw += OnDraw;
        }

        private void OnDraw(IRenderer renderer)
        {
            try
            {
                if (this.InformationDrawings)
                {
                    if (this.InsecDrawings)
                    {
                        var startPos = new Vector2(this.PosX.Value, this.PosY.Value);
                        var insecInd = this.Insec.Value;
                        renderer.DrawText(startPos,
                            "Insec: " + (insecInd ? "ON" : "OFF"), insecInd ? System.Drawing.Color.LawnGreen : System.Drawing.Color.Red, this.TextSize);
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{exception}");
            }
        }

        protected override void OnDeactivate()
        {
            this.MinimumHookChanceMenu.PropertyChanged -= this.MinimumHookChancePropertyChanged;
           // this.RocketPush.PropertyChanged -= this.RocketPushPropertyChanged;

            Unit.OnModifierAdded -= this.OnHookAdded;
            Unit.OnModifierRemoved -= this.OnHookRemoved;
            RendererManager.Draw -= OnDraw;
            this.RocketHandler.Cancel();

            base.OnDeactivate();
        }

       /* private void RocketPushPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RocketPush.Value = this.RocketPush.Item.GetValue<bool>();
        }*/

        private void MinimumHookChancePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.MinimumHookChance = this.MinimumHookChanceMenu.GetEnum<HitChance>();
        }

        private void OnHookAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner.Team != sender.Team &&
                (args.Modifier.Name == "modifier_rattletrap_hookshot"))
            {
                Log.Debug($"Hook detected");
                this.IsHookModifier = true;
            }
        }

        private void OnHookRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner.Team != sender.Team && (args.Modifier.Name == "modifier_rattletrap_hookshot"))
            {
                this.IsHookModifier = false;
            }
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Rocket.CanBeCasted)
            {
                await Task.Delay(125, token);
                return;
            }

            var killstealTarget = EntityManager<Hero>.Entities.FirstOrDefault(
                x => x.IsAlive && x.Team != this.Owner.Team && this.Rocket.CanBeCasted && this.Rocket.CanHit(x) &&
                     x.Distance2D(this.Owner) < 5000 && !x.IsIllusion &&
                     x.Health < GetDamage(x));

            if (killstealTarget != null)
            {
                var input = new PredictionInput(Owner, killstealTarget,
                        this.Rocket.GetCastDelay() / 1000f,
                        1750,
                        float.MaxValue,
                        600,
                        PredictionSkillshotType.SkillshotCircle,
                        true,
                        null)
                    {Delay = Math.Max(Rocket.CastPoint + Game.Ping, 0) / 1000f};
                var output = this.Rocket.GetPredictionOutput(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    Rocket.UseAbility(output.CastPosition);
                    await this.AwaitKillstealDelay(Rocket.GetCastDelay(), token);
                }
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || !this.Rocket.CanBeCasted || !this.RocketPush.Value ||
                this.Config.General.ComboKey.Value.Active)
            {
                await Task.Delay(250, token);
                return;
            }

            var creeps = EntityManager<Unit>.Entities.OrderBy(x => x.Distance2D(this.Owner)).Where(
                x =>
                    x.IsValid && x is Creep && x.IsSpawned && !x.IsNeutral && x.Health + 50 <= GetDamage(x) &&
                    !x.IsMoving && x.IsAlive &&
                    x.Team != this.Owner.Team && x.Distance2D(this.Owner) > 1000).ToList();

            if (creeps.Any())
            {
                var input = new PredictionInput(Owner, creeps.Last(),
                    this.Rocket.GetCastDelay() / 1000f,
                    1750,
                    float.MaxValue,
                    600,
                    PredictionSkillshotType.SkillshotCircle,
                    true,
                    creeps)
                { Delay = Math.Max(Rocket.CastPoint + Game.Ping, 0) / 1000f };
                var output = this.Rocket.GetPredictionOutput(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    Rocket.UseAbility(output.CastPosition);
                    await Task.Delay(Rocket.GetCastDelay(), token);
                }
            }
        }

        protected float RawDamage
        {
            get
            {
                var damage = this.Rocket.Ability.GetAbilitySpecialData("damage");

                var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_clockwerk_2);
                if (talent?.Level > 0)
                {
                    damage += talent.GetAbilitySpecialData("value");
                }

                return damage;
            }
        }

        public float GetDamage(params Unit[] targets)
        {
            var totalDamage = 0.0f;

            var damage = this.RawDamage;
            var amplify = this.Owner.GetSpellAmplification();
            foreach (var target in targets)
            {
                var reduction = this.Rocket.Ability.GetDamageReduction(target, DamageType.Magical);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }

            return totalDamage;
        }
    }
}