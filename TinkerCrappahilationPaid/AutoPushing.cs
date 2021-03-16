using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Divine;
using Divine.SDK.Extensions;

using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;
using Ensage.Common.Objects.UtilityObjects;
using Ensage.SDK.Abilities;
using Ensage.SDK.Extensions;
using Ensage.SDK.Helpers;
using Ensage.SDK.Menu;
using SharpDX;
using UnitExtensions = Divine.SDK.Extensions.UnitExtensions;

namespace TinkerCrappahilationPaid
{
    public class AutoPushing
    {
        private AbilitiesInCombo Abilities => _main.AbilitiesInCombo;
        private Config Config => _main.Config;
        private Hero Me => _main.Me;
        private readonly TinkerCrappahilationPaid _main;
        public HashSet<Vector3> SaveSpotTable;
        //public List<Vector3> SaveSpotTable2;
        private TpPosition tpPos;
        private List<Unit> Shrines;
        private IEnumerable<JunglePosition> jungleFarmPositions;
        private State _currentState;

        public class TpPosition
        {
            public Vector3 CreepPosition;
            public Vector3 BlinkPosition;

            public TpPosition(Vector3 creepPosition, Vector3 blinkPosition)
            {
                CreepPosition = creepPosition;
                BlinkPosition = blinkPosition;
            }
            public TpPosition(Unit creep, Vector3 blinkPosition)
            {
                CreepPosition = creep.Position;
                BlinkPosition = blinkPosition;
            }
        }

        public class LanePosition
        {
            public readonly Vector3 Pos;

            public LanePosition(Vector3 pos)
            {
                Pos = pos;
            }
        }
        public class JunglePosition
        {
            public readonly Vector3 Start;
            public readonly Vector3 CastPos;
            public readonly Team Team;

            public JunglePosition(Vector3 start, Vector3 castPos, Team team = Team.Radiant)
            {
                Start = start;
                CastPos = castPos;
                Team = team;
            }
        }

        public enum State
        {
            RegenOnBase,
            TpAction,
            Farm,
            Jungle,
            GoingHome
        }

        public AutoPushing(TinkerCrappahilationPaid main)
        {
            _main = main;

            Factory = main.Config.Factory.Menu("Auto Pusing");
            Enable = Factory.Item("Push Key", new KeyBind('0', KeyBindType.Toggle));
            PushLaneWithAlly = Factory.Item("Push lane with ally hero", true);
            PushLaneWithEnemy = Factory.Item("Push lane with enemy hero", true);
            AutoEscape = Factory.Item("Escape if under enemy's vision", true);

            PushLanes = Factory.Item("Push lanes", true);
            PushJungle = Factory.Item("Push jungle", true);

            SaveSpotTable = new HashSet<Vector3>
            {
                new Vector3(-7332, -3269, 384),
                new Vector3(-7233, -1376, 384),
                new Vector3(-7200, -1017, 384),
                new Vector3(-7212, -551, 384),
                new Vector3(-7125, -81, 384),
                new Vector3(-7114, 337, 384),
                new Vector3(-7194, 732, 384),
                new Vector3(-7129, 1337, 384),
                new Vector3(-7140, 1645, 384),
                new Vector3(-7176, 2070, 384),
                new Vector3(-7089, 2307, 512),
                new Vector3(-6847, 3532, 384),
                new Vector3(-7226, 3989, 384),
                new Vector3(-6994, 4915, 384),
                new Vector3(-6900, 5118, 384),
                new Vector3(-6732, 5540, 384),
                new Vector3(-6581, 5919, 384),
                new Vector3(-6273, 6178, 384),
                new Vector3(-6104, 6542, 384),
                new Vector3(-5458, 6709, 384),
                new Vector3(-5130, 6783, 384),
                new Vector3(-4631, 6760, 384),
                new Vector3(-4308, 6977, 384),
                new Vector3(-3791, 6757, 384),
                new Vector3(-3497, 6873, 384),
                new Vector3(-3117, 6930, 384),
                new Vector3(-2696, 6878, 384),
                new Vector3(-2321, 6938, 384),
                new Vector3(-1731, 6864, 384),
                new Vector3(-1100, 6951, 384),
                new Vector3(-767, 7021, 384),
                new Vector3(-82, 6823, 384),
                new Vector3(183, 6728, 384),
                new Vector3(673, 6884, 384),
                new Vector3(1009, 6861, 384),
                new Vector3(1561, 6964, 384),
                new Vector3(2540, 6960, 384),
                new Vector3(3445, 6863, 384),
                new Vector3(7400, 2808, 384),
                new Vector3(7456, 2090, 256),
                new Vector3(7226, 866, 384),
                new Vector3(7029, 494, 384),
                new Vector3(7086, -37, 384),
                new Vector3(6850, -193, 384),
                new Vector3(7205, -493, 383),
                new Vector3(4987, -5374, 384),
                new Vector3(6918, -908, 384),
                new Vector3(7080, -1472, 384),
                new Vector3(7171, -1807, 384),
                new Vector3(7297, -2177, 384),
                new Vector3(7031, -3224, 384),
                new Vector3(6898, -3549, 384),
                new Vector3(7460, -4648, 384),
                new Vector3(6924, -4814, 384),
                new Vector3(6891, -5163, 384),
                new Vector3(6701, -5480, 384),
                new Vector3(6647, -5824, 384),
                new Vector3(6583, -6132, 384),
                new Vector3(6381, -6424, 384),
                new Vector3(6059, -6451, 384),
                new Vector3(6021, -6588, 384),
                new Vector3(5650, -6737, 384),
                new Vector3(5378, -6735, 384),
                new Vector3(4971, -6738, 384),
                new Vector3(4536, -6652, 384),
                new Vector3(4333, -6725, 384),
                new Vector3(3879, -6734, 384),
                new Vector3(3364, -6777, 384),
                new Vector3(3013, -6804, 384),
                new Vector3(2696, -6795, 384),
                new Vector3(2388, -6791, 384),
                new Vector3(1970, -6840, 384),
                new Vector3(1594, -6898, 384),
                new Vector3(1150, -6852, 384),
                new Vector3(759, -6957, 384),
                new Vector3(289, -6964, 384),
                new Vector3(-330, -6876, 384),
                new Vector3(-623, -6858, 384),
                new Vector3(-1073, -6927, 384),
                new Vector3(-2947, -6995, 256),
                new Vector3(-3990, -7001, 384),
                new Vector3(-530, -5611, 384),
                new Vector3(2463, -5622, 384),
                new Vector3(3951, -5522, 384),
                new Vector3(5655, -3890, 384),
                new Vector3(5565, -1369, 384),
                new Vector3(5690, 995, 384),
                new Vector3(2228, 2684, 256),
                new Vector3(2939, 1222, 256),
                new Vector3(1008, 1594, 256),
                new Vector3(489, 1282, 256),
                new Vector3(1283, 19, 256),
                new Vector3(-907, -1464, 256),
                new Vector3(-2041, -936, 256),
                new Vector3(-2490, -1083, 256),
                new Vector3(-1236, -1858, 256),
                new Vector3(-2032, -2420, 256),
                new Vector3(-2303, -2759, 256),
                new Vector3(-2832, -1435, 256),
                new Vector3(-3888, -2336, 256),
                new Vector3(-2676, 5523, 384),
                new Vector3(-1180, 5551, 384),
                new Vector3(-5025, 5136, 384),
                new Vector3(-5269, 5023, 384),
                new Vector3(-4234, 5335, 384),
                new Vector3(-3737, 5550, 384),
                new Vector3(4623, -5468, 384),
                new Vector3(2962, -5598, 383),
            };

            jungleFarmPositions = new List<JunglePosition>
            {
                new JunglePosition(new Vector3(-4620, 156, 256), new Vector3(-4568, 252, 256)),
                new JunglePosition(new Vector3(-903, -4109, 384), new Vector3(-1033, -3828, 256)),
                new JunglePosition(new Vector3(3670, -4655, 256), new Vector3(3757, -4497, 256)),

                new JunglePosition(new Vector3(3520, 155, 384), new Vector3(3696, 321, 384), Team.Dire),
                new JunglePosition(new Vector3(3377, 56, 384), new Vector3(3592, 248, 384), Team.Dire),
                new JunglePosition(new Vector3(-2406, 3738, 256), new Vector3(-2409, 3863, 256), Team.Dire),
                new JunglePosition(new Vector3(474, 3788, 384), new Vector3(583, 3650, 384), Team.Dire),
            }.Where(x => x.Team == main.Me.Team);

            if (_main.Config.DrawBlinkPoints)
            {
                Drawing.OnDraw += BlinkPoints;
            }

            if (_main.Config.DrawJunglePoints)
            {
                Drawing.OnDraw += JunglePoints;
            }


            _main.Config.DrawBlinkPoints.PropertyChanged += (sender, args) =>
            {
                if (_main.Config.DrawBlinkPoints)
                {
                    Drawing.OnDraw += BlinkPoints;
                }
                else
                {
                    Drawing.OnDraw -= BlinkPoints;
                }
            };

            _main.Config.DrawJunglePoints.PropertyChanged += (sender, args) =>
            {
                if (_main.Config.DrawJunglePoints)
                {
                    Drawing.OnDraw += JunglePoints;
                }
                else
                {
                    Drawing.OnDraw -= JunglePoints;
                }
            };

            

            /*Drawing.OnDraw += args =>
            {
                foreach (var unit in EntityManager<Unit>.Entities.Where(x=> !(x is Hero)))
                {
                    if (Drawing.WorldToScreen(unit.Position, out var p) && !p.IsZero)
                        Drawing.DrawCircle(p, 20, 20, Color.YellowGreen);
                }

                Drawing.DrawText($"CurrentStage {CurrentState}", new Vector2(200, 200), new Vector2(15), Color.White,
                    FontFlags.None);

            };*/

            Fountain = EntityManager.GetEntities<Unit>()
                .FirstOrDefault(x => x.NetworkName == "CDOTA_Unit_Fountain" && x.Team == Me.Team);

            CurrentState = State.RegenOnBase;
            Shrines = new List<Unit>();
            foreach (var source in EntityManager.GetEntities<Unit>().Where(
                x =>
                    x.IsValid && x.IsAlive && x.Team == Me.Team &&
                    (x.NetworkName == "CDOTA_BaseNPC_Healer" || x.Name == "npc_dota_goodguys_healers" || x.Name == "npc_dota_badguys_healers")))
            {
                Shrines.Add(source);
                TinkerCrappahilationPaid.Log.Warn("new shrine");
            }

            var jungleSleeper = new MultiSleeper();

            Enable.PropertyChanged += (sender, args) =>
            {
                if (Enable)
                {
                    TinkerCrappahilationPaid.Log.Error($"lets push. Active");
                    CurrentState = State.RegenOnBase;
                    TinkerCrappahilationPaid.Log.Warn($"Fountain != null {Fountain!=null} Fountain: [{Fountain?.NetworkName}] MyTeam: {Me.Team}");

                    if (Fountain == null)
                    {
                        Fountain = ObjectManager.GetEntities<Unit>()
                            .FirstOrDefault(x =>
                                x.Name == "dota_fountain" && x.Team == Me.Team);
                        if (Fountain == null)
                        {
                            TinkerCrappahilationPaid.Log.Warn($"Failed with fountain x2");
                            return;
                        }
                    }
                    var maxMarch = 0;

                    GameManager.ExecuteCommand("dota_player_units_auto_attack_mode 0");
                    UpdateManager.BeginInvoke(async () =>
                    {
                        var stage = -1;
                        while (Enable)
                        {
                            ActiveAbility travel;
                            switch (CurrentState)
                            {
                                case State.RegenOnBase:
                                    try
                                    {
                                        stage = 3;
                                        if (!Me.IsInRange(Fountain, 1000))
                                        {
                                            TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Moving to fountain");
                                            if (Me.IsInRange(Fountain, 3000))
                                            {
                                                stage = 1;
                                                Me.Move(Fountain.Position);
                                            }
                                            else
                                            {
                                                stage = 2;
                                                CurrentState = State.GoingHome;
                                            }
                                        }
                                        else
                                        {
                                            stage = 8;
                                            if (Me.Spellbook.Spells.Any(x => x.AbilityState == AbilityState.OnCooldown) ||
                                                Me.Inventory.Items.Any(x =>
                                                    (x.Id != AbilityId.item_bottle) &&
                                                    (x.AbilityState == AbilityState.ItemOnCooldown ||
                                                     x.AbilityState == AbilityState.OnCooldown)))
                                            {
                                                if (!Abilities.Rearm.Ability.IsInAbilityPhase &&
                                                    !UnitExtensions.IsChanneling(Me))
                                                {
                                                    stage = 4;
                                                    Abilities.Rearm.UseAbility();
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Rearm");
                                                    await Task.Delay(Abilities.Rearm.GetCastDelay());
                                                }
                                            }
                                            else
                                            {
                                                stage = 9;
                                                if (Math.Abs(Me.Mana - Me.MaximumMana) <= 300 && Me.HealthPercent() >= 0.85)
                                                {
                                                    stage = 5;
                                                    CurrentState = State.TpAction;
                                                    TinkerCrappahilationPaid.Log.Warn(
                                                        $"[{CurrentState}] All items/abilities without cooldown. Next stage");
                                                }
                                                else
                                                {
                                                    stage = 6;
                                                    if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted &&
                                                        !UnitExtensions.HasModifier(Me,
                                                            Abilities.Bottle.TargetModifierName))
                                                    {
                                                        stage = 7;
                                                        Abilities.Bottle.UseAbility();
                                                    }
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Under regen");
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        TinkerCrappahilationPaid.Log.Error($"[AutoPush.RegeinOnBase][code: {stage}] {e}");
                                    }
                                    
                                    break;
                                case State.TpAction:
                                    try
                                    {
                                        travel = (ActiveAbility)Abilities.Travel ?? Abilities.Travel2;
                                        if (travel != null && travel.CanBeCasted)
                                        {
                                            tpPos = PushLanes ? GetTheBestLane() : null;
                                            if (tpPos != null)
                                            {
                                                maxMarch = 2;//Abilities.Veil != null ? 1 : 2;
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Found creeps for lane pushing.");
                                                if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted)
                                                {
                                                    Abilities.Glimmer.UseAbility(Me);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Glimmer");
                                                    await Task.Delay(Abilities.Glimmer.GetCastDelay(Me));
                                                }
                                                travel.UseAbility(tpPos.CreepPosition);
                                                CurrentState = State.Farm;
                                                if (Abilities.Blink != null && !tpPos.BlinkPosition.IsZero)
                                                {
                                                    Abilities.Blink.Ability.UseAbility(tpPos.BlinkPosition, true);
                                                    Me.Move(tpPos.BlinkPosition, true);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Tp with blink");
                                                }
                                                else
                                                {
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Tp without blink");
                                                }
                                                await Task.Delay((int)(3250 + GameManager.Ping));
                                            }
                                            else if (PushJungle)
                                            {
                                                maxMarch = 3;//Abilities.Veil != null ? 1 : 2;

                                                var freeJunglePosition = jungleFarmPositions
                                                    .ToList().FirstOrDefault(z => !jungleSleeper.Sleeping(z));
                                                if (freeJunglePosition != null)
                                                {
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Cant find creeps for lane pushing. Will try to farm jungle");
                                                    var closestShrine = Shrines.Where(x =>
                                                            x != null && x.IsValid && x.IsAlive && !x.IsVisibleToEnemies &&
                                                            x.IsAlly(Me) && x.IsInRange(freeJunglePosition.Start, 3000))
                                                        .OrderBy(z =>
                                                            Ensage.SDK.Extensions.EntityExtensions.Distance2D(z,
                                                                freeJunglePosition.Start)).FirstOrDefault();
                                                    var creepsForTp = EntityManager<Creep>.Entities.Where(x =>
                                                        x.IsAlive && x.IsVisible && x.IsAlly(Me) &&
                                                        x.IsInRange(freeJunglePosition.Start, 3000)).OrderBy(z =>
                                                        freeJunglePosition.Start.Distance2D(z));
                                                    if (closestShrine != null)
                                                    {
                                                        Vector3 positionForTp = closestShrine.Position;
                                                        foreach (var creep in creepsForTp)
                                                        {
                                                            if (freeJunglePosition.Start.Distance2D(creep) <
                                                                freeJunglePosition.Start.Distance2D(closestShrine))
                                                            {
                                                                positionForTp = creep.NetworkPosition;
                                                                break;
                                                            }
                                                        }
                                                        tpPos = new TpPosition(freeJunglePosition.Start,
                                                            freeJunglePosition.CastPos);
                                                        travel.UseAbility(positionForTp);
                                                        var secs = (int)(GameManager.GameTime % 60);
                                                        var nextDelay = (60 - secs) * 1000;
                                                        jungleSleeper.Sleep(nextDelay, freeJunglePosition);
                                                        CurrentState = State.Jungle;
                                                        if (Abilities.Blink != null)
                                                        {
                                                            Abilities.Blink.Ability.UseAbility(tpPos.CreepPosition, true);
                                                            Me.Move(tpPos.CreepPosition, true);
                                                            TinkerCrappahilationPaid.Log.Warn(
                                                                $"[{CurrentState}][Jungle] Tp with blink");
                                                        }
                                                        await Task.Delay((int)(3250 + GameManager.Ping));
                                                    }
                                                    else
                                                    {
                                                        TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Cant find shrine");
                                                    }
                                                }
                                                else
                                                {
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Cant find creeps for lane pushing. All jungle camps farmed. Will w8");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            CurrentState = State.GoingHome;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        TinkerCrappahilationPaid.Log.Error($"[AutoPush.TpAction] {e}");
                                    }
                                    
                                    break;
                                case State.Jungle:
                                    try
                                    {
                                        if (Me.IsInRange(tpPos.CreepPosition, 100))
                                        {
                                            if (maxMarch > 0)
                                            {
                                                if (Abilities.March.CanBeCasted())
                                                {
                                                    if (!Abilities.March.IsInAbilityPhase)
                                                    {
                                                        Abilities.March.UseAbility(tpPos.BlinkPosition);
                                                        Me.Stop(true);
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] March ({maxMarch}) CastRange: {Abilities.March.GetCastRange()}");
                                                        maxMarch--;
                                                        await Task.Delay(800);
                                                    }
                                                    if (AutoEscape && Me.IsVisibleToEnemies)
                                                    {
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] AutoEscape cuz under vision");
                                                        CurrentState = State.GoingHome;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted)
                                                    {
                                                        Abilities.SoulRing.UseAbility();
                                                        TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] SoulRing");
                                                        await Task.Delay(Abilities.SoulRing.GetCastDelay());
                                                    }
                                                    if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted)
                                                    {
                                                        Abilities.Glimmer.UseAbility(Me);
                                                        TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Glimmer");
                                                        await Task.Delay(Abilities.Glimmer.GetCastDelay(Me));
                                                    }
                                                    if (Abilities.Veil != null && Abilities.Veil.CanBeCasted)
                                                    {
                                                        var enemyCreeps = EntityManager<Unit>.Entities
                                                            .Where(x => x.IsAlive && x.IsVisible && x.IsEnemy(Me) &&
                                                                        Abilities.Veil.CanHit(x) && !UnitExtensions.HasModifier(x, Abilities.Veil.TargetModifierName) && x.IsSpawned)
                                                            .OrderBy(z => z.Distance2D(Me)).FirstOrDefault();
                                                        if (enemyCreeps != null)
                                                        {
                                                            Abilities.Veil.UseAbility(enemyCreeps);
                                                            TinkerCrappahilationPaid.Log.Warn(
                                                                $"[{CurrentState}] Veil To Creeps");
                                                            await Task.Delay(Abilities.Veil.GetCastDelay(enemyCreeps));
                                                        }
                                                        else
                                                        {
                                                            TinkerCrappahilationPaid.Log.Warn(
                                                                $"[{CurrentState}] cant find creeps for veil");
                                                        }
                                                    }
                                                    if (Abilities.Rearm.CanBeCasted)
                                                    {
                                                        Abilities.Rearm.UseAbility();
                                                        TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Rearm");
                                                        await Task.Delay(Abilities.Rearm.GetCastDelay());
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted)
                                                {
                                                    Abilities.SoulRing.UseAbility();
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] SoulRing");
                                                    await Task.Delay(Abilities.SoulRing.GetCastDelay());
                                                }
                                                if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted)
                                                {
                                                    Abilities.Glimmer.UseAbility(Me);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Glimmer");
                                                    await Task.Delay(Abilities.Glimmer.GetCastDelay(Me));
                                                }
                                                if (Abilities.Veil != null && Abilities.Veil.CanBeCasted)
                                                {
                                                    var enemyCreeps = EntityManager<Unit>.Entities
                                                        .Where(x => x.IsAlive && x.IsVisible && x.IsEnemy(Me) &&
                                                                    Abilities.Veil.CanHit(x) && !UnitExtensions.HasModifier(x, Abilities.Veil.TargetModifierName) && x.IsSpawned)
                                                        .OrderBy(z => z.Distance2D(Me)).FirstOrDefault();
                                                    if (enemyCreeps != null)
                                                    {
                                                        Abilities.Veil.UseAbility(enemyCreeps);
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] Veil To Creeps");
                                                        await Task.Delay(Abilities.Veil.GetCastDelay(enemyCreeps));
                                                    }
                                                    else
                                                    {
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] cant find creeps for veil");
                                                    }
                                                }
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Set order to going home");
                                                CurrentState = State.GoingHome;
                                            }
                                        }
                                        else
                                        {
                                            Me.Move(tpPos.CreepPosition);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        TinkerCrappahilationPaid.Log.Error($"[AutoPush.Jungle] {e}");
                                    }
                                    
                                    break;
                                case State.Farm:
                                    try
                                    {
                                        if (maxMarch > 0)
                                        {
                                            if (Me.IsInRange(Fountain, 1000))
                                            {
                                                CurrentState = State.RegenOnBase;
                                                break;
                                            }
                                            if (Abilities.March.CanBeCasted())
                                            {
                                                if (!Abilities.March.IsInAbilityPhase)
                                                {
                                                    var enemyCreeps = EntityManager<Creep>.Entities
                                                        .Where(x => x.IsAlive && x.IsVisible && x.IsEnemy(Me) &&
                                                                    x.IsInRange(Me, 900))
                                                        .OrderByDescending(z => z.Distance2D(Me)).FirstOrDefault();

                                                    var targetPos = Me.NetworkPosition;
                                                    var mePos = enemyCreeps != null
                                                        ? enemyCreeps.Position
                                                        : tpPos.CreepPosition;
                                                    var pos = targetPos - mePos;
                                                    pos.Normalize();
                                                    pos *= - /*Abilities.March.GetCastRange() -*/ 175;
                                                    pos = targetPos + pos;
                                                    Abilities.March.UseAbility(pos);
                                                    Me.Stop(true);
                                                    TinkerCrappahilationPaid.Log.Warn(
                                                        $"[{CurrentState}] March ({maxMarch}) CastRange: {Abilities.March.GetCastRange()}");
                                                    maxMarch--;
                                                    await Task.Delay(800);
                                                }
                                                if (AutoEscape && Me.IsVisibleToEnemies)
                                                {
                                                    TinkerCrappahilationPaid.Log.Warn(
                                                        $"[{CurrentState}] AutoEscape cuz under vision");
                                                    CurrentState = State.GoingHome;
                                                }
                                            }
                                            else
                                            {
                                                if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted)
                                                {
                                                    Abilities.SoulRing.UseAbility();
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] SoulRing");
                                                    await Task.Delay(Abilities.SoulRing.GetCastDelay());
                                                }
                                                if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted)
                                                {
                                                    Abilities.Glimmer.UseAbility(Me);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Glimmer");
                                                    await Task.Delay(Abilities.Glimmer.GetCastDelay(Me));
                                                }
                                                if (Abilities.Veil != null && Abilities.Veil.CanBeCasted)
                                                {
                                                    var enemyCreeps = EntityManager<Creep>.Entities
                                                        .Where(x => x.IsAlive && x.IsVisible && x.IsEnemy(Me) &&
                                                                    Abilities.Veil.CanHit(x) && !UnitExtensions.HasModifier(x, Abilities.Veil.TargetModifierName))
                                                        .OrderBy(z => z.Distance2D(Me)).FirstOrDefault();
                                                    if (enemyCreeps != null)
                                                    {
                                                        Abilities.Veil.UseAbility(enemyCreeps);
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] Veil To Creeps");
                                                        await Task.Delay(Abilities.Veil.GetCastDelay(enemyCreeps));
                                                    }
                                                    else
                                                    {
                                                        TinkerCrappahilationPaid.Log.Warn(
                                                            $"[{CurrentState}] cant find creeps for veil");
                                                    }
                                                }
                                                if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted &&
                                                    !UnitExtensions.HasModifier(Me,
                                                        Abilities.Bottle.TargetModifierName))
                                                {
                                                    Abilities.Bottle.UseAbility();
                                                    Me.Stop(true);
                                                    await Task.Delay(100);
                                                }

                                                if (Abilities.Rearm.CanBeCasted)
                                                {
                                                    Abilities.Rearm.UseAbility();
                                                    Me.Move(Me.Position, true);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Rearm {Abilities.Rearm.GetCastDelay()}ms");
                                                    await Task.Delay(Abilities.Rearm.GetCastDelay());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted)
                                            {
                                                Abilities.SoulRing.UseAbility();
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] SoulRing");
                                                await Task.Delay(Abilities.SoulRing.GetCastDelay());
                                            }
                                            if (Abilities.Glimmer != null && Abilities.Glimmer.CanBeCasted)
                                            {
                                                Abilities.Glimmer.UseAbility(Me);
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Glimmer");
                                                await Task.Delay(Abilities.Glimmer.GetCastDelay(Me));
                                            }
                                            if (Abilities.Veil != null && Abilities.Veil.CanBeCasted)
                                            {
                                                var enemyCreeps = EntityManager<Creep>.Entities
                                                    .Where(x => x.IsAlive && x.IsVisible && x.IsEnemy(Me) &&
                                                                Abilities.Veil.CanHit(x))
                                                    .OrderBy(z => z.Distance2D(Me)).FirstOrDefault();
                                                if (enemyCreeps != null)
                                                {
                                                    Abilities.Veil.UseAbility(enemyCreeps);
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] VeilToCreeps");
                                                    await Task.Delay(Abilities.Veil.GetCastDelay(enemyCreeps));
                                                }
                                                else
                                                {
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] cant find creeps for veil");
                                                }
                                            }

                                            if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted &&
                                                !UnitExtensions.HasModifier(Me,
                                                    Abilities.Bottle.TargetModifierName))
                                            {
                                                Abilities.Bottle.UseAbility();
                                                Me.Stop(true);
                                                await Task.Delay(100);
                                            }
                                            TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Set order to going home");
                                            CurrentState = State.GoingHome;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        TinkerCrappahilationPaid.Log.Error($"[AutoPush.Farm] {e}");
                                    }
                                    
                                    break;
                                case State.GoingHome:
                                    try
                                    {
                                        travel = (ActiveAbility)Abilities.Travel ?? Abilities.Travel2;
                                        if (Abilities.Blink != null)
                                        {
                                            if (Abilities.Blink.CanBeCasted)
                                            {
                                                var closestToFountainPosition = SaveSpotTable
                                                    .Where(x => x.IsInRange(Me, Abilities.Blink.CastRange - 50) && !x.IsInRange(Me, 150) && CheckForTrees(x))
                                                    .OrderBy(z => z.Distance2D(Fountain)).FirstOrDefault();
                                                if (!closestToFountainPosition.IsZero)
                                                {
                                                    Abilities.Blink.UseAbility(closestToFountainPosition);
                                                    await Task.Delay(
                                                        Abilities.Blink.GetCastDelay(closestToFountainPosition) + 100);
                                                }
                                            }
                                        }
                                        if (travel != null)
                                        {
                                            if (Me.IsInRange(Fountain, 1000))
                                            {
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Already on founatin.");
                                                CurrentState = State.RegenOnBase;
                                                continue;
                                            }
                                            if (travel.CanBeCasted)
                                            {
                                                travel.UseAbility(Fountain.Position);
                                                TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Tp tp fountain");
                                                CurrentState = State.RegenOnBase;
                                                await Task.Delay(2500);
                                            }
                                            else
                                            {
                                                if (Abilities.SoulRing != null && Abilities.SoulRing.CanBeCasted)
                                                {
                                                    Abilities.SoulRing.UseAbility();
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] SoulRing");
                                                    await Task.Delay(100);
                                                    continue;
                                                }
                                                if (Abilities.Bottle != null && Abilities.Bottle.CanBeCasted &&
                                                    !UnitExtensions.HasModifier(Me,
                                                        Abilities.Bottle.TargetModifierName))
                                                {
                                                    Abilities.Bottle.UseAbility();
                                                }
                                                if (Abilities.Rearm.CanBeCasted)
                                                {
                                                    Abilities.Rearm.UseAbility();
                                                    TinkerCrappahilationPaid.Log.Warn($"[{CurrentState}] Rearm");
                                                    await Task.Delay(Abilities.Rearm.GetCastDelay() + 100);
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        TinkerCrappahilationPaid.Log.Error($"[AutoPush.GoingHome] {e}");
                                    }
                                    
                                    break;
                            }
                            
                            await Task.Delay(100);
                        }
                        GameManager.ExecuteCommand("dota_player_units_auto_attack_mode 1");
                    }, 50);
                }
                else
                {
                    GameManager.ExecuteCommand("dota_player_units_auto_attack_mode 1");
                }
            };
        }

        private bool CheckForTrees(Vector3 pos)
        {
            var treeCount = EntityManager.GetEntities<Tree>().Count(x => x.IsValid && x.IsAlive && x.IsInRange(pos, 200));
            return treeCount >= 3;
        }

        public MenuItem<bool> PushLanes { get; set; }
        public MenuItem<bool> PushJungle { get; set; }

        private void BlinkPoints(EventArgs args)
        {
            foreach (var pos in SaveSpotTable)
            {
                if (Drawing.WorldToScreen(pos, out var p) && !p.IsZero)
                {
                    Drawing.DrawCircle(p, 20, 20, Color.Red);
                    /*Drawing.DrawText($"[{pos}]", p, new Vector2(25), Color.White,
                        FontFlags.None);*/
                }
            }
            
        }
        private void JunglePoints(EventArgs args)
        {
            foreach (var pos in jungleFarmPositions)
            {
                if (Drawing.WorldToScreen(pos.Start, out var p) && !p.IsZero)
                    Drawing.DrawCircle(p, 20, 20, Color.Yellow);
                if (Drawing.WorldToScreen(pos.CastPos, out var p2) && !p.IsZero)
                    Drawing.DrawCircle(p2, 20, 20, Color.YellowGreen);
            }
        }

        public MenuItem<bool> AutoEscape { get; set; }

        public MenuItem<bool> PushLaneWithAlly { get; set; }
        public MenuItem<bool> PushLaneWithEnemy { get; set; }

        private TpPosition GetTheBestLane()
        {
            var creeps = EntityManager.GetEntities<Creep>().Where(x=>x.IsAlive && x.IsVisible && x.IsSpawned);
            var allyCreeps = new List<Creep>();
            var enemyCreeps = new List<Creep>();
            foreach (var creep in creeps)
            {
                if (creep.IsAlly(Me))
                {
                    allyCreeps.Add(creep);
                }
                else
                {
                    enemyCreeps.Add(creep);
                }
            }
            var bestPos = Vector3.Zero;
            Unit tpCreep = null;
            var maxEnemyCount = 0;
            var allyHeroes = EntityManager.GetEntities<Hero>().Where(x => x.IsAlive && x.IsAlly(Me)).ToList();
            var enemyHeroes = EntityManager.GetEntities<Hero>().Where(x => x.IsAlive && x.IsEnemy(Me)).ToList();
            foreach (var allyCreep in allyCreeps)
            {
                if (!PushLaneWithAlly)
                {
                    if (allyHeroes.Any(x => x.IsInRange(allyCreep, 700)))
                    {
                        continue;
                    }
                }
                if (!PushLaneWithEnemy)
                {
                    if (enemyHeroes.Any(x => x.IsInRange(allyCreep, 700)))
                    {
                        continue;
                    }
                }
                var enemyCreepsCount = enemyCreeps.Count(x => x.IsInRange(allyCreep, 500));
                if (enemyCreepsCount > maxEnemyCount)
                {
                    maxEnemyCount = enemyCreepsCount;
                    if (Abilities.Blink != null)
                    {
                        var safePos = SaveSpotTable.Where(x => x.IsInRange(allyCreep, 1100) && CheckForTrees(x))
                            .OrderBy(z => z.Distance2D(allyCreep)).FirstOrDefault();

                        if (!safePos.IsZero)
                        {
                            tpCreep = allyCreep;
                            bestPos = safePos;
                        }
                    }
                    else
                    {
                        tpCreep = allyCreep;
                    }
                }
            }

            if (tpPos==null || tpPos.CreepPosition.IsZero)
            {
                //TODO: towers
                foreach (var allyCreep in EntityManager.GetEntities<Tower>().Where(x=>x.IsAlive && x.IsAlly(Me)))
                {
                    if (!PushLaneWithAlly)
                    {
                        if (allyHeroes.Any(x => x.IsInRange(allyCreep, 700)))
                        {
                            continue;
                        }
                    }
                    if (!PushLaneWithEnemy)
                    {
                        if (enemyHeroes.Any(x => x.IsInRange(allyCreep, 700)))
                        {
                            continue;
                        }
                    }
                    var enemyCreepsCount = enemyCreeps.Count(x => x.IsInRange(allyCreep, 500));
                    if (enemyCreepsCount > maxEnemyCount)
                    {
                        maxEnemyCount = enemyCreepsCount;
                        if (Abilities.Blink != null)
                        {
                            var safePos = SaveSpotTable.Where(x => x.IsInRange(allyCreep, 1100) && CheckForTrees(x))
                                .OrderBy(z => z.Distance2D(allyCreep.Position)).FirstOrDefault();

                            if (!safePos.IsZero)
                            {
                                tpCreep = allyCreep;
                                bestPos = safePos;
                            }
                        }
                        else
                        {
                            tpCreep = allyCreep;
                        }
                    }
                }
            }

            AfterChecking:
            return tpCreep == null ? null : new TpPosition(tpCreep, bestPos);
        }

        public State CurrentState
        {
            get => _currentState;
            set
            {
                TinkerCrappahilationPaid.Log.Debug($"Changed order from {_currentState} to {value}");
                _currentState = value;
            }
        }

        public Unit Fountain { get; set; }

        public MenuItem<KeyBind> Enable { get; set; }

        public MenuFactory Factory { get; set; }
    }
}