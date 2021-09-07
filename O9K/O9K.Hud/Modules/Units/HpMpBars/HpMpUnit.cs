﻿namespace O9K.Hud.Modules.Units.HpMpBars;

using System;
using System.Collections.Generic;

using Core.Entities.Abilities.Base.Types;
using Core.Entities.Abilities.Items;
using Core.Entities.Units;
using Core.Helpers;
using Core.Logger;
using Core.Managers.Renderer.Utils;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Numerics;
using Divine.Renderer;

internal class HpMpUnit
{
    public readonly Unit9 unit;

    private readonly Color borderColor = Color.Black;

    private readonly float borderSize = 1 * Hud.Info.ScreenRatio;

    private readonly List<IHealthRestore> healthRestore = new List<IHealthRestore>();

    private readonly Color healthRestoreColor = new Color(89, 0, 0);

    private readonly Color manaColor = new Color(25, 54, 255);

    private readonly List<IManaRestore> manaRestore = new List<IManaRestore>();

    private readonly Color manaRestoreColor = new Color(5, 22, 140);

    public HpMpUnit(Unit9 unit)
    {
        this.unit = unit;
        this.Handle = unit.Handle;
    }

    public uint Handle { get; }

    public Rectangle9 HealthBar
    {
        get
        {
            return this.unit.HealthBar;
        }
    }

    public bool IsValid
    {
        get
        {
            return this.unit.IsValid && this.unit.IsVisible && this.unit.IsAlive && !this.unit.HideHud;
        }
    }

    public void AddAbility(IHealthRestore ability)
    {
        if (!ability.InstantRestore)
        {
            return;
        }

        this.healthRestore.Add(ability);
    }

    public void AddAbility(IManaRestore ability)
    {
        if (!ability.InstantRestore)
        {
            return;
        }

        this.manaRestore.Add(ability);
    }

    public void DrawHealthBar(
        Rectangle9 hpBar,
        int hpPositionY,
        int hpSizeY,
        bool showHpRestore,
        bool showHpAmount,
        int hpTextSize)
    {
        try
        {
            var hp = this.unit.Health;

            if (showHpRestore)
            {
                var maxHp = this.unit.MaximumHealth;
                var restore = 0f;

                var faerieAdded = false;

                foreach (var ability in this.healthRestore)
                {
                    if (!ability.IsValid || !ability.CanBeCasted(false))
                    {
                        continue;
                    }

                    if (ability.Id == AbilityId.item_faerie_fire)
                    {
                        // dont stack faerie hp restore cuz they have cooldown

                        if (faerieAdded)
                        {
                            continue;
                        }

                        faerieAdded = true;
                    }

                    restore += ability.GetHealthRestore(this.unit);
                }

                restore = Math.Min(restore, maxHp - hp);

                if (restore > 0)
                {
                    var restoreBar = new RectangleF(
                        hpBar.TopLeft.X + ((hpBar.Width * (hp / maxHp)) + 1),
                        hpBar.TopLeft.Y + hpPositionY,
                        hpBar.Width * (restore / maxHp),
                        hpBar.Height + hpSizeY);

                    //var borderBar = new RectangleF(
                    //    restoreBar.X - this.borderSize,
                    //    restoreBar.Y - this.borderSize,
                    //    restoreBar.Width + this.borderSize * 2,
                    //    restoreBar.Height + this.borderSize * 2);

                    //renderer.DrawFilledRectangle(borderBar, this.borderColor);

                    RendererManager.DrawFilledRectangle(restoreBar, this.healthRestoreColor);
                }
            }

            if (showHpAmount)
            {
                RendererManager.DrawText(
                    hp.ToString("N0"),
                    new RectangleF(hpBar.X - (hpBar.Width / 2), hpBar.Y - (hpBar.Height / 2), hpBar.Width * 2, hpBar.Height * 2),
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    hpTextSize);
            }
        }
        catch (InvalidOperationException)
        {
            //ignore
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public void DrawManaBar(
        Rectangle9 hpBar,
        int mpPositionY,
        int mpSizeY,
        bool showMpRestore,
        bool showMpAmount,
        int manaTextSize)
    {
        try
        {
            var mana = this.unit.Mana;
            var maxMana = this.unit.MaximumMana;

            var mpBar = new RectangleF(
                hpBar.BottomLeft.X,
                hpBar.BottomLeft.Y + mpPositionY,
                hpBar.Size.Width * (mana / maxMana),
                hpBar.Size.Height + mpSizeY);

            var borderBar = new RectangleF(
                mpBar.X - this.borderSize,
                mpBar.Y - this.borderSize,
                hpBar.Width + (this.borderSize * 2),
                mpBar.Height + (this.borderSize * 2));

            RendererManager.DrawFilledRectangle(borderBar, this.borderColor);
            RendererManager.DrawFilledRectangle(mpBar, this.manaColor);

            if (showMpRestore)
            {
                var restore = 0f;

                foreach (var ability in this.manaRestore)
                {
                    if (!ability.IsValid || !ability.CanBeCasted(false))
                    {
                        continue;
                    }

                    if (ability is EnchantedMango mango)
                    {
                        restore += ability.GetManaRestore(this.unit) * mango.Charges;
                    }
                    else
                    {
                        restore += ability.GetManaRestore(this.unit);
                    }
                }

                restore = Math.Min(restore, maxMana - mana);

                if (restore > 0)
                {
                    var restoreBar = new RectangleF(
                        mpBar.TopRight.X,
                        mpBar.TopRight.Y,
                        hpBar.Width * (restore / maxMana),
                        mpBar.Height);
                    RendererManager.DrawFilledRectangle(restoreBar, this.manaRestoreColor);
                }
            }

            if (showMpAmount)
            {
                RendererManager.DrawText(
                    mana.ToString("N0"),
                    borderBar,
                    Color.White,
                    FontFlags.Center | FontFlags.VerticalCenter,
                    manaTextSize);
            }
        }
        catch (InvalidOperationException)
        {
            //ignore
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    public void RemoveAbility(IHealthRestore ability)
    {
        this.healthRestore.Remove(ability);
    }

    public void RemoveAbility(IManaRestore ability)
    {
        this.manaRestore.Remove(ability);
    }
}