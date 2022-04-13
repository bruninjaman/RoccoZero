﻿namespace O9K.Evader.Abilities.Heroes.Kunkka.Tidebringer;

using System;

using Base.Evadable;
using Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Entities.Abilities.Heroes.Kunkka;
using Core.Logger;

using Divine.Game;
using Divine.Update;

using Metadata;

internal sealed class TidebringerEvadable : EvadableAbility, IAutoAttack
{
    private float tidebringerEndTime;

    public TidebringerEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Tidebringer = (Tidebringer)ability;

        this.Counters.Add(Abilities.SleightOfFist);
        this.Counters.Add(Abilities.Bulwark);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.PhysShield);
        this.Counters.Add(Abilities.BladeMail);
        this.Counters.Add(Abilities.ArcanistArmor);
        this.Counters.Add(Abilities.CrimsonGuard);
        this.Counters.UnionWith(Abilities.Heal);
        this.Counters.UnionWith(Abilities.Suicide);

        this.Counters.ExceptWith(Abilities.MagicImmunity);
        this.Counters.Remove(Abilities.Enrage);
        this.Counters.Remove(Abilities.HeavenlyGrace);
        this.Counters.Remove(Abilities.Bulldoze);
        this.Counters.Remove(Abilities.DarkPact);
        this.Counters.Remove(Abilities.ShadowDance);
        this.Counters.Remove(Abilities.SolarCrest);
        this.Counters.Remove(Abilities.MedallionOfCourage);
    }

    public override bool CanBeDodged { get; } = false;

    public Tidebringer Tidebringer { get; }

    public void AttackEnd()
    {
        if (GameManager.RawGameTime < this.tidebringerEndTime)
        {
            this.Pathfinder.CancelObstacle(this.Ability.Handle);
        }
    }

    public void AttackStart()
    {
        var time = GameManager.RawGameTime - (GameManager.Ping / 2000);

        UpdateManager.BeginInvoke(
            10,
            () =>
                {
                    try
                    {
                        if (!Tidebringer.IsTidebringerAnimation(this.Owner.BaseUnit.AnimationName))
                        {
                            return;
                        }

                        this.tidebringerEndTime = time + this.Owner.GetAttackPoint();

                        var obstacle = new TidebringerObstacle(this, this.Owner.Position)
                        {
                            EndCastTime = this.tidebringerEndTime,
                            EndObstacleTime = this.tidebringerEndTime
                        };

                        this.Pathfinder.AddObstacle(obstacle);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e);
                    }
                }
            );
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
}