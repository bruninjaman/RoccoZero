﻿namespace O9K.Evader.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;

using Abilities.Base;

using Core.Entities.Heroes;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Entity;
using Core.Managers.Menu.EventArgs;

using Divine.Entity.Entities.Units.Components;
using Divine.Game;
using Divine.Map.Components;
using Divine.Numerics;
using Divine.Renderer;
using Divine.Update;

using Evader.EvadeModes;

using Metadata;

using Pathfinder;
using Pathfinder.Obstacles.Types;

using Settings;

internal class Debugger : IEvaderService, IDebugger
{
    private const float TextSize = 18f;

    private readonly IAbilityManager abilityManager;

    private readonly IActionManager actionManager;

    private readonly List<EvadeResult> evadeResults = new List<EvadeResult>();

    private readonly Pathfinder pathfinder;

    private readonly DebugMenu settings;

    private Owner owner;

    public Debugger(
        IPathfinder pathfinder,
        IMainMenu menu,
        IAbilityManager abilityManager,
        IActionManager actionManager)
    {
        this.pathfinder = (Pathfinder)pathfinder;
        this.settings = menu.Debug;
        this.abilityManager = abilityManager;
        this.actionManager = actionManager;
    }

    public LoadOrder LoadOrder { get; } = LoadOrder.Debugger;

    public void Activate()
    {
        this.owner = EntityManager9.Owner;

        this.settings.DrawAbilities.ValueChange += this.DrawAbilitiesOnValueChanged;
        this.settings.DrawEvadeResult.ValueChange += this.DrawEvadeResultOnValueChanged;
        this.settings.DrawObstacleMap.ValueChange += this.DrawObstacleMapOnValueChanged;
        this.settings.DrawIntersections.ValueChange += this.DrawIntersectionsOnValueChanged;
        this.settings.DrawUsableAbilities.ValueChange += this.DrawUsableAbilitiesOnValueChanged;
        this.settings.DrawEvadableAbilities.ValueChange += this.DrawEvadableAbilitiesOnValueChanged;
    }

    public void AddEvadeResult(EvadeResult evadeResult)
    {
        if (!this.settings.DrawEvadeResult.IsEnabled)
        {
            return;
        }

        if (evadeResult == null || evadeResult.State == EvadeResult.EvadeState.TooEarly
                                || evadeResult.State == EvadeResult.EvadeState.Ignore)
        {
            return;
        }

        if (this.evadeResults.Contains(evadeResult))
        {
            return;
        }

        if (this.evadeResults.Count >= 7)
        {
            this.evadeResults.Remove(this.evadeResults.Last());
        }

        this.evadeResults.Add(evadeResult);
        UpdateManager.BeginInvoke(7500, () => this.evadeResults.Remove(evadeResult));
    }

    public void Dispose()
    {
        RendererManager.Draw -= this.DrawEvadableAbilities;
        RendererManager.Draw -= this.DrawIntersections;
        RendererManager.Draw -= this.DrawUsableAbilities;
        RendererManager.Draw -= this.DrawAbilityObstacles;
        RendererManager.Draw -= this.DrawMap;
    }

    private void DrawAbilitiesOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            if (e.OldValue)
            {
                //to prevent mini freeze on 1st particle creation on usage
                //var fix = new AbilityObstacleDrawer();
                //fix.DrawArcRectangle(Vector3.Zero, Vector3.Zero + 100, 100, 200);
                //fix.DrawCircle(Vector3.Zero, 100);
            }

            RendererManager.Draw += this.DrawAbilityObstacles;
        }
        else
        {
            RendererManager.Draw -= this.DrawAbilityObstacles;
        }
    }

    private void DrawAbilityObstacles()
    {
        try
        {
            foreach (var obstacle in this.pathfinder.Obstacles.ToList())
            {
                if (obstacle is IDrawable drawable)
                {
                    drawable.Draw();
                }
            }
        }
        catch (Exception e)
        {
            Logger.Warn(e.ToString());
        }
    }

    private void DrawEvadableAbilities()
    {
        try
        {
            foreach (var unit in this.abilityManager.EvadableAbilities.Select(x => x.Ability.Owner)
                .Where(x => x.IsAlive && x.IsVisible)
                .Distinct()
                .ToList())
            {
                var position = RendererManager.WorldToScreen(unit.Position);
                if (position.IsZero)
                {
                    continue;
                }

                position -= new Vector2(-50, 110);

                foreach (var ability in this.abilityManager.EvadableAbilities.Where(x => x.Owner.Equals(unit))
                    .OrderBy(x => x.Ability.BaseAbility.AbilitySlot)
                    .ToList())
                {
                    var text = ability.Ability.DisplayName;
                    if (ability is IModifierCounter modifier)
                    {
                        text += " (Modifier " + (modifier.ModifierEnemyCounter ? "enemy" : "ally") + ")";
                    }

                    RendererManager.DrawText(
                        text,
                        (position += new Vector2(0, 20)) + new Vector2(20, 0),
                        ability.Ability.BaseAbility.IsInAbilityPhase ? Color.LawnGreen :
                        ability.Ability.CanBeCasted() ? Color.White : Color.Gray,
                        "Arial",
                        TextSize);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Warn(e.ToString());
        }
    }

    private void DrawEvadableAbilitiesOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.DrawEvadableAbilities;
        }
        else
        {
            RendererManager.Draw -= this.DrawEvadableAbilities;
        }
    }

    private void DrawEvadeResultOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.ShowEvadeResult;
        }
        else
        {
            RendererManager.Draw -= this.ShowEvadeResult;
        }
    }

    private void DrawIntersections()
    {
        try
        {
            foreach (var unit in EntityManager9.Units.Where(x => x.IsHero && x.IsAlive && x.IsAlly(this.owner.Team)).ToList())
            {
                var obstacles = this.pathfinder.GetIntersectingObstacles(unit).ToList();
                if (obstacles.Count == 0)
                {
                    continue;
                }

                var position = RendererManager.WorldToScreen(unit.Position);
                if (position.IsZero)
                {
                    continue;
                }

                foreach (var obstacle in obstacles)
                {
                    RendererManager.DrawText(
                         obstacle.EvadableAbility.Ability.DisplayName + " (" + obstacle.Id + ") " + obstacle.GetEvadeTime(unit, false).ToString("n2"),
                        (position += new Vector2(0, 20)) + new Vector2(-120, 0),
                        this.actionManager.IsObstacleIgnored(unit, obstacle) ? Color.Gray : Color.White,
                        "Arial",
                        TextSize);
                }
            }
        }
        catch
        {
            //ignored
        }
    }

    private void DrawIntersectionsOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.DrawIntersections;
        }
        else
        {
            RendererManager.Draw -= this.DrawIntersections;
        }
    }

    private void DrawMap()
    {
        try
        {
            var center = GameManager.MousePosition;
            const int CellCount = 40;
            for (var i = 0; i < CellCount; ++i)
            {
                for (var j = 0; j < CellCount; ++j)
                {
                    Vector2 p;
                    p.X = (this.pathfinder.NavMesh.CellSize * (i - (CellCount / 2))) + center.X;
                    p.Y = (this.pathfinder.NavMesh.CellSize * (j - (CellCount / 2))) + center.Y;
                    Color color;

                    var isFlying = this.owner.Hero.MoveCapability == MoveCapability.Fly
                                   || (this.owner.Hero.UnitState & UnitState.Flying) != 0;
                    var flag = this.pathfinder.NavMesh.GetCellFlags(p);
                    if (!isFlying && (flag & MeshCellFlags.Walkable) != 0)
                    {
                        color = (flag & MeshCellFlags.Tree) != 0 ? Color.Purple : Color.Green;
                        if ((flag & MeshCellFlags.GridFlagObstacle) != 0)
                        {
                            color = Color.Pink;
                        }
                    }
                    else if (isFlying && (flag & MeshCellFlags.MovementBlocker) == 0)
                    {
                        color = Color.Green;
                    }
                    else
                    {
                        color = Color.Red;
                    }

                    RendererManager.DrawFilledRectangle(new RectangleF(i * 10, 50 + ((CellCount - j - 1) * 10), 9, 9), color);
                }
            }

            this.pathfinder.NavMesh.GetCellPosition(this.owner.Hero.Position - center, out var heroX, out var heroY);
            heroX += CellCount / 2;
            heroY += CellCount / 2;

            if (heroX >= 0 && heroX < CellCount && heroY >= 0 && heroY < CellCount)
            {
                RendererManager.DrawFilledRectangle(new RectangleF(heroX * 10, 50 + ((CellCount - heroY - 1) * 10), 9, 9), Color.Blue);
            }

            //this.pathfinder.NavMesh.GetCellPosition(GameManager.MousePosition - center, out var mouseX, out var mouseY);
            //mouseX += CellCount / 2;
            //mouseY += CellCount / 2;

            //if (mouseX >= 0 && mouseX < CellCount && mouseY >= 0 && mouseY < CellCount)
            //{
            //    Drawing.DrawRect(
            //        new Vector2(mouseX * 10, 50 + ((CellCount - mouseY - 1) * 10)),
            //        new Vector2(9),
            //        SharpDX.Color.White,
            //        false);
            //}
        }
        catch (Exception e)
        {
            Logger.Warn(e.ToString());
        }
    }

    private void DrawObstacleMapOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.DrawMap;
        }
        else
        {
            RendererManager.Draw -= this.DrawMap;
        }
    }

    private void DrawUsableAbilities()
    {
        try
        {
            var units = this.abilityManager.UsableBlinkAbilities.Select(x => x.Ability.Owner)
                .Concat(this.abilityManager.UsableCounterAbilities.Select(x => x.Ability.Owner))
                .Concat(this.abilityManager.UsableDisableAbilities.Select(x => x.Ability.Owner))
                .Where(x => x.IsAlive)
                .Distinct()
                .ToList();

            foreach (var unit in units)
            {
                var position = RendererManager.WorldToScreen(unit.Position);
                if (position.IsZero)
                {
                    continue;
                }

                position -= new Vector2(200, 110);

                if (this.actionManager.IsInputBlocked(unit))
                {
                    RendererManager.DrawText("Blocked", position + new Vector2(150, 20), Color.Red, "Arial", TextSize);
                }

                var blinks = this.abilityManager.UsableBlinkAbilities.Where(x => x.Ability.Owner.Equals(unit))
                    .OrderBy(x => x.Ability.BaseAbility.AbilitySlot)
                    .ToList();

                if (blinks.Count > 0)
                {
                    RendererManager.DrawText("Blinks:", (position += new Vector2(0, 20)) + new Vector2(10, 0), Color.White, "Arial", TextSize);

                    foreach (var ability in blinks)
                    {
                        RendererManager.DrawText(
                            ability.Ability.DisplayName,
                            (position += new Vector2(0, 20)) + new Vector2(20, 0),
                            ability.Ability.BaseAbility.IsInAbilityPhase ? Color.LawnGreen :
                            ability.Ability.CanBeCasted() ? Color.White : Color.Gray,
                            "Arial",
                            TextSize);
                    }
                }

                var counters = this.abilityManager.UsableCounterAbilities.Where(x => x.Ability.Owner.Equals(unit))
                    .OrderBy(x => x.Ability.BaseAbility.AbilitySlot)
                    .ToList();

                if (counters.Count > 0)
                {
                    RendererManager.DrawText(
                        "Counters:",
                        (position += new Vector2(0, 20)) + new Vector2(10, 0),
                        Color.White,
                        "Arial",
                        TextSize);

                    foreach (var ability in counters)
                    {
                        RendererManager.DrawText(
                            ability.Ability.DisplayName,
                            (position += new Vector2(0, 20)) + new Vector2(20, 0),
                            ability.Ability.BaseAbility.IsInAbilityPhase ? Color.LawnGreen :
                            ability.Ability.CanBeCasted() ? Color.White : Color.Gray,
                            "Arial",
                            TextSize);
                    }
                }

                var disables = this.abilityManager.UsableDisableAbilities.Where(x => x.Ability.Owner.Equals(unit))
                    .OrderBy(x => x.Ability.BaseAbility.AbilitySlot)
                    .ToList();

                if (disables.Count > 0)
                {
                    RendererManager.DrawText(
                        "Disables:",
                        (position += new Vector2(0, 20)) + new Vector2(10, 0),
                        Color.White,
                        "Arial",
                        TextSize);

                    foreach (var ability in disables)
                    {
                        RendererManager.DrawText(
                            ability.Ability.DisplayName,
                            (position += new Vector2(0, 20)) + new Vector2(20, 0),
                            ability.Ability.BaseAbility.IsInAbilityPhase ? Color.LawnGreen :
                            ability.Ability.CanBeCasted() ? Color.White : Color.Gray,
                            "Arial",
                            TextSize);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Logger.Warn(e.ToString());
        }
    }

    private void DrawUsableAbilitiesOnValueChanged(object sender, SwitcherEventArgs e)
    {
        if (e.NewValue)
        {
            RendererManager.Draw += this.DrawUsableAbilities;
        }
        else
        {
            RendererManager.Draw -= this.DrawUsableAbilities;
        }
    }

    private void ShowEvadeResult()
    {
        try
        {
            var position = new Vector2(Hud.Info.ScreenSize.X, 23);

            for (var i = this.evadeResults.Count - 1; i >= 0; i--)
            {
                var result = this.evadeResults[i];
                var text = string.Empty;

                if (result.State == EvadeResult.EvadeState.Failed)
                {
                    text = result.Ally + " can't evade " + result.EnemyAbility + (result.IsModifier ? " (modifier)" : "");
                }
                else
                {
                    switch (result.Mode)
                    {
                        case EvadeMode.Dodge:
                        {
                            if (result.AllyAbility == null)
                            {
                                text = result.Ally + " dodging " + result.EnemyAbility;
                            }
                            else
                            {
                                text = result.Ally + " using " + result.AllyAbility + " to dodge " + result.EnemyAbility;
                            }

                            break;
                        }
                        case EvadeMode.Counter:
                        {
                            text = result.AbilityOwner + " using " + result.AllyAbility + " (" + (result.IsModifier ? "modifier " : "")
                                   + "counter) on " + result.Ally + " vs " + result.EnemyAbility;
                            break;
                        }
                        case EvadeMode.Blink:
                        {
                            text = result.AbilityOwner + " using " + result.AllyAbility + " (" + (result.IsModifier ? "modifier " : "")
                                   + "blink) vs " + result.EnemyAbility;
                            break;
                        }
                        case EvadeMode.Disable:
                        {
                            text = result.AbilityOwner + " using " + result.AllyAbility + " (" + (result.IsModifier ? "modifier " : "")
                                   + "disable) on " + result.Enemy + " vs " + result.EnemyAbility;
                            break;
                        }
                        case EvadeMode.GoldSpend:
                        {
                            text = result.Ally + " trying to spend gold vs " + result.EnemyAbility;
                            break;
                        }
                    }
                }

                var size = RendererManager.MeasureText(text, TextSize + 2);
                position += new Vector2(0, TextSize + 2);

                RendererManager.DrawLine(
                    position - new Vector2(size.X + 20, ((TextSize + 2) / -2) - 3),
                    position - new Vector2(0, ((TextSize + 2) / -2) - 3),
                    new Color(0, 0, 0, 220),
                    TextSize + 2);

                RendererManager.DrawText(
                    text,
                    position - new Vector2(size.X + 10, 0),
                    result.State == EvadeResult.EvadeState.Failed ? Color.Red : Color.Green,
                    TextSize + 2);
            }
        }
        catch
        {
            //ignore
        }
    }
}