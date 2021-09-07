﻿namespace O9K.Core.Entities.Heroes.Unique;

using System.Linq;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Spells.Meepo;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

using Metadata;

[HeroId(HeroId.npc_dota_hero_meepo)]
public class Meepo : Hero9
{
    public Meepo(Hero baseHero)
        : base(baseHero)
    {
        if (this.IsIllusion)
        {
            return;
        }

        var ult = (DividedWeStand)this.BaseUnit.Spellbook.Spells.FirstOrDefault(x => x.Id == AbilityId.meepo_divided_we_stand);
        if (ult == null)
        {
            return;
        }

        this.MeepoIndex = ult.UnitIndex;
    }

    public bool IsMainMeepo
    {
        get
        {
            return this.MeepoIndex == 0;
        }
    }

    public int MeepoIndex { get; } = -1;
}