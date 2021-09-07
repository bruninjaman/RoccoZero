﻿namespace O9K.Core.Entities.Abilities.Heroes.Kunkka;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.kunkka_torrent_storm)]
public class TorrentStorm : ActiveAbility
{
    public TorrentStorm(Ability baseAbility)
        : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "torrent_max_distance");
    }
}