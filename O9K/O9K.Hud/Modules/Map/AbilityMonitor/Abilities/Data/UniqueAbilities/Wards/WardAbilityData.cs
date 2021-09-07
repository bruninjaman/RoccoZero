﻿namespace O9K.Hud.Modules.Map.AbilityMonitor.Abilities.Data.UniqueAbilities.Wards;

using System.Collections.Generic;
using System.Linq;

using Base;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Entity.Entities.Units;

using Helpers.Notificator;

internal class WardAbilityData : AbilityFullData
{
    public string UnitName { get; set; }

    public void AddDrawableAbility(List<IDrawableAbility> drawableAbilities, Vector3 position)
    {
        var drawableAbility = new DrawableWardAbility
        {
            AbilityUnitName = this.UnitName,
            AbilityTexture = this.AbilityId.ToString(),
            Position = position,
            Duration = this.Duration,
            IsShowingRange = this.ShowRange,
            Range = this.Range,
            RangeColor = this.RangeColor,
            ShowUntil = GameManager.RawGameTime + this.Duration,
        };

        drawableAbility.DrawRange();
        drawableAbilities.Add(drawableAbility);
    }

    public override void AddDrawableAbility(List<IDrawableAbility> drawableAbilities, Unit unit, INotificator notificator)
    {
        var ward = drawableAbilities.OfType<DrawableWardAbility>()
            .FirstOrDefault(x => x.Unit == null && x.AbilityUnitName == this.UnitName && x.Position.Distance2D(unit.Position) < 400);
        if (ward != null)
        {
            ward.AddUnit(unit);
            return;
        }

        var mod = unit.Modifiers.FirstOrDefault(x => x.Name == "modifier_item_buff_ward");

        var drawableAbility = new DrawableWardAbility
        {
            AbilityUnitName = this.UnitName,
            AbilityTexture = this.AbilityId.ToString(),
            Position = unit.Position,
            Unit = unit,
            Duration = this.Duration,
            IsShowingRange = this.ShowRange,
            Range = this.Range,
            RangeColor = this.RangeColor,
            AddedTime = GameManager.RawGameTime - (mod?.ElapsedTime ?? 0),
            ShowUntil = GameManager.RawGameTime + (mod?.RemainingTime ?? this.Duration)
        };

        drawableAbility.DrawRange();
        drawableAbilities.Add(drawableAbility);
    }
}