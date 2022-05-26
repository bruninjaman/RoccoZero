using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
using Ensage.SDK.Abilities.Aggregation;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using Ensage.SDK.Prediction;
using Ensage.SDK.Extensions;
using Ensage.SDK.Geometry;
using Ensage.SDK.Renderer;
using Ensage.SDK.Renderer.Particle;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using AbilityExtensions = Ensage.SDK.Extensions.AbilityExtensions;
using Color = System.Drawing.Color;
using UnitExtensions = Ensage.SDK.Extensions.UnitExtensions;

namespace BAIO.Heroes.Base
{
    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_nevermore)]
    public class Nevermore : BaseHero
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

        #region Abilities

        public Ability RazeShort { get; private set; }

        public Ability RazeMedium { get; private set; }

        public Ability RazeLong { get; set; }

        public Ability Ulti { get; private set; }

        #endregion

        #region MenuItems

        public MenuItem<KeyBind> UltiCombo;
        public MenuItem<bool> DrawRazeRange;
        public MenuItem<bool> PriRaze;
        public MenuItem<Slider> DrawingX;
        public MenuItem<Slider> DrawingY;
        public MenuItem<KeyBind> BkbToggle { get; private set; }
        public MenuItem<HeroToggler> EtherealHeroes;
        public MenuItem<HeroToggler> VeilOfDiscordHeroes;
        public MenuItem<HeroToggler> HexHeroes;
        public MenuItem<HeroToggler> OrchidBloodthornHeroes;
        public MenuItem<HeroToggler> AtosHeroes;
        public MenuItem<HeroToggler> HalberdHeroes;
        public MenuItem<bool> UseBlink;
        public MenuItem<bool> EullessUlti;
        #endregion

        public bool IsRazeModifier { get; set; }
        public string RazeModifierName { get; } = "modifier_nevermore_shadowraze_counter";
        public bool InUlti { get; set; }
        public IParticleManager Particle;
        public IPrediction Prediction;
        public List<Ability> Razes;
        public Vector3 PredictedTargetPosition { get; set; }

        public readonly List<string> EvadeAbilities = new List<string>()
        {
            "ursa_enrage",
            "nyx_assassin_spiked_carapace",
            "life_stealer_rage",
            "juggernaut_blade_fury"
        };
        public readonly List<string> EvadeItems = new List<string>()
        {
            "item_blade_mail",
            "item_black_king_bar",
            "item_cyclone",
        };

        protected override ComboMode GetComboMode()
        {
            return new NevermoreCombo(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;
            this.DrawRazeRange = factory.Item("Draw Raze Indicators", true);
            this.DrawRazeRange.Item.Tooltip = "When enabled, will draw raze indicators.";
            this.PriRaze = factory.Item("Make sure razes hit", true);
            this.PriRaze.Item.Tooltip =
                "When enabled, will stop razes if not guaranteed hit, but skip some attacks in return.";
            this.UltiCombo = factory.Item("Ulti Key", new KeyBind(70));
            this.UltiCombo.Item.Tooltip = "Will use ulti with available items on target.";
            this.EullessUlti = factory.Item("Use ulti without eul too", false);
            this.EullessUlti.Item.Tooltip = "Will use ulti without euls too. Only activate with good ping/fps";
            this.DrawingX = factory.Item("Drawing X", new Slider(0, 0, 1800));
            this.DrawingY = factory.Item("Drawing Y", new Slider(0, 0, 1800));

            this.BkbToggle = itemMenu.Item("Bkb Toggle", new KeyBind(71, KeyBindType.Toggle, true));
            this.EtherealHeroes = itemMenu.Item("Ethereal Blade",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.VeilOfDiscordHeroes = itemMenu.Item("Veil Of Discord",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HexHeroes = itemMenu.Item("Hex",
                new HeroToggler(new Dictionary<string, bool>(), true, false, false));
            this.OrchidBloodthornHeroes = itemMenu.Item("Orchid/Bloodthorn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.OrchidBloodthornHeroes = itemMenu.Item("Orchid/Bloodthorn",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.AtosHeroes = itemMenu.Item("Rod of Atos",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.HalberdHeroes = itemMenu.Item("Heaven's Halberd",
                new HeroToggler(new Dictionary<string, bool>(), true, false, true));
            this.UseBlink = itemMenu.Item("Use blink in combo?", true);

            this.Particle = this.Context.Particle;
            this.Prediction = this.Context.Prediction;

            UpdateManager.Subscribe(OnUpdate, 20);

            Unit.OnModifierAdded += this.OnRazeAdded;
            Unit.OnModifierRemoved += this.OnRazeRemoved;
            this.Context.RenderManager.Draw += this.OnDraw;

            this.RazeShort = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze1);
            this.RazeMedium = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze2);
            this.RazeLong = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_shadowraze3);
            this.Ulti = UnitExtensions.GetAbilityById(this.Owner, AbilityId.nevermore_requiem);
        }


        protected override void OnDeactivate()
        {
            base.OnDeactivate();

            UpdateManager.Unsubscribe(OnUpdate);

            Unit.OnModifierAdded -= this.OnRazeAdded;
            Unit.OnModifierRemoved -= this.OnRazeRemoved;
            this.Context.RenderManager.Draw -= this.OnDraw;
        }

        private void OnUpdate()
        {
            if (RazeShort.Level <= 0)
            {
                return;
            }

            Razes = new List<Ability> { RazeShort, RazeMedium, RazeLong };

            if (DrawRazeRange == true)
            {
                var raze1 = UnitExtensions.InFront(Owner, this.RazeShort.GetAbilitySpecialData("shadowraze_range"));
                var raze2 = UnitExtensions.InFront(Owner, this.RazeMedium.GetAbilitySpecialData("shadowraze_range"));
                var raze3 = UnitExtensions.InFront(Owner, this.RazeLong.GetAbilitySpecialData("shadowraze_range"));

                var file = "materials/ensage_ui/particles/alert_range.vpcf";
                Particle.AddOrUpdate(Owner, $"DrawRange_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None, 0,
                    raze1,
                    1, new Vector3(255, 0, 0), 2, new Vector3(200, 255, 40));
                Particle.AddOrUpdate(Owner, $"DrawRange_{2}", file, ParticleAttachment.AbsOrigin, RestartType.None, 0,
                    raze2,
                    1, new Vector3(255, 0, 0), 2, new Vector3(200, 255, 40));
                Particle.AddOrUpdate(Owner, $"DrawRange_{3}", file, ParticleAttachment.AbsOrigin, RestartType.None, 0,
                    raze3,
                    1, new Vector3(255, 0, 0), 2, new Vector3(200, 255, 40));

                var targets = EntityManager<Hero>.Entities.FirstOrDefault(
                    x => x.IsAlive && x.Team != this.Owner.Team &&
                         x.Distance2D(this.Owner) < 1500 && !x.IsIllusion);


                if (targets != null && this.RazeShort.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeShort, targets, GetRazeDelay(targets, RazeShort), true))
                {
                    Particle.AddOrUpdate(Owner, $"DrawRange_{1}", file, ParticleAttachment.AbsOrigin, RestartType.None,
                        0,
                        raze1, 1, new Vector3(0, 255, 0), 2, new Vector3(200, 255, 40));
                }

                if (targets != null && this.RazeMedium.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeMedium, targets, GetRazeDelay(targets, RazeMedium), true))
                {
                    Particle.AddOrUpdate(Owner, $"DrawRange_{2}", file, ParticleAttachment.AbsOrigin, RestartType.None,
                        0,
                        raze2, 1, new Vector3(0, 255, 0), 2, new Vector3(200, 255, 40));
                }

                if (targets != null && this.RazeLong.CanBeCasted() && !UnitExtensions.IsMagicImmune(targets) && !targets.IsInvulnerable() &&
                    CanHit(this.RazeLong, targets, GetRazeDelay(targets, RazeLong), true))
                {
                    Particle.AddOrUpdate(Owner, $"DrawRange_{3}", file, ParticleAttachment.AbsOrigin, RestartType.None,
                        0,
                        raze3, 1, new Vector3(0, 255, 0), 2, new Vector3(200, 255, 40));
                }
            }
            else
            {
                Particle.Remove($"DrawRange_{1}");
                Particle.Remove($"DrawRange_{2}");
                Particle.Remove($"DrawRange_{3}");
            }
        }


        public bool CanHit(Ability ability, Unit targetsama, float customDelay, bool checkForFace = true)
        {
            var modifs = targetsama.HasModifiers(new string[] { "modifier_puck_phase_shift", "modifier_cyclone", "modifier_obsidian_destroyer_astral_imprisonment_prison" }, false);
            if (checkForFace && targetsama.IsValid && targetsama.IsAlive && !targetsama.IsInvulnerable() && !modifs)
            {
                var radius = ability.GetRadius();
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                var predFontPos = UnitExtensions.InFront(Owner, range);
                var input = Input(targetsama, ability, range);
                var output = Prediction.GetPrediction(input);
                output.CastPosition = predFontPos;
                PredictedTargetPosition = output.UnitPosition;
                var hullRadius = targetsama.HullRadius;

                var inRange = PredictedTargetPosition.Distance2D(predFontPos) <= radius + hullRadius;

                return inRange;
            }
            else if (checkForFace && targetsama.IsValid && targetsama.IsAlive && !targetsama.IsInvulnerable() && !modifs)
            {
                var radius = ability.GetRadius();
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                var predFontPos = UnitExtensions.InFront(Owner, range);
                var input = Input(targetsama, ability, range);
                var output = Prediction.GetPrediction(input);
                output.CastPosition = predFontPos;
                PredictedTargetPosition = output.UnitPosition;
                var hullRadius = targetsama.HullRadius;
                if (targetsama.IsMoving)
                {
                    hullRadius += 20;
                }
                var inRange = PredictedTargetPosition.Distance2D(predFontPos) <= radius + hullRadius;

                return inRange;
            }
            else
            {
                var radius = ability.GetAbilitySpecialData("shadowraze_radius");
                var range = ability.GetAbilitySpecialData("shadowraze_range");
                PredictedTargetPosition = targetsama.Position;
                var dist = PredictedTargetPosition.Distance2D(Owner);
                var inRange = dist <= (range + radius + targetsama.HullRadius) &&
                              dist >= (range - radius - targetsama.HullRadius);

                return inRange;
            }
        }

        /*protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive || this.RazeShort.Level <= 0)
            {
                await Task.Delay(125, token);
                return;
            }

            try
            {
                var damage = this.RazeShort.GetAbilitySpecialData("shadowraze_damage");
                var stackDamage = this.RazeShort.GetAbilitySpecialData("stack_bonus_damage");

                var killstealTarget = EntityManager<Hero>.Entities.FirstOrDefault(
                    x => x.IsAlive && x.Team != this.Owner.Team && x.Distance2D(this.Owner) < 900 && !x.IsIllusion);

                

                if (killstealTarget != null)
                {
                    var modifier = killstealTarget.FindModifier(RazeModifierName);

                    if (modifier != null && IsRazeModifier)
                    {
                        damage += modifier.StackCount * stackDamage;
                    }

                }

                var talent = Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner,
                    AbilityId.special_bonus_unique_nevermore_2);
                if (talent?.Level > 0)
                {
                    damage += 150;
                }
                // todo: fix
                if (killstealTarget != null)
                {
                    damage *= 1 - killstealTarget.MagicDamageResist;
                    if (killstealTarget.Health < damage && this.Owner.CanCast())
                    {
                        this.Owner.MoveToDirection(killstealTarget.Position);

                        foreach (
                            var raze in
                            Razes.OrderByDescending(
                                x =>
                                    Ensage.SDK.Extensions.EntityExtensions.Distance2D(killstealTarget,
                                        UnitExtensions.InFront(Owner, x.GetCastRange()))))
                        {
                            if (raze != null && killstealTarget.IsAlive && raze.CanBeCasted() &&
                                CanHit(raze, killstealTarget, GetRazeDelay(killstealTarget, raze)))
                            {
                                raze.UseAbility();
                                var castDelay = GetRazeDelay(killstealTarget, raze);
                                await this.AwaitKillstealDelay(castDelay + 150, token);
                            }
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                Log.Debug(e);
            }
        }*/

        private void OnRazeAdded(Unit sender, ModifierChangedEventArgs args)
        {
            if ((sender is Hero) && this.Owner.Team != sender.Team &&
                (args.Modifier.Name == "modifier_nevermore_shadowraze_counter"))
            {
                //Log.Debug($"Raze modifier added");
                this.IsRazeModifier = true;
            }
        }

        private void OnRazeRemoved(Unit sender, ModifierChangedEventArgs args)
        {
            if (this.Owner.Team != sender.Team && (args.Modifier.Name == "modifier_nevermore_shadowraze_counter"))
            {
                this.IsRazeModifier = false;
            }
        }

        private void OnDraw(IRenderer renderer)
        {
            if (this.BlackKingBar == null)
            {
                return;
            }

            Vector2 screenPos;
            var drawPos = new Vector2(DrawingX, DrawingY);
            var mousePos = Game.MousePosition;
            var value = BkbToggle.Value.Active;

            if (Drawing.WorldToScreen(mousePos, out screenPos))
            {
                this.Context.Renderer.DrawText(drawPos, $"Bkb Toggle: {value.ToString()}", Color.Red, 30);
            }
        }

        public PredictionInput Input(Unit predTarget, Ability predAbility, float castRange)
        {
            return new PredictionInput(Owner, predTarget, GetRazeDelay(predTarget, predAbility) / 1000f,
                float.MaxValue, predAbility.GetAbilitySpecialData("shadowraze_range"), 250, PredictionSkillshotType.SkillshotCircle, true, null, true);
        }


        public virtual int GetRazeDelay(Unit targetto, Ability ability)
        {
            return
                (int)
                (((ability.GetCastPoint(ability.Level - 1) + Owner.TurnTime(targetto.NetworkPosition)) * 1000.0f) +
                 Game.Ping);
        }
    }
}