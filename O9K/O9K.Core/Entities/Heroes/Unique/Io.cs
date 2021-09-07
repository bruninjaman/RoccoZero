﻿namespace O9K.Core.Entities.Heroes.Unique;

using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

using Metadata;

[HeroId(HeroId.npc_dota_hero_wisp)]
internal class Io : Hero9
{
    public Io(Hero baseHero)
        : base(baseHero)
    {
    }

    public override float GetTurnTime(float angleRad)
    {
        return 0;
    }
}