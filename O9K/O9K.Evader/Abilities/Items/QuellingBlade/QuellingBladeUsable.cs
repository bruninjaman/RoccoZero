﻿namespace O9K.Evader.Abilities.Items.QuellingBlade;

using System.Linq;

using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Core.Managers.Entity;
using Divine.Extensions;
using Divine.Entity.Entities.Trees;

using Metadata;

using Pathfinder.Obstacles;

internal class QuellingBladeUsable : CounterAbility
{
    private Tree tree;

    public QuellingBladeUsable(Ability9 ability, IMainMenu menu)
        : base(ability, menu)
    {
        this.CanBeCastedOnAlly = true;
    }

    public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        if (!base.CanBeCasted(ally, enemy, obstacle))
        {
            return false;
        }

        this.tree = EntityManager9.Trees
            .Where(x => x.Distance2D(ally.Position) < obstacle.EvadableAbility.Ability.Radius && x.Name == "dota_temp_tree")
            .OrderBy(x => this.Owner.GetTurnTime(x.Position))
            .FirstOrDefault(x => x.Distance2D(this.Owner.Position) < this.Ability.CastRange);

        if (this.tree == null)
        {
            return false;
        }

        return true;
    }

    public override float GetRequiredTime(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        return this.ActiveAbility.GetHitTime(this.tree.Position);
    }

    public override bool Use(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        this.MoveCamera(this.tree.Position);
        return this.ActiveAbility.UseAbility(this.tree, false, true);
    }
}