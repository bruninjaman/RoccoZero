﻿namespace O9K.Core.Entities.Mines;

using Divine.Entity.Entities.Units;

using Metadata;

[UnitName("npc_dota_techies_remote_mine")]
public class RemoteMine : Mine
{
    public RemoteMine(Unit baseUnit)
        : base(baseUnit)
    {
    }
}