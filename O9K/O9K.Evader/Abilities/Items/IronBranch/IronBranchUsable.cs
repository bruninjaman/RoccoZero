﻿namespace O9K.Evader.Abilities.Items.IronBranch;

using System.Linq;

using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;
using Core.Extensions;
using Divine.Entity;
using Divine.Numerics;
using Divine.Particle;
using Divine.Modifier.Modifiers;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using Pathfinder.Obstacles;

internal class IronBranchUsable : CounterAbility
{
    private Modifier kineticField;

    private Particle pounce;

    private Vector3 usePosition;

    public IronBranchUsable(Ability9 ability, IMainMenu menu)
        : base(ability, menu)
    {
    }

    private Modifier KineticField
    {
        get
        {
            if (this.kineticField?.IsValid != true)
            {
                this.kineticField = EntityManager.GetEntities<Unit>()
                    .Where(x => x.IsValid && x.Team != this.Ability.Owner.Team && x.NetworkName == "CDOTA_BaseNPC")
                    .SelectMany(x => x.Modifiers)
                    .FirstOrDefault(x => x.IsValid && x.Name == "modifier_disruptor_kinetic_field_thinker");
            }

            return this.kineticField;
        }
    }

    private Particle Pounce
    {
        get
        {
            if (this.pounce?.IsValid != true)
            {
                this.pounce = ParticleManager.Particles.FirstOrDefault(x => x.IsValid && x.Name.Contains("pounce_leash.vpcf"));
            }

            return this.pounce;
        }
    }

    public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        if (!base.CanBeCasted(ally, enemy, obstacle))
        {
            return false;
        }

        switch (obstacle.EvadableAbility.Ability.Id)
        {
            case AbilityId.slark_pounce:
            {
                var center = this.Pounce?.GetControlPoint(3);
                if (center == null)
                {
                    return false;
                }

                if (ally.Distance(center.Value) < 370)
                {
                    return false;
                }

                this.usePosition = ally.Position.Extend2D(center.Value, 20);
                return true;
            }
            case AbilityId.disruptor_kinetic_field:
            {
                var center = this.KineticField?.Owner.Position;
                if (center == null)
                {
                    return false;
                }

                var distance = ally.Distance(center.Value);
                if (distance < 325 || distance > 340 || ally.GetAngle(center.Value) < 2)
                {
                    return false;
                }

                this.usePosition = ally.Position.Extend2D(center.Value, 20);
                return true;
            }
        }

        return false;
    }

    public override bool Use(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        this.MoveCamera(this.usePosition);
        return this.ActiveAbility.UseAbility(this.usePosition, false, true);
    }
}