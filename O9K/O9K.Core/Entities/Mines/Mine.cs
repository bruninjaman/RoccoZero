﻿namespace O9K.Core.Entities.Mines;

using Divine.Entity.Entities.Units;

using Metadata;

using Units;

[UnitName("npc_dota_techies_land_mine")]
[UnitName("npc_dota_techies_stasis_trap")]
public class Mine : Unit9
{
    public Mine(Unit baseUnit)
        : base(baseUnit)
    {
        this.IsUnit = false;
    }
}