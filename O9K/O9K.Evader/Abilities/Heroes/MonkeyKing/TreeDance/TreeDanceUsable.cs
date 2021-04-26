namespace O9K.Evader.Abilities.Heroes.MonkeyKing.TreeDance
{
    using System;
    using System.Linq;

    using Base.Usable.BlinkAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;
    using Core.Logger;

    using Divine;
    using Divine.SDK.Extensions;

    using Metadata;

    using Pathfinder.Obstacles;

    using SharpDX;

    internal class TreeDanceUsable : BlinkAbility, IDisposable
    {
        private readonly Tree[] trees;

        private Tree tree;

        private ActiveAbility ult;

        private float ultEndTime;

        private Vector3 ultPosition;

        public TreeDanceUsable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
            : base(ability, pathfinder, menu)
        {
            this.trees = EntityManager.GetEntities<Tree>().ToArray();
            OrderManager.OrderAdding += OnOrderAdding;
        }

        private ActiveAbility Ult
        {
            get
            {
                if (this.ult?.IsValid != true)
                {
                    this.ult = (ActiveAbility)this.Owner.Abilities.FirstOrDefault(x => x.Id == AbilityId.monkey_king_wukongs_command);
                }

                return this.ult;
            }
        }

        public void Dispose()
        {
            OrderManager.OrderAdding -= OnOrderAdding;
        }

        public override float GetRequiredTime(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            var remainingTime = obstacle.GetEvadeTime(ally, true) - this.ActiveAbility.CastPoint;

            this.tree = this.trees
                .Where(
                    x => x.IsValid && x.IsAlive && ally.Distance(x.Position) < this.ActiveAbility.CastRange
                         && (GameManager.RawGameTime > this.ultEndTime || x.Distance2D(this.ultPosition) < this.ult.Radius - 50))
                .OrderBy(x => x.Distance2D(this.FountainPosition))
                .FirstOrDefault(x => ally.GetTurnTime(x.Position) + 0.1f < remainingTime);

            if (this.tree == null)
            {
                return 9999;
            }

            return this.ActiveAbility.GetCastDelay(this.tree.Position) + 0.15f;
        }

        public override bool Use(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            return this.ActiveAbility.UseAbility(this.tree, false, true);
        }

        private void OnOrderAdding(OrderAddingEventArgs e)
        {
            try
            {
                var order = e.Order;
                if (!e.Process || order.IsQueued || order.Type != OrderType.CastPosition)
                {
                    return;
                }

                if (order.Ability.Handle != this.Ult?.Handle)
                {
                    return;
                }

                this.ultEndTime = GameManager.RawGameTime + this.ult.Duration;
                this.ultPosition = order.Position;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}