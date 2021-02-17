using System.Collections.Generic;

using Divine.SDK.Extensions;
using Divine.SDK.Managers.Log;
using Divine.SDK.Managers.Update;

using SharpDX;

namespace Divine.Divine.SkywrathMage
{
    internal static class Orbwalker
    {
        private static readonly HashSet<NetworkActivity> attackActivities = new()
        {
            NetworkActivity.Attack,
            NetworkActivity.Attack2,
            NetworkActivity.AttackEvent
        };

        private static readonly HashSet<NetworkActivity> attackCancelActivities = new()
        {
            NetworkActivity.Idle,
            NetworkActivity.IdleRare,
            NetworkActivity.Move
        };

        static Orbwalker()
        {
            Owner = EntityManager.LocalHero;

            Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
        }

        public static Vector3 OrbwalkingPoint { get; set; } = Vector3.Zero;

        private static float LastAttackOrderIssuedTime { get; set; }

        private static float LastAttackTime { get; set; }

        private static float LastMoveOrderIssuedTime { get; set; }

        private static Unit Owner { get; }

        private static float PingTime
        {
            get
            {
                return GameManager.Ping / 2000f;
            }
        }

        private static float TurnEndTime { get; set; }

        public static bool Attack(Unit unit, float time)
        {
            if ((time - LastAttackOrderIssuedTime) < (5f / 1000f))
            {
                return false;
            }

            TurnEndTime = GetTurnTime(unit, time);

            if (Owner.Attack(unit))
            {
                LastAttackOrderIssuedTime = time;
                return true;
            }

            return false;
        }

        public static bool Attack(Unit unit)
        {
            return Attack(unit, GameManager.RawGameTime);
        }

        public static bool CanAttack(Unit target, float time)
        {
            return Owner.CanAttack() && (GetTurnTime(target, time) - LastAttackTime) > (1f / Owner.AttacksPerSecond);
        }

        public static bool CanAttack(Unit target)
        {
            return CanAttack(target, GameManager.RawGameTime);
        }

        public static bool CanMove(float time)
        {
            return (((time - 0.1f) + PingTime) - LastAttackTime) > Owner.AttackPoint();
        }

        public static bool CanMove()
        {
            return CanMove(GameManager.RawGameTime);
        }

        public static float GetTurnTime(Entity unit, float time)
        {
            return time + PingTime + Owner.TurnTime(unit.Position) + (100f / 1000f);
        }

        public static float GetTurnTime(Entity unit)
        {
            return GetTurnTime(unit, GameManager.RawGameTime);
        }

        public static bool Move(Vector3 position, float time)
        {
            if (Owner.Position.Distance(position) < 60)
            {
                return false;
            }

            if ((time - LastMoveOrderIssuedTime) < (60f / 1000f))
            {
                return false;
            }

            if (Owner.Move(position))
            {
                LastMoveOrderIssuedTime = time;
                return true;
            }

            return false;
        }

        public static bool Move(Vector3 position)
        {
            return Move(position, GameManager.RawGameTime);
        }

        public static bool OrbwalkTo(Unit target)
        {
            return OrbwalkTo(target, GameManager.MousePosition);
        }

        public static bool OrbwalkTo(Unit target, Vector3 position)
        {
            var time = GameManager.RawGameTime;

            // turning
            if (TurnEndTime > time)
            {
                return false;
            }

            // owner disabled
            if (Owner.IsChanneling() || !Owner.IsAlive || Owner.IsStunned())
            {
                return false;
            }

            var validTarget = target != null;

            // move
            if ((!validTarget || !CanAttack(target)) && CanMove(time))
            {
                return Move(position, time);
            }

            // attack
            if (validTarget && CanAttack(target))
            {
                return Attack(target, time);
            }

            return false;
        }

        private static void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_networkactivity")
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                if (sender != Owner)
                {
                    return;
                }

                var newNetworkActivity = (NetworkActivity)e.NewValue.GetInt32();

                if (attackActivities.Contains(newNetworkActivity))
                {
                    LastAttackTime = GameManager.RawGameTime - PingTime;
                }
                else if (attackCancelActivities.Contains(newNetworkActivity))
                {
                    if (!CanMove(GameManager.RawGameTime + 0.05f))
                    {
                        LastAttackTime = 0;
                    }
                }
            });
        }

        public static void MoveToMousePosition()
        {
            Move(GameManager.MousePosition);
        }
    }
}