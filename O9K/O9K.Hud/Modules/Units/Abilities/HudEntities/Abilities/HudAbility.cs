﻿namespace O9K.Hud.Modules.Units.Abilities.HudEntities.Abilities;

using System;

using Core.Entities.Abilities.Base;
using Core.Logger;
using Core.Managers.Renderer.Utils;

using Divine.Numerics;
using Divine.Renderer;

internal class HudAbility
{
    private readonly bool displayLevel;

    private readonly int maxLevel;

    private bool abilityEnabled = true;

    public HudAbility(Ability9 ability)
    {
        this.Ability = ability;
        this.maxLevel = ability.MaximumLevel;
        this.displayLevel = this.maxLevel > 1;
    }

    public Ability9 Ability { get; }

    public bool ShouldDisplay
    {
        get
        {
            if (!this.abilityEnabled)
            {
                return false;
            }

            if (!this.Ability.IsValid)
            {
                return false;
            }

            return this.Ability.IsAvailable /* && !this.Ability.IsHidden*/;
        }
    }

    public void ChangeEnabled(bool enabled)
    {
        this.abilityEnabled = enabled;
    }

    public void Draw(Rectangle9 position, float cooldownSize)
    {
        try
        {
            RendererManager.DrawImage(this.Ability.TextureName, position, ImageType.Ability);

            if (this.Ability.IsCasting || this.Ability.IsChanneling)
            {
                RendererManager.DrawRectangle(position - 3f, Color.LightGreen, 3);
            }
            else
            {
                RendererManager.DrawRectangle(position - 1, Color.Black);
            }

            if (this.displayLevel)
            {
                var level = this.Ability.Level;
                if (level == 0)
                {
                    RendererManager.DrawImage("o9k.ability_0lvl_bg", position);
                    return;
                }

                var levelText = level.ToString("N0");
                var levelSize = RendererManager.MeasureText(levelText, position.Width * 0.45f);
                var levelPosition = position.SinkToBottomLeft(levelSize.X, levelSize.Y * 0.8f);

                RendererManager.DrawImage("o9k.ability_lvl_bg", levelPosition);
                RendererManager.DrawText(levelText, levelPosition, Color.White, FontFlags.VerticalCenter, position.Width * 0.45f);
            }

            if (this.Ability.IsDisplayingCharges)
            {
                var chargesText = this.Ability.Charges.ToString("N0");
                var chargesPosition = position.SinkToBottomRight(position.Width * 0.5f, position.Height * 0.5f);
                RendererManager.DrawImage("o9k.charge_bg", chargesPosition);
                RendererManager.DrawImage("o9k.outline_green", chargesPosition * 1.07f);
                RendererManager.DrawText(chargesText, chargesPosition, Color.White, FontFlags.Center, position.Width * 0.45f);
            }

            if (this.Ability.IsChanneling)
            {
                return;
            }

            var cooldown = this.Ability.RemainingCooldown;
            if (cooldown > 0)
            {
                RendererManager.DrawImage("o9k.ability_cd_bg", position);
                RendererManager.DrawText(
                    Math.Ceiling(cooldown).ToString("N0"),
                    position,
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    cooldownSize);
            }
            else if (this.Ability.ManaCost > this.Ability.Owner.Mana)
            {
                RendererManager.DrawImage("o9k.ability_mana_bg", position);
                RendererManager.DrawText(
                    Math.Ceiling((this.Ability.ManaCost - this.Ability.Owner.Mana) / this.Ability.Owner.ManaRegeneration)
                        .ToString("N0"),
                    position,
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    cooldownSize);
            }
            else if (!this.Ability.IsUsable)
            {
                RendererManager.DrawImage("o9k.ability_0lvl_bg", position);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public void DrawMinimalistic(Rectangle9 position, float cooldownSize)
    {
        try
        {
            RendererManager.DrawImage("o9k.ability_minimal_bg", position);
            var levelHeight = position.Height * 0.2f;

            var cooldown = this.Ability.RemainingCooldown;
            if (cooldown > 0)
            {
                position = position.MoveTopBorder(cooldownSize * 0.8f);
                RendererManager.DrawImage("o9k.ability_minimal_cd_bg", position);
                RendererManager.DrawText(
                    Math.Ceiling(cooldown).ToString("N0"),
                    position.MoveTopBorder(cooldownSize * 0.2f),
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    cooldownSize);
            }
            else if (this.Ability.ManaCost > this.Ability.Owner.Mana)
            {
                position = position.MoveTopBorder(cooldownSize * 0.8f);
                RendererManager.DrawImage("o9k.ability_minimal_mana_bg", position);
                RendererManager.DrawText(
                    Math.Ceiling((this.Ability.ManaCost - this.Ability.Owner.Mana) / this.Ability.Owner.ManaRegeneration)
                        .ToString("N0"),
                    position.MoveTopBorder(cooldownSize * 0.2f),
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    cooldownSize);
            }

            RendererManager.DrawRectangle(position - 1, this.Ability.IsCasting ? Color.LightGreen : Color.Black);

            var rec = position - 5;
            var levelWidth = rec.Width / this.maxLevel;
            var space = levelWidth * 0.05f;
            var posY = rec.Bottom - levelHeight;
            var levelDrawWidth = levelWidth - (space * 2);
            var lvl = this.Ability.Level;

            for (var i = 0; i < lvl; i++)
            {
                var lvlPos = new Rectangle9(rec.X + space, posY, levelDrawWidth, levelHeight);
                RendererManager.DrawImage("o9k.ability_level_rec", lvlPos);
                rec = rec.MoveLeftBorder(levelWidth);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}