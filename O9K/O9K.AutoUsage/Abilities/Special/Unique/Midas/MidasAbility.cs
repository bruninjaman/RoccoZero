﻿namespace O9K.AutoUsage.Abilities.Special.Unique.Midas;

using System.Collections.Generic;
using System.Linq;

using Core.Data;
using Core.Entities;
using Core.Entities.Abilities.Base.Components.Base;
using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Managers.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Abilities.Components;

using Settings;

[AbilityId(AbilityId.item_hand_of_midas)]
internal class MidasAbility : SpecialAbility
{
    private static readonly Dictionary<string, int> Experience = new Dictionary<string, int>();

    private readonly MidasSettings settings;

    public MidasAbility(IActiveAbility ability, GroupSettings settings)
        : base(ability)
    {
        this.settings = new MidasSettings(settings.Menu, ability);
    }

    public override bool UseAbility(List<Unit9> heroes)
    {
        var ignoreExp = this.Ability.Owner.Level >= GameData.MaxHeroLevel;
        var creeps = EntityManager9.Units.Where(x => x.IsCreep && !x.IsAncient && !x.IsAlly(this.Owner) && x.IsVisible && x.IsAlive);

        foreach (var creep in creeps)
        {
            if (!this.Ability.CanHit(creep))
            {
                continue;
            }

            if (!ignoreExp && GetCreepExp(creep) < this.settings.Experience)
            {
                continue;
            }

            return this.Ability.UseAbility(creep);
        }

        return false;
    }

    private static int GetCreepExp(Entity9 creep)
    {
        if (!Experience.TryGetValue(creep.Name, out var exp))
        {
            Experience[creep.Name] = Unit.GetKeyValueByName(creep.Name).GetSubKey("BountyXP")?.GetInt32() ?? 0;
        }

        return exp;
    }
}