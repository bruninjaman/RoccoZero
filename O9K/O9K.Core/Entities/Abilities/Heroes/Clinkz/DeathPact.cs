﻿namespace O9K.Core.Entities.Abilities.Heroes.Clinkz;

using System.Linq;

using Base;

using Divine.Entity;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;

using Metadata;

using O9K.Core.Entities.Abilities.Base.Types;

using O9K.Core.Entities.Units;

using O9K.Core.Managers.Entity;

[AbilityId(AbilityId.clinkz_death_pact)]
public class DeathPact : RangedAbility, IBuff
{
    public DeathPact(Ability baseAbility)
        : base(baseAbility)
    {

    }

    public string BuffModifierName { get; } = "modifier_clinkz_death_pact";

    public bool BuffsAlly { get; } = false;

    public bool BuffsOwner { get; } = true;

    public override bool TargetsEnemy { get; } = false;

    public override bool UseAbility(Unit9 target, bool queue = false, bool bypass = false)
    {
        var bestCreep = EntityManager.GetEntities<Creep>()
            .Where(x => x.Distance2D(Owner) < CastRange && x.IsAlive && x.IsVisible && x.IsSpawned)
            .OrderByDescending(x => x.MaximumHealth)
            .FirstOrDefault();

        if (bestCreep == null)
        {
            return false;
        }

        return base.UseAbility(EntityManager9.GetUnitFast(bestCreep.Handle), queue, bypass);
    }
}