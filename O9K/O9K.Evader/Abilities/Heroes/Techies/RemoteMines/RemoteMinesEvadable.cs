﻿namespace O9K.Evader.Abilities.Heroes.Techies.RemoteMines;

using System;
using System.Collections.Generic;
using System.Linq;

using Base.Evadable;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Core.Logger;
using Core.Managers.Entity;
using Divine.Game;
using Divine.Update;
using Divine.Entity.Entities.Units.Components;

using Metadata;

using Pathfinder.Obstacles.Abilities.AreaOfEffect;

internal sealed class RemoteMinesEvadable : AreaOfEffectEvadable, IDisposable
{
    private readonly List<Unit9> bombs = new List<Unit9>();

    private readonly UpdateHandler techiesRemotesHandler;

    public RemoteMinesEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.techiesRemotesHandler = UpdateManager.CreateIngameUpdate(0, false, this.TechiesRemotesOnUpdate);
        EntityManager9.UnitAdded += this.OnUnitAdded;
        EntityManager9.UnitRemoved += this.OnUnitRemoved;

        this.Blinks.UnionWith(Abilities.Blink);

        this.Counters.Add(Abilities.PhaseShift);
        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.Snowball);
        this.Counters.UnionWith(Abilities.Invulnerability);
        this.Counters.Add(Abilities.TricksOfTheTrade);
        this.Counters.Add(Abilities.Supernova);
        this.Counters.UnionWith(Abilities.MagicImmunity);
        this.Counters.Add(Abilities.Mischief);
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Shield);
        this.Counters.UnionWith(Abilities.StrongMagicShield);
        this.Counters.UnionWith(Abilities.Suicide);
        this.Counters.Remove(Abilities.SpikedCarapace);
    }

    public override bool CanBeDodged { get; } = false;

    public void Dispose()
    {
        EntityManager9.UnitAdded -= this.OnUnitAdded;
        EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        UpdateManager.DestroyIngameUpdate(this.techiesRemotesHandler);
    }

    public override void PhaseCancel()
    {
    }

    public override void PhaseStart()
    {
    }

    protected override void AddObstacle()
    {
    }

    private void AddBomb(Unit9 bomb)
    {
        this.bombs.Add(bomb);
        this.techiesRemotesHandler.IsEnabled = true;
    }

    private void OnUnitAdded(Unit9 entity)
    {
        try
        {
            if (!entity.IsAlly(this.Owner) || !entity.IsAlive || entity.Name != "npc_dota_techies_remote_mine")
            {
                return;
            }

            if ((entity.UnitState & UnitState.OutOfGame) != 0)
            {
                UpdateManager.BeginInvoke(3300, () => this.AddBomb(entity));
                return;
            }

            this.AddBomb(entity);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitRemoved(Unit9 entity)
    {
        try
        {
            if (!entity.IsAlly(this.Owner) || entity.Name != "npc_dota_techies_remote_mine")
            {
                return;
            }

            this.RemoveBomb(entity);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void RemoveBomb(Unit9 bomb)
    {
        this.bombs.Remove(bomb);

        if (!this.bombs.Any(x => x.IsValid))
        {
            this.techiesRemotesHandler.IsEnabled = false;
            this.bombs.Clear();
        }
    }

    private void TechiesRemotesOnUpdate()
    {
        try
        {
            foreach (var bomb in this.bombs.ToList())
            {
                if (!bomb.IsValid || !bomb.IsAlive || !bomb.IsVisible)
                {
                    continue;
                }

                if ((bomb.BaseUnit.UnitState & UnitState.Invisible) != 0)
                {
                    continue;
                }

                this.RemoveBomb(bomb);

                var time = GameManager.RawGameTime - (GameManager.Ping / 2000) - 0.05f;

                var obstacle = new AreaOfEffectObstacle(this, bomb.Position)
                {
                    EndCastTime = time,
                    EndObstacleTime = time + this.Ability.ActivationDelay,
                };

                this.Pathfinder.AddObstacle(obstacle);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}