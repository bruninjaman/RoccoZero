using System;
using BAIO.Heroes.Modes.Combo;
using BAIO.Interfaces;
using BAIO.Modes;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.SDK.Abilities.Items;
using Ensage.SDK.Inventory.Metadata;
using Ensage.SDK.Menu;
using log4net;
using PlaySharp.Toolkit.Helper.Annotations;
using PlaySharp.Toolkit.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BAIO.UnitManager;
using Ensage.Common;
using Ensage.Common.Enums;
using Ensage.Common.Menu.MenuItems;
using Ensage.SDK.Abilities.npc_dota_hero_pangolier;
using Ensage.SDK.Abilities.npc_dota_hero_rubick;
using Ensage.SDK.Extensions;
using Ensage.SDK.Handlers;
using Ensage.SDK.Helpers;
using Ensage.SDK.Localization;
using Ensage.SDK.Prediction;
using Ensage.SDK.Service;
using EnsageSharp.Sandbox;
using Newtonsoft.Json;
using SharpDX;
using SharpDX.Direct2D1.Effects;
using EntityExtensions = Ensage.Common.Extensions.EntityExtensions;
using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;
using AbilityId = Ensage.AbilityId;

namespace BAIO.Heroes.Base
{
    public class AbilityStolenInfo
    {
        public AbilityStolenInfo(Ability ability)
        {
            this.AbilityId = ability.Id;
            this.GameTime = Game.GameTime;
            this.Cooldown = ability.Cooldown;

            if (this.Cooldown <= 0)
            {
                this.Cooldown = ability.CooldownLength;
            }
        }

        public AbilityId AbilityId { get; private set; }

        public float Cooldown { get; private set; }

        public float GameTime { get; private set; }

        public void Update(Ability ability)
        {
            this.AbilityId = ability.Id;
            this.GameTime = Game.GameTime;
            this.Cooldown = ability.Cooldown;

            if (this.Cooldown <= 0)
            {
                this.Cooldown = ability.CooldownLength;
            }
        }
    }

    [PublicAPI]
    [ExportHero(HeroId.npc_dota_hero_base)]
    internal class GenericHero : BaseHero
    {
        internal enum DisabledState
        {
            NotDisabled,
            AlreadyDisabled,
            UsedAbilityToDisable
        }

        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Abilities

        private static readonly AbilityId[] SpecialAbilities =
        {
            AbilityId.enigma_black_hole,
            AbilityId.earthshaker_echo_slam,
            AbilityId.tidehunter_ravage,
            AbilityId.treant_overgrowth,
            AbilityId.venomancer_poison_nova,
            AbilityId.faceless_void_chronosphere,
        };

        private static readonly AbilityId[] PriorityAbilityIds =
            {
                // ultimates
                AbilityId.enigma_black_hole,
                AbilityId.magnataur_reverse_polarity,
                AbilityId.luna_eclipse,
                AbilityId.earthshaker_echo_slam,
                AbilityId.juggernaut_omni_slash,
                AbilityId.death_prophet_exorcism,
                AbilityId.warlock_rain_of_chaos,
                AbilityId.zuus_thundergods_wrath,
                AbilityId.skywrath_mage_mystic_flare,
                AbilityId.queenofpain_sonic_wave,
                AbilityId.obsidian_destroyer_sanity_eclipse,
                AbilityId.venomancer_poison_nova,
                AbilityId.doom_bringer_doom,
                AbilityId.crystal_maiden_freezing_field,
                AbilityId.dark_seer_wall_of_replica,
                AbilityId.omniknight_guardian_angel,
                AbilityId.razor_eye_of_the_storm,
                AbilityId.antimage_mana_void,
                AbilityId.alchemist_chemical_rage,
                AbilityId.bane_fiends_grip,
                AbilityId.kunkka_ghostship,
                AbilityId.necrolyte_reapers_scythe,
                AbilityId.phoenix_supernova,
                AbilityId.puck_dream_coil,
                AbilityId.winter_wyvern_winters_curse,
                AbilityId.pugna_life_drain,

                // special
                AbilityId.pudge_meat_hook,

                // disables
                AbilityId.earthshaker_fissure,
                AbilityId.lion_impale,
                AbilityId.sandking_burrowstrike,
                AbilityId.tiny_avalanche,
                AbilityId.sven_storm_bolt,
                AbilityId.skeleton_king_hellfire_blast,
                AbilityId.vengefulspirit_magic_missile,
                AbilityId.chaos_knight_chaos_bolt,
                AbilityId.slardar_slithereen_crush,

                // others
                AbilityId.spirit_breaker_charge_of_darkness,
                AbilityId.spirit_breaker_nether_strike,
            };

        private readonly Dictionary<Hero, Ability> castedAbilities = new Dictionary<Hero, Ability>(5);

        private readonly HashSet<AbilityStolenInfo> stolenInfos = new HashSet<AbilityStolenInfo>();

        private Tuple<Ability, float> nextStealAbility;

        protected readonly AbilityId[] ItemList =
        {
            AbilityId.item_diffusal_blade,
            AbilityId.item_medallion_of_courage,
            AbilityId.item_solar_crest,
            AbilityId.item_veil_of_discord,
            AbilityId.item_ethereal_blade,
            AbilityId.item_urn_of_shadows,
            AbilityId.item_dagon,
            AbilityId.item_dagon_2,
            AbilityId.item_dagon_3,
            AbilityId.item_dagon_4,
            AbilityId.item_dagon_5,
            AbilityId.item_shivas_guard,
            AbilityId.item_blade_mail,
            AbilityId.item_satanic,
            AbilityId.item_mjollnir,
            AbilityId.item_manta,
            AbilityId.item_black_king_bar
        };

        protected readonly AbilityId[] DisableItemList =
        {
            AbilityId.item_heavens_halberd,
            AbilityId.item_orchid,
            AbilityId.item_bloodthorn,
            AbilityId.item_sheepstick,
            AbilityId.item_abyssal_blade,
            AbilityId.item_rod_of_atos,
            AbilityId.item_nullifier,
            AbilityId.item_cyclone
        };


        public readonly List<AbilityId> DisableAbilities = new List<AbilityId>()
        {
            AbilityId.centaur_hoof_stomp,
            AbilityId.chaos_knight_chaos_bolt,
            AbilityId.crystal_maiden_frostbite,
            AbilityId.enigma_malefice,
            AbilityId.ogre_magi_fireblast,
            AbilityId.ogre_magi_unrefined_fireblast,
            AbilityId.sven_storm_bolt,
            AbilityId.vengefulspirit_magic_missile,
            AbilityId.skeleton_king_hellfire_blast,
            AbilityId.lion_impale,
            AbilityId.lion_voodoo,
            AbilityId.sandking_burrowstrike,
            AbilityId.shadow_shaman_shackles,
            AbilityId.shadow_shaman_voodoo,
            AbilityId.beastmaster_primal_roar,
            AbilityId.bane_fiends_grip,
            AbilityId.nyx_assassin_impale,
            AbilityId.naga_siren_ensnare,
            AbilityId.meepo_earthbind,
            AbilityId.earthshaker_fissure,
        };

        public readonly Dictionary<AbilityId, float> EulAbilities = new Dictionary<AbilityId, float>
        {
            {AbilityId.lina_light_strike_array, 0.95f },
            {AbilityId.leshrac_split_earth,  1.05f},
            {AbilityId.pugna_nether_blast, 1.1f },
            {AbilityId.jakiro_ice_path, 1.15f },
            {AbilityId.death_prophet_silence, 0.4f }
        };

        #endregion

        #region Items

        [ItemBinding] public item_abyssal_blade AbyssalBlade { get; private set; }

        [ItemBinding] public item_manta Manta { get; private set; }

        [ItemBinding] public item_nullifier Nullifier { get; private set; }

        [ItemBinding] public item_cyclone Euls { get; private set; }

        [ItemBinding] public item_diffusal_blade DiffusalBlade { get; private set; }

        [ItemBinding] public item_invis_sword ShadowBlade { get; private set; }

        [ItemBinding] public item_silver_edge SilverEdge { get; private set; }

        [ItemBinding] public item_blink BlinkDagger { get; private set; }

        [ItemBinding] public item_bloodthorn BloodThorn { get; private set; }

        [ItemBinding] public item_black_king_bar BlackKingBar { get; set; }

        [ItemBinding] public item_orchid Orchid { get; private set; }

        [ItemBinding] public item_mjollnir Mjollnir { get; private set; }

        [ItemBinding] public item_force_staff ForceStaff { get; private set; }

        [ItemBinding] public item_ethereal_blade EtherealBlade { get; private set; }

        [ItemBinding] public item_veil_of_discord VeilOfDiscord { get; private set; }

        [ItemBinding] public item_shivas_guard ShivasGuard { get; private set; }

        [ItemBinding] public item_sheepstick Sheepstick { get; private set; }

        [ItemBinding] public item_rod_of_atos RodOfAtos { get; private set; }

        [ItemBinding] public item_urn_of_shadows Urn { get; private set; }

        [ItemBinding] public item_spirit_vessel Vessel { get; private set; }

        [ItemBinding] public item_lotus_orb Lotus { get; private set; }

        [ItemBinding] public item_solar_crest SolarCrest { get; private set; }

        [ItemBinding] public item_blade_mail BladeMail { get; private set; }

        [ItemBinding] public item_medallion_of_courage Medallion { get; private set; }

        [ItemBinding] public item_heavens_halberd HeavensHalberd { get; private set; }

        [ItemBinding] public item_satanic Satanic { get; private set; }

        [ItemBinding] public item_mask_of_madness Mom { get; private set; }

        [ItemBinding] public item_power_treads Treads { get; private set; }

        #endregion

        #region MenuItems

        
        public MenuFactory CameraMover;
        public MenuFactory SpellSteal;
        public MenuFactory Specials;
        

        public MenuFactory BlinkMenu;
        public MenuItem<bool> UseBlink;
        public MenuItem<bool> PrioritizeAbilities;
        public MenuItem<bool> PrioritizeBlink;
        public MenuItem<Slider> MinimumBlinkRange;
        public MenuItem<Slider> BlinkDistance2Target;

        public MenuItem<Slider> MaximumTargetDistance;

        public MenuItem<AbilityToggler> KillstealAbilityToggler;

        public MenuItem<AbilityToggler> AbilitiesBeforeBlink;

        public MenuItem<PriorityChanger> AbilityPriorityChanger;
        public MenuItem<AbilityToggler> AbilityAbilityToggler;

        public MenuItem<PriorityChanger> ItemPriorityChanger;
        public MenuItem<AbilityToggler> ItemAbilityToggler;

        public MenuItem<PriorityChanger> DisablePriorityChanger;
        public MenuItem<AbilityToggler> DisableAbilityToggler;
        public MenuItem<bool> StackDisables;
        public MenuItem<bool> CameraMoverEnabled;
        #endregion

        public Dictionary<string, bool> AbilityDictionary;

        public List<Ability> Abilities { get; set; }

        public pangolier_swashbuckle Swashbuckle { get; set; }

        public Dictionary<Ability, DamageType> KillstealAbilities { get; } = new Dictionary<Ability, DamageType>();

        //public Dictionary<int, string> Translations { get; set; } 

        public TaskHandler StealHandler { get; private set; }
        public TaskHandler StealHandler2 { get; private set; }

        public rubick_spell_steal Steal { get; private set; }

        public Ability StolenAbility
        {
            get
            {
                return this.Owner.Spellbook.SpellD;
            }
        }

        private int GetAbilityPriority(Ability ability)
        {
            for (var i = 0; i < PriorityAbilityIds.Length; i++)
            {
                if (PriorityAbilityIds[i] == ability.Id)
                {
                    return i;
                }
            }

            // TODO: maybe better logic for other spells
            var priority = PriorityAbilityIds.Length + 1;
            if (ability.AbilityType != AbilityType.Ultimate)
            {
                priority += 10;
                //if (!this.Owner.HasAghanimsScepter())
                //{
                //    return int.MaxValue;
                //}
            }

            return priority;
        }

        private bool HasHigherPriority(Ability ability, Ability testAbility)
        {
            return this.GetAbilityPriority(ability) > this.GetAbilityPriority(testAbility);
        }


        protected override ComboMode GetComboMode()
        {
            return new GenericHeroCombo(this);
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive)
            {
                await Task.Delay(250, token);
                return;
            }

            var stealM = this.SpellSteal;

            foreach (var hero in Ensage.Common.Objects.Heroes.GetByTeam(this.Owner.GetEnemyTeam()))
            {
                if (!stealM.Target.Items.Any(x => x.DisplayName == hero.Name))
                {
                    stealM.Item(hero.Name, new AbilityToggler(hero.Spellbook.Spells.Where(x => !x.Name.StartsWith("special_") &&
                                                                                               !x.AbilityBehavior.HasFlag(AbilityBehavior
                                                                                                   .Passive) &&
                                                                                               !x.AbilityBehavior.HasFlag(AbilityBehavior
                                                                                                   .NotLearnable) &&
                                                                                               (!x.AbilityBehavior.HasFlag(AbilityBehavior
                                                                                                    .Toggle) || x.NetworkName ==
                                                                                                "CDOTA_Ability_Leshrac_Pulse_Nova")).ToList().ToDictionary(x => x.Name, y => true)));
                }
            }
        }

        private void AbilityCasted(object sender, AbilityEventArgs e)
        {
            var hero = e.Caster as Hero;
            if ((hero != null) && hero.IsEnemy(this.Owner) && !hero.IsIllusion && hero.HeroId != HeroId.npc_dota_hero_invoker)
            {
                //Log.Debug($"{hero.Name} casted {e.Ability.Ability.Name}");
                this.castedAbilities[hero] = e.Ability.Ability;
            }
        }

        private async Task OnUpdate2(CancellationToken token)
        {
            if (Game.IsPaused || !this.Owner.IsAlive)
            {
                await Task.Delay(250, token);
                return;
            }

            if (StolenAbility.Name == "pangolier_swashbuckle")
            {
                this.Swashbuckle = this.Context.AbilityFactory.GetAbility<pangolier_swashbuckle>();
            }

            var heroesToRemove = this.castedAbilities.Keys
                .Where(hero => !hero.IsValid || !hero.IsVisible || !hero.IsAlive).ToList();
            foreach (var hero in heroesToRemove)
            {
                this.castedAbilities.Remove(hero);
            }

            // Log.Debug($"{this.Owner.Position.X}f, {this.Owner.Position.Y}f, {this.Owner.Position.Z}f ");
            if (!Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
            {
                if (!Ensage.SDK.Extensions.UnitExtensions.IsInvisible(this.Owner) && this.Steal.CanBeCasted)
                {
                    // get best possible steal target
                    var time = Game.GameTime;
                    Ability bestSteal = null;
                    foreach (var castedAbility in this.castedAbilities)
                    {
                        var hero = castedAbility.Key;
                        if (!this.Steal.CanHit(hero))
                        {
                            continue;
                        }

                        var ability = castedAbility.Value;
                        if ((ability.Id != this.StolenAbility.Id) &&
                            ((bestSteal == null) || this.HasHigherPriority(bestSteal, ability)))
                        {
                            // test if ability was already stolen and is still on cd
                            var info = this.stolenInfos.FirstOrDefault(x => x.AbilityId == ability.Id);
                            if ((info == null) || ((time - info.GameTime) > info.Cooldown))
                            {
                                //if (info != null)
                                //{
                                //    //Log.Debug(
                                //    //    $"info: {time} - {info.GameTime} = {time - info.GameTime} > {info.Cooldown}");
                                //}

                                bestSteal = ability;
                            }
                        }
                    }

                    if ((bestSteal != null) && ((this.StolenAbility == null) || (this.StolenAbility.Cooldown > 0) ||
                                                this.HasHigherPriority(this.StolenAbility, bestSteal)))
                    {
                        if ((this.GetAbilityPriority(bestSteal) != int.MaxValue))
                        {
                            if ((this.StolenAbility != null) &&
                                !this.StolenAbility.AbilityBehavior.HasFlag(AbilityBehavior.Passive))
                            {
                                var info = this.stolenInfos.FirstOrDefault(x => x.AbilityId == this.StolenAbility.Id);
                                if (info != null)
                                {
                                    info.Update(this.StolenAbility);
                                    //Log.Debug($"updating info for {this.StolenAbility.Name}");
                                }
                                else
                                {
                                    this.stolenInfos.Add(new AbilityStolenInfo(this.StolenAbility));
                                    //Log.Debug($"adding info for {this.StolenAbility.Name}");
                                }
                            }
                            //Log.Debug($"{IsAbilityEnabled(bestSteal.Id, bestSteal.Owner.Name)}");
                            if (IsAbilityEnabled(bestSteal.Id, bestSteal.Owner.Name))
                            {
                                this.nextStealAbility = new Tuple<Ability, float>(bestSteal, time);
                                var target = (Unit)bestSteal.Owner;
                                //Log.Debug($"stealing {bestSteal} from {target.Name}!");
                                if (this.Steal.UseAbility(target))
                                {
                                    await Task.Delay(this.Steal.GetHitTime(target) + 250, token);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (this.nextStealAbility == null)
            {
                return;
            }

            var time = Game.GameTime;
            if ((time - this.nextStealAbility.Item2) > 2.0f)
            {
                this.nextStealAbility = null;
                return;
            }

            Vector2 screenPos;
            if (Drawing.WorldToScreen(this.Owner.Position + new Vector3(0, 0, this.Owner.HealthBarOffset), out screenPos))
            {
                screenPos += new Vector2(0, -65);
                try
                {
                    var texture = Drawing.GetTexture($"materials/ensage_ui/spellicons/{this.nextStealAbility.Item1.Name}.vmat");
                    Drawing.DrawRect(screenPos, new Vector2(48, 48), texture);
                }
                catch (DotaTextureNotFoundException)
                {
                    Drawing.DrawText($"Stealing {this.nextStealAbility.Item1.Name}", screenPos, Color.Yellow, FontFlags.DropShadow | FontFlags.AntiAlias);
                }
            }
        }

        public bool IsAbilityEnabled(AbilityId id, string heroName)
        {
            return this.SpellSteal.GetValue<AbilityToggler>(heroName).IsEnabled(id.ToString());
        }

        private void EntityManagerOnEntityAdded(object sender, Ability ability)
        {
            if (ability.Owner != this.Owner || ability.Name.Contains("rubick_hidden") || ability.NetworkName.Contains("Item") || !ability.IsStolen)
            {
                return;
            }

            if (!this.Abilities.Contains(ability))
            {
                Abilities.Add(ability);
            }

            if (!this.AbilityAbilityToggler.Value.Dictionary.ContainsKey(ability.Name))
            {
                this.AbilityAbilityToggler.Value.Add(ability.Name, true);
            }

            if (!this.AbilityPriorityChanger.Value.Dictionary.ContainsKey(ability.Name))
            {
                this.AbilityPriorityChanger.Value.Add(ability.Name);
            }

            if (SpecialAbilities.Contains(ability.Id))
            {
                if (this.Specials == null)
                {
                    this.Specials = this.Config.Hero.Factory.Menu("Specials");
                    this.Specials.Item($"{LocalizationHelper.LocalizeAbilityName(ability.Name)}",
                        new Slider(2, 1, 5));
                }
                else
                {
                    this.Specials.Item($"{LocalizationHelper.LocalizeAbilityName(ability.Name)}",
                        new Slider(2, 1, 5));
                }
            }
        }

        private void EntityManagerOnEntityRemoved(object sender, Ability ability)
        {
            if (ability.Owner != this.Owner || ability.Name.Contains("rubick_hidden"))
            {
                return;
            }

            if (this.Abilities.Contains(ability))
            {
                Abilities.Remove(ability);
            }

            if (this.AbilityAbilityToggler.Value.Dictionary.ContainsKey(ability.Name))
            {
                this.AbilityAbilityToggler.Value.Remove(ability.Name);
            }

            if (this.AbilityPriorityChanger.Value.Dictionary.ContainsKey(ability.Name))
            {
                this.AbilityPriorityChanger.Value.Remove(ability.Name);
            }

            if (this.KillstealAbilityToggler.Value.Dictionary.ContainsKey(ability.Name))
            {
                this.KillstealAbilityToggler.Value.Remove(ability.Name);
            }
        }

        public int GetSliderCount(string ability)
        {
            return this.Specials.GetValue<Slider>(LocalizationHelper.LocalizeAbilityName(ability)).Value;
        }


        protected override void OnActivate()
        {
            base.OnActivate();
            this.Abilities = new List<Ability>();
            AbilityDictionary = new Dictionary<string, bool>();

            foreach (var ability in this.Context.Owner.Spellbook.Spells.Where(x => !x.Name.StartsWith("special_") &&
                                                                                   !x.Name.Equals("ability_capture") &&
                                                                                   !x.AbilityBehavior.HasFlag(AbilityBehavior.Passive) &&
                                                                                   !x.AbilityBehavior.HasFlag(AbilityBehavior.NotLearnable) &&
                                                                                   (!x.AbilityBehavior.HasFlag(AbilityBehavior.Toggle) || x.NetworkName == "CDOTA_Ability_Leshrac_Pulse_Nova")
                                                                                   && x.Name != "rubick_spell_steal" && !x.IsStolen))
            {
                AbilityDictionary.Add(ability.Name, true);
                Abilities.Add(ability);
            }
            // attack_factor_tooltip => PA stifling dagger
            // damage_per_health => necro ult
            // echo_slam_initial_damage => echo slam
            foreach (var killstealAbility in Abilities.Where(x => (x.GetDamage(x.Level - 1) > 0) || x.AbilitySpecialData?.FirstOrDefault(y => y.Name == "damage" ||
                                                                                                                                              y.Name == "damage_tooltip" ||
                                                                                                                                              y.Name == "bonus_damage" ||
                                                                                                                                              y.Name == "attack_factor_tooltip" ||
                                                                                                                                              y.Name == "damage_per_health" ||
                                                                                                                                              y.Name == "echo_slam_initial_damage")?.Value > 0))
            {
                KillstealAbilities.Add(killstealAbility, killstealAbility.DamageType);
            }

            var factory = this.Config.Hero.Factory;
            var itemMenu = this.Config.Hero.ItemMenu;

            if (this.Owner.HeroId == HeroId.npc_dota_hero_rubick)
            {
                this.SpellSteal = factory.MenuWithTexture("Spell Steal", "rubick_spell_steal");
                this.Steal = this.Context.AbilityFactory.GetAbility<rubick_spell_steal>();
                this.StealHandler = UpdateManager.Run(OnUpdate);
                this.StealHandler2 = UpdateManager.Run(OnUpdate2);
                EntityManager<Ability>.EntityAdded += this.EntityManagerOnEntityAdded;
                EntityManager<Ability>.EntityRemoved += this.EntityManagerOnEntityRemoved;
                this.Context.AbilityDetector.AbilityCasted += this.AbilityCasted;
                Drawing.OnDraw += this.OnDraw;
            }

            foreach (var ability in this.Abilities)
            {
                if (SpecialAbilities.Contains(ability.Id))
                {
                    this.Specials = factory.Menu("Specials");
                    this.Specials.Item($"{LocalizationHelper.LocalizeAbilityName(ability.Name)}",
                        new Slider(2, 1, 5));
                }
            }
            
            this.BlinkMenu = itemMenu.MenuWithTexture(this.Config.Hero.Translations[1], "item_blink");
            var blinkMenu = this.BlinkMenu;

            this.CameraMover = factory.Menu(this.Config.Hero.Translations[29]);
            var cameraMover = this.CameraMover;

            this.CameraMoverEnabled = cameraMover.Item("Enabled", false);
            this.CameraMoverEnabled.Item.Tooltip = "Will move camera to order position if position is out of screen";

            this.UseBlink = blinkMenu.Item(this.Config.Hero.Translations[9], true);
            this.PrioritizeBlink = blinkMenu.Item(this.Config.Hero.Translations[10], true);
            this.MinimumBlinkRange = blinkMenu.Item(this.Config.Hero.Translations[11], new Slider(200, 0, 1200));
            this.MinimumBlinkRange.Item.Tooltip = this.Config.Hero.Translations[12];
            this.BlinkDistance2Target = blinkMenu.Item(this.Config.Hero.Translations[13], new Slider(1, 1, 1200));
            this.BlinkDistance2Target.Item.Tooltip = this.Config.Hero.Translations[14];

            this.AbilitiesBeforeBlink = blinkMenu.Item(this.Config.Hero.Translations[15], new AbilityToggler(AbilityDictionary));

            this.PrioritizeAbilities = factory.Item(this.Config.Hero.Translations[16], false);
            this.PrioritizeAbilities.Item.Tooltip = this.Config.Hero.Translations[17];

            this.MaximumTargetDistance = factory.Item(this.Config.Hero.Translations[18], new Slider(1500, 1000, 10000));

            this.KillstealAbilityToggler = factory.Item(this.Config.Hero.Translations[19], new AbilityToggler(KillstealAbilities.ToDictionary(value => value.Key.ToString(), value => true)));

            this.AbilityAbilityToggler = factory.Item(this.Config.Hero.Translations[20], new AbilityToggler(AbilityDictionary));

            this.AbilityPriorityChanger = factory.Item(this.Config.Hero.Translations[21],
                new PriorityChanger(AbilityDictionary.Keys.ToList()));

            this.ItemAbilityToggler = itemMenu.Item(this.Config.Hero.Translations[22],
                new AbilityToggler(ItemList
                    .Where(x => x != AbilityId.item_dagon && x != AbilityId.item_dagon_2 && x != AbilityId.item_dagon_3 &&
                                x != AbilityId.item_dagon_4)
                    .ToDictionary(value => value.ToString(), value => true)));

            List<string> items = ItemList.Where(x =>
                    x != AbilityId.item_dagon && x != AbilityId.item_dagon_2 && x != AbilityId.item_dagon_3 &&
                    x != AbilityId.item_dagon_4)
                .Select(x => x.ToString()).ToList();

            this.ItemPriorityChanger = itemMenu.Item(this.Config.Hero.Translations[23], new PriorityChanger(items));

            this.DisableAbilityToggler = itemMenu.Item(this.Config.Hero.Translations[24],
                new AbilityToggler(DisableItemList.ToDictionary(value => value.ToString(), value => true)));

            List<string> disableItems = DisableItemList.Select(x => x.ToString()).ToList();
            this.DisablePriorityChanger =
                itemMenu.Item(this.Config.Hero.Translations[25], new PriorityChanger(disableItems));

            this.StackDisables = itemMenu.Item(this.Config.Hero.Translations[26], false);
            this.StackDisables.Item.Tooltip = this.Config.Hero.Translations[27];

            this.Updater = new Updater(this);

            if (this.Owner.HeroId == HeroId.npc_dota_hero_pangolier)
            {
                this.Swashbuckle = this.Context.AbilityFactory.GetAbility<pangolier_swashbuckle>();
            }

            Game.PrintMessage($"{LocalizationHelper.LocalizeName(this.Owner.HeroId) + " " + this.Config.Hero.Translations[28]}");
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            EntityManager<Ability>.EntityAdded -= this.EntityManagerOnEntityAdded;
            EntityManager<Ability>.EntityRemoved -= this.EntityManagerOnEntityRemoved;
            this.Updater.Dispose();
            this.StealHandler?.Cancel();
            this.StealHandler2?.Cancel();
            this.Context.AbilityDetector.AbilityCasted -= this.AbilityCasted;
            Drawing.OnDraw -= this.OnDraw;
        }

        public async Task<bool> MoveOrBlinkToEnemy(Unit target, CancellationToken token = default)
        {
            var blink = this.Owner.Inventory.Items.FirstOrDefault(k => k.Id == Ensage.AbilityId.item_blink);
            if (blink == null || !blink.CanBeCasted())
            {
                return true;
            }

            var l = (EntityExtensions.Distance2D(this.Owner, target) - BlinkDistance2Target) / BlinkDistance2Target;
            var posA = this.Owner.Position;
            var posB = target.Position;
            var x = (posA.X + (l * posB.X)) / (1 + l);
            var y = (posA.Y + (l * posB.Y)) / (1 + l);
            var position = new Vector3((int) x, (int) y, posA.Z);

            var distance = EntityExtensions.Distance2D(this.Owner, position);

            if (distance <= this.MinimumBlinkRange)
            {
                return false;
            }

            if (this.Owner.UnitState != UnitState.Muted)
            {
                if (this.UseBlink)
                {
                    if (blink != null && blink.CanBeCasted() && !Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
                    {
                        var blinkRange = blink.AbilitySpecialData.First(f => f.Name == "blink_range").Value;
                        if (distance <= blinkRange && distance >= this.MinimumBlinkRange)
                        {
                            if (target.IsMoving)
                            {
                                var movesPosition = Vector3Extensions.Extend(position,
                                    Ensage.Common.Extensions.UnitExtensions.InFront(target, 50), 50);
                                if (blink.UseAbility(movesPosition))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                    return false;
                                }
                            }
                            else
                            {
                                if (blink.UseAbility(position))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                    return false;
                                }
                                
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }

                var phaseBoots = this.Owner.Inventory.Items.FirstOrDefault(z => z.Name == "item_phase_boots");
                if (phaseBoots != null && phaseBoots.CanBeCasted() && this.Owner.UnitState != UnitState.Invisible)
                {
                    if (phaseBoots.UseAbility())
                    {
                        await Task.Delay(50 + (int)(Game.Ping), token);
                        return false;
                    }
                }
            }

            return false;
        }

        public bool CastingChecks(string name, Unit hero, Unit target, Ability ability = null)
        {
            switch (name)
            {
                case "bloodseeker_bloodrage" when Ensage.SDK.Extensions.UnitExtensions.HasModifier(hero, "modifier_bloodseeker_bloodrage"):
                case "axe_battle_hunger" when Ensage.SDK.Extensions.UnitExtensions.HasModifier(target, "modifier_axe_battle_hunger"):
                case "bounty_hunter_track" when Ensage.SDK.Extensions.UnitExtensions.HasModifier(target, "modifier_bounty_hunter_track"):
                case "lycan_summon_wolves" when Updater.AllUnits.Any(x => x.Unit.Name.Contains("npc_dota_lycan_wolf")):
                case "abaddon_aphotic_shield" when Ensage.SDK.Extensions.UnitExtensions.HasModifier(hero, "modifier_abaddon_aphotic_shield"):
                    return false;
                case "antimage_mana_void" when ability != null:
                {
                    var damage = ability.GetAbilitySpecialData("mana_void_damage_per_mana");
                    var amp = ability.SpellAmplification();
                    var manaDamage = (target.MaximumMana - target.Mana) * damage;

                    var reduction = ability.GetDamageReduction(target, DamageType.Magical);
                    if (DamageHelpers.GetSpellDamage(manaDamage, amp, reduction) < target.Health)
                    {
                        return false;
                    }

                    break;
                }
                case "sniper_shrapnel" when ability != null:
                {
                    var modif = target.FindModifier("modifier_sniper_shrapnel_slow");
                    if (modif != null || ability.IsInAbilityPhase)
                    {
                        return false;
                    }

                    break;
                }
                case "axe_culling_blade" when ability != null:
                {
                    var threshold = ability.GetAbilitySpecialData("kill_threshold");
                    if (target.Health > threshold)
                    {
                        return false;
                    }

                    break;
                }
                case "lion_mana_drain" when ability != null:
                {
                    var manaLow = (float)(this.Owner.Mana / this.Owner.MaximumMana) * 100f;
                    if (manaLow >= 15)
                    {
                        return false;
                    }

                    break;
                }
                case "visage_soul_assumption" when ability != null:
                {
                    var modif = this.Owner.FindModifier("modifier_visage_soul_assumption");
                    if (modif == null || modif.StackCount < ability.GetAbilityData("stack_limit"))
                    {
                        return false;
                    }

                    break;
                }
                case "skeleton_king_mortal_strike" when ability != null:
                {
                    var modif = this.Owner.FindModifier("modifier_skeleton_king_mortal_strike");
                    if (modif == null || modif.StackCount < ability.GetAbilityData("max_skeleton_charges"))
                    {
                        return false;
                    }

                    break;
                }
                case "centaur_return" when ability != null:
                {
                    var modif = this.Owner.FindModifier("modifier_centaur_return_counter");
                    if (modif == null || modif.StackCount < ability.GetAbilitySpecialData("max_stacks"))
                    {
                        return false;
                    }

                    break;
                }
                case "monkey_king_boundless_strike" when ability != null:
                {
                    var modif = this.Owner.FindModifier("modifier_monkey_king_quadruple_tap_bonuses");
                    if (modif == null || modif.StackCount < 1)
                    {
                        return false;
                    }

                    break;
                }
                case "alchemist_unstable_concoction_throw" when ability != null:
                {
                    var modif = this.Owner.FindModifier("modifier_alchemist_unstable_concoction");
                    if (modif == null || modif.ElapsedTime < 4.5f)
                    {
                        return false;
                    }

                    break;
                }
                case "omniknight_purification" when ability != null:
                {
                    var distance = this.Owner.Distance2D(target) <= GetAbilityRadius(ability);
                    if (!distance)
                    {
                        return false;
                    }

                    break;
                }
                case "templar_assassin_meld" when ability != null:
                {
                    var distance = this.Owner.Distance2D(target) <= this.Owner.GetAttackRange() - 85f;
                    if (!distance)
                    {
                        return false;
                    }

                    break;
                }
                case "mirana_leap" when ability != null:
                {
                    var distance = this.Owner.Distance2D(target) >= 500;
                    if (!distance)
                    {
                        return false;
                    }
                    else
                    {
                        if (hero.MoveToDirection(target.Position))
                        {
                            return true;
                        }
                    }

                    break;
                }
                case "pangolier_gyroshell" when ability != null:
                {
                    var modif = hero.FindModifier("modifier_pangolier_gyroshell");
                    if (modif != null)
                    {
                        return false;
                    }

                    break;
                }
                case "pangolier_shield_crash" when ability != null:
                {
                    var modif = hero.FindModifier("modifier_pangolier_gyroshell");
                    if (modif != null && hero.Distance2D(target) < 250)
                    {
                        return false;
                    }
                    if (Ensage.SDK.Extensions.UnitExtensions.InFront(hero, 250).Distance2D(target) >= 450)
                    {
                        return false;
                    }

                    break;
                }
                case "tidehunter_ravage" when ability != null:
                {
                    // todo: Original range is 1250. Travel speed is 750. Find a way to check if enemies can leave the area.
                    var enemyCount = hero.GetEnemiesInRange<Hero>(1000).Count();
                    if (enemyCount < GetSliderCount(ability.Name))
                    {
                        return false;
                    }

                    break;
                }
                case "earthshaker_echo_slam" when ability != null:
                {
                    var enemyCount = hero.GetEnemiesInRange<Hero>(600).Count();
                    if (enemyCount < GetSliderCount(ability.Name) || Ensage.SDK.Extensions.UnitExtensions.IsMagicImmune(target))
                    {
                        return false;
                    }

                    break;
                }
                case "treant_overgrowth" when ability != null:
                {
                    var enemyCount = hero.GetEnemiesInRange<Hero>(GetAbilityRadius(ability)).Count();
                    if (enemyCount < GetSliderCount(ability.Name))
                    {
                        return false;
                    }

                    break;
                }
                case "venomancer_poison_nova" when ability != null:
                {
                    var enemyCount = hero.GetEnemiesInRange<Hero>(GetAbilityRadius(ability)).Count();
                    if (enemyCount < GetSliderCount(ability.Name))
                    {
                        return false;
                    }

                    break;
                }
                case "lina_dragon_slave" when ability != null:
                {
                    var eulsReady = this.Euls != null && this.Euls.CanBeCasted;
                    if (eulsReady || target.IsInvul())
                    {
                        return false;
                    }

                    break;
                }
            }

            if (Ensage.SDK.Extensions.UnitExtensions.HasModifier(hero, "modifier_templar_assassin_meld"))
            {
                return false;
            }

            return true;
        }

        public bool LegionCommanderPressTheAttack
        {
            get
            {
                return Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner,
                           AbilityId.special_bonus_unique_legion_commander_5) != null && Ensage.SDK.Extensions
                           .UnitExtensions
                           .GetAbilityById(this.Owner, AbilityId.special_bonus_unique_legion_commander_5).Level > 0;
            }
        }

        public bool AxeAghs
        {
            get
            {
                return this.Owner.HasAghanimsScepter();

            }
        }

        public async Task UseGenericAbilities(Unit target, CancellationToken token = default, float minimumTime = 0)
        {
            var isOwnerInvisBased = this.Owner.HeroId == HeroId.npc_dota_hero_riki ||
                                    this.Owner.HeroId == HeroId.npc_dota_hero_sand_king;

            if (this.Owner.UnitState == UnitState.Silenced)
            {
                return;
            }

            if ((!isOwnerInvisBased && this.Owner.UnitState == UnitState.Invisible))
            {
                return;
            }

            if (Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
            {
                return;
            }

            List<KeyValuePair<string, uint>> itemChanger = this.AbilityPriorityChanger.Value.Dictionary.Where(
                    x => this.AbilityAbilityToggler.Value.IsEnabled(x.Key))
                .OrderByDescending(x => x.Value)
                .ToList();
            // spaghetti incoming
            foreach (var order in itemChanger)
            {
                foreach (var ability in Abilities.Where(x => x != null && x.IsActivated && !x.IsHidden))
                {
                    if (!Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner) && !Ensage.SDK.Extensions.UnitExtensions.IsSilenced(this.Owner))
                    {
                        if (ability != null && ability.IsActivated
                        && AbilityExtensions.CanBeCasted(ability)
                        && Extensions.CanHit(ability, target)
                        && ability.ToString() == order.Key
                        && (ability.TargetTeamType.HasFlag(TargetTeamType.Enemy) || ability.TargetTeamType.HasFlag(TargetTeamType.Custom) ||
                            ability.TargetTeamType.HasFlag(TargetTeamType.None))
                        && (this.Owner.Distance2D(target) <= GetAbilityCastRange(ability) ||
                            this.Owner.Distance2D(target) <= GetAbilityRadius(ability)) ||
                        (AbilityExtensions.CanBeCasted(ability) && !this.PrioritizeBlink && this.AbilitiesBeforeBlink.Value.IsEnabled(ability.Name)))
                        {
                            float duration = 0;
                            if ((target.IsHexed(out duration) || target.IsStunned(out duration) ||
                                 target.UnitState == UnitState.Silenced) &&
                                DisableAbilities.Contains(ability.Id)
                                && duration >= minimumTime)
                            {
                                continue;
                            }

                            var eul = Ensage.SDK.Extensions.UnitExtensions.GetItemById(this.Owner,
                                AbilityId.item_cyclone);
                            var eulCombo = this.DisableAbilityToggler.Value.IsEnabled("item_cyclone") && eul != null && eul.CanBeCasted() &&
                                           (!Ensage.SDK.Extensions.UnitExtensions.IsHexed(target) && !Ensage.SDK.Extensions.UnitExtensions.IsStunned(target));
                            if (EulAbilities.ContainsKey(ability.Id) && (eulCombo || target.UnitState.HasFlag(UnitState.Invulnerable)))
                            {
                                continue;
                            }

                            PositionCamera(target);

                            if (ability.Name == "legion_commander_press_the_attack" && LegionCommanderPressTheAttack)
                            {
                                if (ability.UseAbility(this.Owner.Position))
                                {
                                    await Task.Delay(GetAbilityCastDelay(ability, this.Owner, this.Owner), token);
                                }
                            }

                            if (ability.Name == "axe_battle_hunger" && AxeAghs)
                            {
                                if (ability.UseAbility(target.Position))
                                {
                                    await Task.Delay(GetAbilityCastDelay(ability, this.Owner, target), token);
                                }
                            }

                            if (ability.Name == "earthshaker_enchant_totem" && AxeAghs)
                            {
                                var otherTargets = EntityManager<Hero>.Entities
                                    .Where(x => x != null && x.IsValid && x.IsAlive && x.Team != this.Owner.Team &&
                                                x.Distance2D(target) <= ability.GetRadius() * 1.4).ToList();
                                var input = new PredictionInput(Owner, target,
                                        1,
                                        float.MaxValue,
                                        GetAbilityCastRange(ability),
                                        300,
                                        PredictionSkillshotType.SkillshotCircle,
                                        true,
                                        otherTargets)
                                    { Delay = 1 };
                                var output = this.Context.Prediction.GetPrediction(input);
                                if (output.HitChance >= HitChance.Medium)
                                {
                                    if (ability.UseAbility(output.CastPosition))
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                }
                            }

                            if (ability.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget) && CastingChecks(ability.Name, this.Owner, target, ability))
                            {
                                if (ability.AbilityBehavior.HasFlag(AbilityBehavior.Toggle) && this.Owner.Distance2D(target) <= GetAbilityRadius(ability)
                                    && !ability.IsToggled && ability.Cooldown <= 0)
                                {
                                    if (ability.ToggleAbility())
                                    {
                                        await Task.Delay(200, token);
                                    }
                                }
                                else if (!ability.AbilityBehavior.HasFlag(AbilityBehavior.Toggle))
                                {
                                    if (ability.UseAbility())
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                }
                            }
                            else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget) && CastingChecks(ability.Name, this.Owner, target, ability))
                            {
                                if ((ability.TargetTeamType.HasFlag(TargetTeamType.Enemy) || ability.TargetTeamType.HasFlag(TargetTeamType.Custom)))
                                {
                                    if (ability.UseAbility(target))
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                    if (ability.UseAbility()) // night stalker fix?
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    } 
                                }
                                else
                                {
                                    //todo
                                    if (ability.Name == "dazzle_shadow_wave" && Updater.AllUnits != null)
                                    {
                                        if (ability.UseAbility(Updater.AllUnits
                                            .FirstOrDefault(x => x.Unit.Distance2D(target) <= 200).Unit))
                                        {
                                            await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                        }
                                    }
                                    if (ability.Name != "furion_sprout")
                                    {
                                        if (ability.UseAbility(this.Owner))
                                        {
                                            await Task.Delay(GetAbilityCastDelay(ability, Owner, this.Owner), token);
                                        }
                                    }
                                }
                            }
                            else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.AreaOfEffect) && CastingChecks(ability.Name, this.Owner, target, ability))
                            {
                                var otherTargets = EntityManager<Hero>.Entities
                                    .Where(x => x != null && x.IsValid && x.IsAlive && x.Team != this.Owner.Team &&
                                                x.Distance2D(target) <= ability.GetRadius() * 1.4).ToList();
                                var input = new PredictionInput(Owner, target,
                                        (int)GetAbilityCastDelay(ability, this.Owner, target) / 1000f,
                                        GetAbilitySpeed(ability),
                                        GetAbilityCastRange(ability),
                                        GetAbilityRadius(ability),
                                        PredictionSkillshotType.SkillshotCircle,
                                        true,
                                        otherTargets)
                                { Delay = Math.Max(GetAbilityCastDelay(ability, this.Owner, target), 0) / 1000f };
                                var output = this.Context.Prediction.GetPrediction(input);
                                var amount = output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.Team != this.Owner.Team) + 1;
                                if (output.HitChance >= HitChance.Medium)
                                {
                                    if (ability.Name == "faceless_void_chronosphere" ||
                                        ability.Name == "enigma_black_hole")
                                    {
                                        if (GetSliderCount(ability.Name) <= amount)
                                        {
                                            if (ability.UseAbility(output.CastPosition))
                                            {
                                                await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                            }
                                        }
                                    }
                                    else if (ability.UseAbility(output.CastPosition))
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                }
                            }
                            else if ((ability.AbilityBehavior.HasFlag(AbilityBehavior.Directional) || ability.Id == AbilityId.pudge_meat_hook)
                                     && CastingChecks(ability.Name, this.Owner, target, ability))
                            {
                                var otherTargets = EntityManager<Hero>.Entities
                                    .Where(x => x != null && x.IsValid && x.IsAlive && x.Team != this.Owner.Team &&
                                                x.Distance2D(target) <= ability.GetRadius() * 1.4).ToList();
                                var input = new PredictionInput(Owner, target,
                                        (int)GetAbilityCastDelay(ability, this.Owner, target) / 1000f,
                                        GetAbilitySpeed(ability),
                                        GetAbilityCastRange(ability),
                                        GetAbilityRadius(ability),
                                        PredictionSkillshotType.SkillshotLine,
                                        true,
                                        otherTargets)
                                    { Delay = Math.Max(GetAbilityCastDelay(ability, this.Owner, target), 0) / 1000f };
                                var output = this.Context.Prediction.GetPrediction(input);
                                if (output.HitChance >= HitChance.Medium)
                                {
                                    if (ability.UseAbility(output.CastPosition))
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                }
                            }
                            else if ((ability.AbilityBehavior.HasFlag(AbilityBehavior.Point) && ability.Id != AbilityId.pudge_meat_hook) && CastingChecks(ability.Name, this.Owner, target, ability))
                            {
                                if (ability.Name != "pangolier_swashbuckle")
                                {
                                    if (ability.UseAbility(target.Position))
                                    {
                                        await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                    }
                                }
                                else
                                {
                                    if (this.Swashbuckle.UseAbility(target.Position))
                                    {
                                        await Task.Delay(this.Swashbuckle.GetCastDelay(), token);
                                    }
                                }
                            }
                        }
                        else if (ability != null
                                 && AbilityExtensions.CanBeCasted(ability)
                                 && ability.TargetTeamType.HasFlag(TargetTeamType.Allied)
                                 && CastingChecks(ability.Name, this.Owner, target, ability)
                                 && ability.Name != "furion_sprout"
                                 && ability.ToString() == order.Key)
                        {
                            if (ability.UseAbility(this.Owner))
                            {
                                await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                            }
                        }
                        else if (ability != null && ability.CanBeCasted() && ability.IsActivated &&
                                 ability.ToString() == order.Key &&
                                 CastingChecks(ability.Name, this.Owner, target, ability) &&
                                 ability.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (GetAbilityRadius(ability) > 0 && this.Owner.Distance2D(target) <= GetAbilityRadius(ability) - 70)
                            {
                                if (ability.UseAbility())
                                {
                                    await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                }
                            }
                            else if (GetAbilityRadius(ability) <= 0)
                            {
                                if (ability.UseAbility())
                                {
                                    await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                }
                            }
                        }

                        if (ability != null && ability.CanBeCasted() && ability.IsActivated &&
                                 ability.ToString() == order.Key &&
                                 ability.TargetType.HasFlag(TargetType.Tree))
                        {
                            var tree = EntityManager<Tree>.Entities.FirstOrDefault(unit => unit != null && unit.IsValid && unit.IsVisible && unit.IsAlive &&
                                Ensage.SDK.Extensions.EntityExtensions.Distance2D(this.Owner, unit) <= 350);
                            if (tree != null)
                            {
                                if (ability.UseAbility(tree))
                                {
                                    await Task.Delay(GetAbilityCastDelay(ability, Owner, target), token);
                                }
                            }
                        }
                    }
                }
            }
        }

        public int GetAbilityCastDelay(Ability ability, Unit source, Unit target)
        {
            return (int)ability.GetCastDelay(source, target, true) > 0
                ? (int)ability.GetCastDelay(source, target, true)
                : (int)ability.GetCastDelay(source, target, true) + 200;
        }

        public float GetAbilitySpeed(Ability ability)
        {
            try
            {
                return ability.AbilitySpecialData.First(x => x.Name.Contains("speed")).Value > 0
                    ? ability.AbilitySpecialData.First(x => x.Name.Contains("speed")).Value
                    : float.MaxValue;
            }
            catch (Exception)
            {
                return ability.Speed > 0 ? ability.Speed : float.MaxValue;
            }
        }

        public float GetAbilityCastRange(Ability ability)
        {
            var range = 0f;
            try
            {
                if (ability.Id == AbilityId.zuus_thundergods_wrath || ability.Id == AbilityId.zuus_cloud || ability.Id == AbilityId.furion_wrath_of_nature)
                {
                    range += float.MaxValue;
                }

                if (ability.Id == AbilityId.clinkz_searing_arrows)
                {
                    range += this.Owner.GetAttackRange() + 150f;
                    return range;
                }

                if (ability.Id == AbilityId.earthshaker_enchant_totem)
                {
                    range += ability.GetAbilitySpecialData("distance_scepter");
                    return range;
                }

                if (ability.Id == AbilityId.faceless_void_time_walk)
                {
                    range += ability.GetAbilitySpecialDataWithTalent(this.Owner, "range");
                    return range;
                }

                range += ability.GetCastRange() >= ability.AbilitySpecialData.First(x => x.Name.Contains("range")).GetValue(ability.Level - 1)
                    ? ability.GetCastRange() : ability.AbilitySpecialData.First(x => x.Name.Contains("range")).GetValue(ability.Level - 1);
                var aetherLens = this.Owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    range += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }
                var talent = this.Owner.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    range += talent.GetAbilitySpecialData("value");
                }
                if (ability.Id == AbilityId.viper_viper_strike && this.Owner.HasAghanimsScepter())
                {
                    range += 400f;
                }
                return range;
            }
            catch (IndexOutOfRangeException)
            {
                range += ability.AbilitySpecialData.First(x => x.Name.Contains("range")).Value;
                var aetherLens = this.Owner.GetItemById(AbilityId.item_aether_lens);
                if (aetherLens != null)
                {
                    range += aetherLens.GetAbilitySpecialData("cast_range_bonus");
                }
                var talent = this.Owner.Spellbook.Spells.FirstOrDefault(x => x.Level > 0 && x.Name.StartsWith("special_bonus_cast_range_"));
                if (talent != null)
                {
                    range += talent.GetAbilitySpecialData("value");
                }
                return range;
            }
            catch (InvalidOperationException)
            {
                range += ability.GetCastRange();
                return range;
            }
        }

        public float GetAbilityRadius(Ability ability)
        {
            try
            {
                var radius = 0f;
                radius += ability.AbilitySpecialData.First(x => x.Name.Contains("radius")).Value;
                if (Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                        .SpecialBonusAbility) != null && Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner, ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                        .SpecialBonusAbility).Level > 0)
                {
                    radius += Ensage.SDK.Extensions.UnitExtensions.GetAbilityById(this.Owner,
                        ability.AbilitySpecialData.First(x => x.Name.Contains("radius"))
                            .SpecialBonusAbility).GetAbilitySpecialData("value");
                }

                if (ability.Name == "templar_assassin_self_trap")
                {
                    radius = 400f;
                }
                return radius;
            }
            catch (Exception)
            {
                return ability.GetRadius();
            }
        }

        public float GetEchoSlamDamage(Ability ability, Vector3 myPosition, params Unit[] targets)
        {
            if (!targets.Any())
            {
                return 0;
            }

            var otherTargets = EntityManager<Unit>.Entities.Where(
                    x => x.IsValid
                         && x.IsAlive
                         && x.IsEnemy(this.Owner)
                         && Ensage.SDK.Extensions.EntityExtensions.Distance2D(x, myPosition) < ability.GetAbilitySpecialData("echo_slam_echo_search_range"))
                .Except(targets);

            var multiplier = 0;
            foreach (var otherTarget in otherTargets)
            {
                if (otherTarget is Hero)
                {
                    multiplier++;
                }

                multiplier++;
            }

            var totalDamage = 0.0f;
            var damage = GetAbilityRawDamage(ability, DamageType.Magical);
            var amplify = this.Owner.GetSpellAmplification();

            foreach (var target in targets)
            {
                var reduction = ability.GetDamageReduction(target, DamageType.Magical);
                totalDamage += DamageHelpers.GetSpellDamage(damage * multiplier, amplify, reduction);
            }

            return totalDamage;
        }

        public float GetAbilityRawDamage(Ability ability, DamageType damageType)
        {
            var level = ability.Level;
            if (level == 0)
            {
                return 0f;
            }

            if (ability.Id == AbilityId.earthshaker_echo_slam)
            {
                return ability.GetAbilitySpecialDataWithTalent(this.Owner, "echo_slam_echo_damage");
            }

            try
            {
                return ability.GetDamage(level - 1) > 0
                    ? ability.GetDamage(level - 1)
                    : ability.AbilitySpecialData
                        .First(x => x != null && (x.Name == "damage" || x.Name == "damage_tooltip"))
                        .GetValue(ability.Level - 1);
            }
            catch (IndexOutOfRangeException)
            {
                return ability.AbilitySpecialData
                    .First(x => x != null && (x.Name == "damage" || x.Name == "damage_tooltip")).Value;
            }
            catch (InvalidOperationException)
            {
                return ability.GetDamage(level - 1);
            }
        }

        public float GetAbilityDamage(Ability ability, params Unit[] targets)
        {
            var totalDamage = 0.0f;
            var damage = GetAbilityRawDamage(ability, ability.DamageType);
            var amplify = this.Owner.GetSpellAmplification();
            foreach (var target in targets)
            {
                if (ability.Id == AbilityId.necrolyte_reapers_scythe)
                {
                    totalDamage += (target.MaximumHealth - target.Health) * ability.GetAbilitySpecialData("damage_per_health", ability.Level);
                }

                if (ability.Id == AbilityId.axe_culling_blade)
                {
                    var threshold = ability.GetAbilitySpecialData("kill_threshold");
                    if (target.Health < threshold)
                    {
                        totalDamage += float.MaxValue;
                        return totalDamage;
                    }
                }

                if (ability.Id == AbilityId.lion_finger_of_death)
                {
                    var modif = this.Owner.GetModifierByName("modifier_lion_finger_of_death_kill_counter");
                    if (modif != null && modif.StackCount > 0)
                    {
                        damage += ability.GetAbilitySpecialData("damage_per_kill") * modif.StackCount;
                    }
                }

                if (ability.Id == AbilityId.earthshaker_echo_slam)
                {
                    return GetEchoSlamDamage(ability, this.Owner.Position, targets);
                }

                if (ability.Id == AbilityId.phantom_assassin_stifling_dagger)
                {
                    var dagger = ability.GetAbilitySpecialData("base_damage");
                    var bonus = ability.GetAbilitySpecialData("attack_factor_tooltip") / 100.0f;
                    var damaj = (bonus * (this.Owner.MinimumDamage + this.Owner.BonusDamage)) + dagger;
                    var reduc = ability.GetDamageReduction(target, DamageType.Physical);
                    return DamageHelpers.GetSpellDamage(damaj, amplify, reduc);
                }

                if (ability.Id == AbilityId.lina_laguna_blade && this.Owner.HasAghanimsScepter())
                {
                    var reductionf = ability.GetDamageReduction(target, DamageType.Pure);
                    return DamageHelpers.GetSpellDamage(damage, amplify, reductionf);
                }

                var reduction = ability.GetDamageReduction(target, ability.DamageType);
                totalDamage += DamageHelpers.GetSpellDamage(damage, amplify, reduction);
            }
            return totalDamage;
        }


        public async Task UseGenericItems(Unit target, CancellationToken token = default)
        {
            if (this.Owner.UnitState == UnitState.Muted || this.Owner.UnitState == UnitState.Invisible)
            {
                return;
            }

            var isOwnerInvisBased = this.Owner.HeroId == HeroId.npc_dota_hero_riki ||
                                    this.Owner.HeroId == HeroId.npc_dota_hero_sand_king;

            if ((!isOwnerInvisBased && this.Owner.UnitState == UnitState.Invisible))
            {
                return;
            }

            if (Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
            {
                return;
            }

            List<KeyValuePair<string, uint>> itemChanger = this.ItemPriorityChanger.Value.Dictionary.Where(
                    x => this.ItemAbilityToggler.Value.IsEnabled(x.Key))
                .OrderByDescending(x => x.Value)
                .ToList();
            foreach (var order in itemChanger)
            {
                foreach (var itemId in ItemList)
                {
                    if (!Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
                    {
                        var item = this.Context.Owner.GetItemById(itemId);
                        if (item != null && item.CanBeCasted(target) && item.CanHit(target) &&
                            ((item.Name.Contains("item_dagon") && "item_dagon_5" == order.Key) || item.ToString() == order.Key) &&
                            (item.TargetTeamType.HasFlag(TargetTeamType.Enemy) || item.TargetTeamType.HasFlag(TargetTeamType.None)))
                        {
                            if (item.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget) && (item.TargetTeamType.HasFlag(TargetTeamType.Enemy) || 
                                                                                             item.TargetTeamType.HasFlag(TargetTeamType.Custom)))
                            {
                                if (item.UseAbility(target))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                }
                            }
                            else if (item.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget) &&
                                     item.TargetTeamType.HasFlag(TargetTeamType.Allied))
                            {
                                if (item.UseAbility(this.Owner))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                }
                            }
                            else if (item.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                            {
                                if (item.UseAbility(target.NetworkPosition))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                }
                            }
                            else if (item.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                            {
                                if (item.UseAbility())
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                }
                            }
                            else if (item.AbilityBehavior.HasFlag(AbilityBehavior.CanSelfCast))
                            {
                                if (item.UseAbility(this.Context.Owner))
                                {
                                    await Task.Delay(50 + (int)(Game.Ping), token);
                                }
                            }
                        }
                        else if (item != null && item.CanBeCasted(target) && item.CanHit(this.Owner) &&
                                 item.ToString() == order.Key &&
                                 item.TargetTeamType == TargetTeamType.Allied)
                        {
                            if (item.UseAbility(this.Owner))
                            {
                                await Task.Delay(50 + (int)(Game.Ping), token);
                            }
                        }
                        else if (item != null && item.CanBeCasted() &&
                                 item.ToString() == order.Key &&
                                 item.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                        {
                            if (item.UseAbility())
                            {
                                await Task.Delay(50 + (int)(Game.Ping), token);
                            }
                        }
                        //else if (item != null && item.CanBeCasted() &&
                        //         item.ToString() == order.Key &&
                        //         item.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget) &&
                        //         item.TargetTeamType == TargetTeamType.Custom)
                        //{
                        //    item.UseAbility(target);
                        //    await Task.Delay(50 + (int)(Game.Ping), token);
                        //}
                    }
                }
            }
        }

        public async Task<DisabledState> DisableEnemy(Unit target, CancellationToken token = default,
            float minimumTime = 0)
        {
            var isOwnerInvisBased = this.Owner.HeroId == HeroId.npc_dota_hero_riki ||
                                    this.Owner.HeroId == HeroId.npc_dota_hero_sand_king;

            if ((!isOwnerInvisBased && this.Owner.UnitState == UnitState.Invisible))
            {
                return DisabledState.NotDisabled;
            }

            if (Ensage.SDK.Extensions.UnitExtensions.IsChanneling(this.Owner))
            {
                return DisabledState.NotDisabled;
            }

            if (this.Owner.UnitState == UnitState.Muted || this.Owner.UnitState == UnitState.Invisible)
            {
                return DisabledState.NotDisabled;
            }

            float duration = 0;
            if (!StackDisables && (target.IsHexed(out duration) || target.IsStunned(out duration) ||
                                   Ensage.SDK.Extensions.UnitExtensions.HasModifier(target, "modifier_eul_cyclone"))
                && duration >= minimumTime)
            {
                return DisabledState.AlreadyDisabled;
            }

            List<KeyValuePair<string, uint>> disableChanger = this.DisablePriorityChanger.Value.Dictionary.Where(
                    x => this.DisableAbilityToggler.Value.IsEnabled(x.Key))
                .OrderByDescending(x => x.Value)
                .ToList();

            if (this.Owner.UnitState != UnitState.Muted)
            {
                foreach (var order in disableChanger)
                {
                    foreach (var itemName in DisableItemList.Except(new AbilityId[]
                    {
                        AbilityId.item_cyclone
                    }))
                    {
                        var item = this.Context.Owner.GetItemById(itemName);
                        if (item != null && item.CanBeCasted(target) && item.CanHit(target) &&
                            item.ToString() == order.Key)
                        {
                            if (item.UseAbility(target))
                            {
                                await Task.Delay(50 + (int)(Game.Ping), token);
                            }
                            if (!StackDisables)
                            {
                                return DisabledState.UsedAbilityToDisable;
                            }
                        }
                    }
                }
            }

            return DisabledState.NotDisabled;
        }

        protected override async Task KillStealAsync(CancellationToken token)
        {
            if (Game.IsPaused || !this.Context.Owner.IsAlive)
            {
                await Task.Delay(125, token);
                return;
            }

            foreach (var ability in this.KillstealAbilities.Keys.Where(y => this.KillstealAbilityToggler.Value.IsEnabled(y.Name)).OrderBy(x => GetAbilityRawDamage(x, x.DamageType)))
            {
                var killstealTarget = EntityManager<Hero>.Entities.FirstOrDefault(
                    x => x.IsAlive
                         && (x.Team != this.Context.Owner.Team)
                         && !x.IsIllusion
                         && ability.CanHit(x)
                         && ability.CanBeCasted()
                         && (this.Owner.Distance2D(x) <= GetAbilityCastRange(ability) || this.Owner.Distance2D(x) <= GetAbilityRadius(ability))
                         && GetAbilityDamage(ability, x) > x.Health);

                if (killstealTarget != null && (ability.Name == "earthshaker_echo_slam" || CastingChecks(ability.Name, this.Owner, killstealTarget, ability)))
                {
                    if (ability.AbilityBehavior.HasFlag(AbilityBehavior.UnitTarget))
                    {
                        PositionCamera(killstealTarget);
                        if (ability.UseAbility(killstealTarget))
                        {
                            await this.AwaitKillstealDelay((int)ability.GetCastDelay(this.Context.Owner as Hero, killstealTarget), token);
                        }
                    }
                    else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.Point))
                    {
                        PositionCamera(killstealTarget);
                        if (ability.UseAbility(killstealTarget.Position))
                        {
                            await this.AwaitKillstealDelay((int)ability.GetCastDelay(this.Context.Owner as Hero, killstealTarget), token);
                        }
                    }
                    else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.AreaOfEffect))
                    {
                        PositionCamera(killstealTarget);
                        if (ability.UseAbility(killstealTarget.Position))
                        {
                            await this.AwaitKillstealDelay((int)ability.GetCastDelay(this.Context.Owner as Hero, killstealTarget), token);
                        }
                    }
                    else if (ability.AbilityBehavior.HasFlag(AbilityBehavior.NoTarget))
                    {
                        PositionCamera(killstealTarget);
                        if (ability.UseAbility())
                        {
                            await this.AwaitKillstealDelay((int)ability.GetCastDelay(this.Context.Owner as Hero, killstealTarget), token);
                        }
                    }
                }
            }
            
            await Task.Delay(125, token);
        }

        public void PositionCamera(float x, float y)
        {
            var pos = new Vector3(x, y, 256);
            Vector2 screenposVector2;
            if (!Drawing.WorldToScreen(pos, out screenposVector2) && this.CameraMoverEnabled)
            {
                Game.ExecuteCommand($"dota_camera_set_lookatpos {x} {y}");
            }
        }
        public void PositionCamera(Unit unit)
        {
            var x = unit.Position.X;
            var y = unit.Position.Y;
            Vector2 screenposVector2;
            if (!Drawing.WorldToScreen(unit.Position, out screenposVector2) && this.CameraMoverEnabled)
            {
                Game.ExecuteCommand($"dota_camera_set_lookatpos {x} {y}");
            }
        }
    }
}