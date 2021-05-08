namespace O9K.Evader.Abilities.Items.ArmletOfMordiggian
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Abilities.Items;
    using Core.Entities.Units;
    using Core.Helpers;
    using Core.Helpers.Damage;
    using Core.Logger;
    using Core.Managers.Entity;
    using Core.Managers.Menu;
    using Core.Managers.Menu.Items;

    using Divine;
    using Divine.SDK.Extensions;

    using Metadata;

    using Pathfinder.Obstacles;

    using SharpDX;

    using Roshan = Core.Entities.Units.Unique.Roshan;

    internal class ArmletOfMordiggianUsable : CounterAbility, IDisposable
    {
        private const float ArmletFullEnableTime = 0.6f;

        private const int ArmletHpGain = 470;

        private readonly ArmletOfMordiggian armlet;

        private readonly Dictionary<Unit9, float> attacks = new Dictionary<Unit9, float>();

        private readonly Dictionary<string, KeyValuePair<int, float>> enemyDot = new Dictionary<string, KeyValuePair<int, float>>
        {
            // eye of the storm
            // epicenter

            { "modifier_rattletrap_battery_assault", new KeyValuePair<int, float>(300, 0.7f) },
            { "modifier_dark_seer_ion_shell", new KeyValuePair<int, float>(275, 0.1f) },
            { "modifier_doom_bringer_scorched_earth_effect", new KeyValuePair<int, float>(625, 1f) },
            { "modifier_ember_spirit_flame_guard", new KeyValuePair<int, float>(425, 0.2f) },
            { "modifier_gyrocopter_rocket_barrage", new KeyValuePair<int, float>(425, 0.1f) },
            { "modifier_leshrac_diabolic_edict", new KeyValuePair<int, float>(525, 0.25f) },
            { "modifier_leshrac_pulse_nova", new KeyValuePair<int, float>(475, 1f) },
            { "modifier_pudge_rot", new KeyValuePair<int, float>(275, 0.2f) },
            { "modifier_sandking_sand_storm", new KeyValuePair<int, float>(550, 0.5f) },
            { "modifier_slark_dark_pact", new KeyValuePair<int, float>(350, 1.5f) },
            { "modifier_slark_dark_pact_pulses", new KeyValuePair<int, float>(350, 0.1f) },
            { "modifier_juggernaut_blade_fury", new KeyValuePair<int, float>(275, 0.2f) },
            { "modifier_phoenix_sun_ray", new KeyValuePair<int, float>(1325, 0.2f) }
        };

        private readonly MenuSlider hpToggle;

        private readonly MenuSwitcher roshanMode;

        private readonly Dictionary<string, float> initialDotTimings = new Dictionary<string, float>();

        private readonly Dictionary<TrackingProjectile, int> projectiles = new Dictionary<TrackingProjectile, int>();

        private readonly Dictionary<string, float> selfDot = new Dictionary<string, float>
        {
            // macropyre
            // midnight pulse
            // fire fly
            // sun ray ?

            { "modifier_alchemist_acid_spray", 1f },
            { "modifier_cold_feet", 1f },
            { "modifier_ice_blast", 0.01f }, // block
            { "modifier_arc_warden_flux", 0.5f },
            { "modifier_axe_battle_hunger", 1f },
            { "modifier_flamebreak_damage", 1f },
            { "modifier_crystal_maiden_frostbite", 0.5f },
            { "modifier_crystal_maiden_freezing_field_slow", 0.1f },
            { "modifier_dazzle_poison_touch", 1f },
            { "modifier_death_prophet_spirit_siphon_slow", 0.25f },
            { "modifier_disruptor_static_storm", 0.25f },
            { "modifier_disruptor_thunder_strike", 2f },
            { "modifier_doom_bringer_infernal_blade_burn", 1f },
            { "modifier_dragon_knight_corrosive_breath_dot", 1f },
            { "modifier_earth_spirit_magnetize", 0.5f },
            { "modifier_ember_spirit_searing_chains", 0.5f },
            { "modifier_enigma_malefice", 2f },
            { "modifier_brewmaster_fire_permanent_immolation", 1f },
            { "modifier_huskar_burning_spear_debuff", 1f },
            { "modifier_invoker_ice_wall_slow_debuff", 1f },
            { "modifier_invoker_chaos_meteor_burn", 0.5f },
            { "modifier_jakiro_dual_breath_burn", 0.5f },
            { "modifier_jakiro_liquid_fire_burn", 0.5f },
            { "modifier_meepo_geostrike_debuff", 1f },
            { "modifier_necrolyte_heartstopper_aura_effect", 0.2f },
            { "modifier_ogre_magi_ignite", 1f },
            { "modifier_phoenix_fire_spirit_burn", 1f },
            { "modifier_phoenix_icarus_dive_burn", 1f },
            { "modifier_phoenix_sun_debuff", 1f },
            { "modifier_pugna_life_drain", 0.25f },
            { "modifier_queenofpain_shadow_strike", 3f },
            { "modifier_silencer_curse_of_the_silent", 1f },
            { "modifier_silencer_last_word", 4f },
            { "modifier_skywrath_mystic_flare_aura_effect", 0.1f },
            { "modifier_sniper_shrapnel_slow", 1f },
            { "modifier_broodmother_poison_sting_debuff", 1f },
            { "modifier_lone_druid_spirit_bear_entangle_effect", 1f },
            { "modifier_tornado_tempest_debuff", 0.25f },
            { "modifier_shredder_chakram_debuff", 0.5f },
            { "modifier_treant_leech_seed", 0.75f },
            { "modifier_treant_overgrowth", 1f },
            { "modifier_abyssal_underlord_firestorm_burn", 1f },
            { "modifier_venomancer_venomous_gale", 3f },
            { "modifier_venomancer_poison_sting", 1f },
            { "modifier_venomancer_poison_sting_ward", 1f },
            { "modifier_venomancer_poison_nova", 1f },
            { "modifier_viper_corrosive_skin_slow", 1f },
            { "modifier_viper_poison_attack_slow", 1f },
            { "modifier_viper_viper_strike_slow", 1f },
            { "modifier_gnoll_assassin_envenomed_weapon_poison", 1f },
            { "modifier_warlock_shadow_word", 1f },
            { "modifier_warlock_golem_permanent_immolation_debuff", 1f },
            { "modifier_weaver_swarm_debuff", 0.8f }, // todo fix interval
            { "modifier_winter_wyvern_arctic_burn_slow", 1f },
            { "modifier_maledict", 1f },
            { "modifier_dark_willow_bramble_maze", 0.5f },
            { "modifier_skeleton_king_hellfire_blast", 1f },
            { "modifier_item_orb_of_venom_slow", 1f },
            { "modifier_item_radiance_debuff", 1f },
            { "modifier_item_urn_damage", 1f },
            { "modifier_item_spirit_vessel_damage", 1f },
            { "modifier_spawnlord_master_freeze_root", 0.5f }
        };

        private readonly Sleeper toggling = new Sleeper();

        private bool canToggle;

        private bool manualDisable;

        public ArmletOfMordiggianUsable(Ability9 ability, IMainMenu menu)
            : base(ability, menu)
        {
            this.armlet = (ArmletOfMordiggian)ability;
            this.ModifierName = null;

            var settings = menu.AbilitySettings.AddAbilitySettingsMenu(ability);

            this.hpToggle = settings.GetOrAdd(new MenuSlider("Toggle hp threshold", "hpToggle" + ability.Name, 222, 75, 400));
            this.hpToggle.AddTranslation(Lang.Ru, "Порог здоровья для переключения");
            this.hpToggle.AddTranslation(Lang.Cn, "要切换的运行状况阈值");

            this.roshanMode = settings.GetOrAdd(new MenuSwitcher("Roshan mode", "RoshanMode" + ability.Name, false));
            this.roshanMode.AddTranslation(Lang.Ru, "Режим рошана");
            this.roshanMode.AddTranslation(Lang.Cn, "肉山模式");

            UpdateManager.CreateIngameUpdate(this.OnUpdate);
            OrderManager.OrderAdding += this.OnOrderAdding;
            ModifierManager.ModifierAdded += this.OnModifierAdded;
            ProjectileManager.TrackingProjectileAdded += this.OnAddTrackingProjectile;
            ProjectileManager.TrackingProjectileRemoved += this.OnRemoveTrackingProjectile;
            EntityManager9.UnitMonitor.UnitDied += this.OnUnitDied;
            EntityManager9.UnitMonitor.AttackStart += this.OnAttackStart;

            //   Drawing.OnDraw += OnDraw;   // Debug
        }

        private static float TogglePing
        {
            get
            {
                return (GameManager.Ping / 2) + 50;
            }
        }

        public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            if (obstacle.IsModifierObstacle && ally.Equals(this.Owner) && obstacle.EvadableAbility.Ability.Id == AbilityId.doom_bringer_doom
                && this.armlet.Enabled)
            {
                return this.Ability.IsUsable;
            }

            if (this.toggling.IsSleeping)
            {
                return false;
            }

            if (!base.CanBeCasted(ally, enemy, obstacle))
            {
                return false;
            }

            if (this.armlet.Enabled && this.IsAffectedByDot())
            {
                return false;
            }

            var health = ally.Health;

            var damage = obstacle.GetDamage(ally);
            if (damage > 0 && health > damage + 100)
            {
                return false;
            }

            if (this.armlet.Enabled)
            {
                health = Math.Max(health - ArmletHpGain, 1);
            }

            if (HpRestored(obstacle.GetEvadeTime(ally, false) - (TogglePing / 1000)) + health < damage)
            {
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            UpdateManager.DestroyIngameUpdate(this.OnUpdate);
            OrderManager.OrderAdding -= this.OnOrderAdding;
            ModifierManager.ModifierAdded -= this.OnModifierAdded;
            ProjectileManager.TrackingProjectileAdded -= this.OnAddTrackingProjectile;
            ProjectileManager.TrackingProjectileRemoved -= this.OnRemoveTrackingProjectile;
            EntityManager9.UnitMonitor.UnitDied -= this.OnUnitDied;
            EntityManager9.UnitMonitor.AttackStart -= this.OnAttackStart;
        }

        public override float GetRequiredTime(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            return this.ActiveAbility.GetHitTime(ally) + Math.Max(
                       Math.Min(obstacle.GetEvadeTime(ally, false) - 0.1f, ArmletFullEnableTime),
                       0);
        }

        public override bool Use(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            if (obstacle?.IsModifierObstacle == true && obstacle.EvadableAbility.Ability.Id == AbilityId.doom_bringer_doom)
            {
                var slot = this.Owner.BaseInventory.FreeBackpackSlots.FirstOrDefault();
                if (slot == ItemSlot.MainSlot_1)
                {
                    slot = ItemSlot.BackPack_1;
                }

                this.Ability.BaseItem.Move(slot);
                return true;
            }

            this.toggling.Sleep(ArmletFullEnableTime);

            if (this.armlet.Enabled)
            {
                if (this.ActiveAbility.UseAbility(false, true))
                {
                    UpdateManager.BeginInvoke(50, () => this.ActiveAbility.UseAbility(false, true));
                    return true;
                }

                return false;
            }

            return this.ActiveAbility.UseAbility(false, true);
        }

        private static float HpRestored(float time)
        {
            if (time <= 0)
            {
                return 0;
            }

            return (Math.Min(time, ArmletFullEnableTime) * ArmletHpGain) / ArmletFullEnableTime;
        }

        private bool IsAffectedByDot()
        {
            var ping = GameManager.Ping / 1000;

            try
            {
                foreach (var modifier in this.Owner.BaseModifiers.Where(x => !x.IsHidden && x.IsDebuff))
                {
                    if (!this.selfDot.TryGetValue(modifier.Name, out var tick))
                    {
                        continue;
                    }

                    if (!this.initialDotTimings.TryGetValue(modifier.Name, out var initialTime))
                    {
                        continue;
                    }

                    var elapsedTime = (GameManager.RawGameTime - initialTime) % tick;

                    if (elapsedTime + 0.2 > tick || elapsedTime < 0.1)
                    {
                        return true;
                    }
                }

                foreach (var unit in EntityManager9.Units.Where(
                    x => x.IsEnemy(this.Owner) && x.IsAlive && x.Distance(this.Owner.Position) < 1325))
                {
                    foreach (var modifier in unit.BaseModifiers.Where(x => !x.IsHidden))
                    {
                        if (!this.enemyDot.TryGetValue(modifier.Name, out var modifierData))
                        {
                            continue;
                        }

                        var distance = modifierData.Key;
                        if (this.Owner.Distance(unit.Position) > distance)
                        {
                            continue;
                        }

                        var tick = modifierData.Value;
                        var elapsedTime = modifier.ElapsedTime % tick;
                        if (elapsedTime + 0.2 + ping > tick || elapsedTime + ping < 0.05)
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            return false;
        }

        private void OnAddTrackingProjectile(TrackingProjectileAddedEventArgs args)
        {
            if (!this.Menu.AbilitySettings.IsCounterEnabled(this.Ability.Name))
            {
                return;
            }

            try
            {
                var projectile = args.TrackingProjectile;
                if (projectile.Target?.Handle != this.Owner.Handle)
                {
                    return;
                }

                var source = projectile.Source as Unit;
                if (source == null)
                {
                    return;
                }

                var unit = EntityManager9.GetUnit(source.Handle);
                if (unit == null)
                {
                    return;
                }

                this.projectiles[projectile] = unit.GetAttackDamage(this.Owner, DamageValue.Maximum);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnAttackStart(Unit9 unit)
        {
            try
            {
                if (unit.Team == this.Owner.Team || unit.Distance(this.Owner) > 2000)
                {
                    return;
                }

                this.attacks[unit] = GameManager.RawGameTime - (GameManager.Ping / 2000);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (this.canToggle)
            {
                RendererManager.DrawText(
                    "Can toggle",
                    RendererManager.WorldToScreen(this.Owner.Position),
                    Color.White,
                    "Arial",
                    25);
            }
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                var order = e.Order;
                if (order.Type != OrderType.CastToggle || order.Ability.Handle != this.Ability.Handle)
                {
                    return;
                }

                if (this.toggling.IsSleeping || !this.canToggle)
                {
                    e.Process = false;
                    return;
                }

                if (e.IsCustom)
                {
                    return;
                }

                if (this.armlet.Enabled)
                {
                    this.manualDisable = true;
                    this.toggling.Sleep(0.1f);
                }
                else
                {
                    this.manualDisable = false;
                    this.toggling.Sleep(ArmletFullEnableTime);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs args)
        {
            if (!this.Menu.AbilitySettings.IsCounterEnabled(this.Ability.Name))
            {
                return;
            }

            try
            {
                if (args.Modifier.Owner.Handle != this.Owner.Handle)
                {
                    return;
                }

                var name = args.Modifier.Name;
                if (name == "modifier_bashed" && args.Modifier.Caster is Divine.Roshan)
                {
                    isLastStun = true;
                    isLastRoshanAttack = true;
                }

                if (name == "modifier_fountain_aura_buff" && this.armlet.Enabled && this.canToggle && !this.toggling.IsSleeping)
                {
                    this.manualDisable = true;
                    this.ActiveAbility.UseAbility(false, true);
                }
                else if (this.selfDot.ContainsKey(name))
                {
                    this.initialDotTimings[name] = GameManager.RawGameTime - args.Modifier.ElapsedTime;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnRemoveTrackingProjectile(TrackingProjectileRemovedEventArgs args)
        {
            try
            {
                this.projectiles.Remove(args.TrackingProjectile);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private void OnUnitDied(Unit9 unit)
        {
            try
            {
                this.attacks.Remove(unit);
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private bool isLastRoshanAttack;

        private bool isLastStun;

        private void OnUpdate()
        {
            if (!this.Menu.AbilitySettings.IsCounterEnabled(this.Ability.Name) || GameManager.IsPaused)
            {
                return;
            }

            try
            {
                if (!this.Ability.CanBeCasted(false) || (this.armlet.Enabled && this.IsAffectedByDot()))
                {
                    this.canToggle = false;
                    return;
                }

                var position = this.Owner.IsMoving && Math.Abs(this.Owner.BaseUnit.RotationDifference) < 60
                                   ? this.Owner.InFront(55)
                                   : this.Owner.Position;

                var noProjectiles = true;
                foreach (var projectile in this.projectiles)
                {
                    if (!projectile.Key.IsValid)
                    {
                        continue;
                    }

                    var time = Math.Max(projectile.Key.Position.Distance2D(position) - this.Owner.HullRadius, 0) / projectile.Key.Speed;
                    var hpRestored = HpRestored(time - (TogglePing / 1000) - 0.12f);

                    if (projectile.Value >= hpRestored)
                    {
                        noProjectiles = false;
                        break;
                    }
                }

                var noAutoAttacks = true;

                foreach (var attack in this.attacks.Where(
                    x => x.Key.IsValid && x.Key.IsAlive && x.Key.Distance(this.Owner) <= x.Key.GetAttackRange(this.Owner, 200)
                         && x.Key.GetAngle(this.Owner.Position) < 0.5
                         && (!x.Key.IsRanged || x.Key.Distance(this.Owner) < 400 /*|| x.Key.AttackPoint() < 0.15*/)))
                {
                    var unit = attack.Key;
                    var attackStart = attack.Value;
                    var attackPoint = unit.GetAttackPoint(this.Owner);
                    var secondsPerAttack = unit.BaseUnit.SecondsPerAttack;
                    var time = GameManager.RawGameTime;

                    var damageTime = attackStart + attackPoint;
                    if (unit.IsRanged)
                    {
                        damageTime += Math.Max(unit.Distance(this.Owner) - this.Owner.HullRadius, 0) / unit.ProjectileSpeed;
                    }

                    var echoSabre = unit.Abilities.FirstOrDefault(x => x.Id == AbilityId.item_echo_sabre);

                    // fuck calcus
                    if ((time <= damageTime // no switch before damage
                         && (attackPoint < 0.35 // no switch if low attackpoint before attack start
                             || time + (attackPoint * 0.6) > damageTime)) // or allow switch if big attack point
                        || (attackPoint < 0.25 // dont allow switch if very low attack point 
                            && time > damageTime + (unit.GetAttackBackswing(this.Owner) * 0.6) // after attack end
                            && time <= attackStart + secondsPerAttack + 0.12) // allow if attack time passed secperatk time
                        || (echoSabre != null && !unit.IsRanged // echo sabre check
                                              && echoSabre.Cooldown - echoSabre.RemainingCooldown <= attackPoint * 2))
                    {
                        noAutoAttacks = false;
                        isLastRoshanAttack = unit is Roshan;
                        break;
                    }
                }

                this.canToggle = ((noProjectiles && noAutoAttacks) || !this.armlet.Enabled) && !this.toggling.IsSleeping;

                var nearEnemies = EntityManager9.Units.Any(
                    x => (x.IsUnit || x.IsTower) && x.IsAlive && !x.IsAlly(this.Owner) && x.Distance(this.Owner.Position) < 1000);

                if ((this.Owner.Health < (roshanMode && isLastRoshanAttack ? 400 : this.hpToggle)) && this.canToggle && (nearEnemies || !this.manualDisable))
                {
                    if (!roshanMode || !isLastStun)
                    {
                        this.Use(null, null, null);
                    }

                    isLastStun = false;
                    isLastRoshanAttack = false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }
    }
}