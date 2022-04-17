using Divine.Entity.Entities.Abilities.Components;
using Divine.Numerics;
using Divine.Renderer;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Feature.ComboConstructor.Combos;
using InvokerAnnihilation.Feature.ComboConstructor.Emum;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;
using InvokerAnnihilation.Feature.ComboExecutor;

namespace InvokerAnnihilation.Feature.ComboConstructor.Panels.Base;

public abstract class BaseComboBuilder : IComboBuilder
{
    protected readonly IAbilityManager AbilityManager;

    protected BaseComboBuilder(ComboConstructorMenu currentMenu, string title, IAbilityManager abilityManager, IComboInfo comboInfo)
    {
        AbilityManager = abilityManager;
        Title = title;
        ComboInfo = comboInfo;
        CurrentMenu = currentMenu;
    }

    protected ComboConstructorMenu CurrentMenu { get; }
    private string Title { get; }
    public IComboInfo ComboInfo { get; }
    protected float ExtraWidth { get; set; } = 20;

    public abstract ComboBuildType Type { get; }
    public abstract void Render();
    public abstract StandardCombo? GetCurrentCombo();
    protected Color EmptyClr { get; } = new(81, 81, 81);
    protected Color ChangeClr { get; } = new(254, 30, 83);
    protected Color ContentBackgroundClr { get; } = new(45, 53, 64);
    public abstract void Dispose();

    protected void RenderTitle(RectangleF rect)
    {
        var size = RendererManager.MeasureText(Title, 30);
        RendererManager.DrawFilledRectangle(new RectangleF(rect.X, rect.Y, rect.Width, size.Y * 1.0f),
            new Color(37, 42, 48));
        RendererManager.DrawText(Title, new Vector2(rect.Center.X - size.X / 2, rect.Y), Color.White, 30);
    }


    protected void RenderCircle(RectangleF position, CircleType clr)
    {
        RendererManager.DrawImage(clr == CircleType.Gray ? "GrayCircle" : "RedCircle", position);
    }

    private float RenderDynamicComboBtn(RectangleF position, float halfHeight, IScreenToggleItem dynamicCombo)
    {
        var fontSize = CurrentMenu.SizeItem - 5;
        var measureDynamic = RendererManager.MeasureText("Dynamic", fontSize);
        var measureCombo = RendererManager.MeasureText("Combo", fontSize);
        var extraWith = 10;
        var maxTextWith = Math.Max(measureDynamic.X, measureCombo.X) + extraWith;
        RendererManager.DrawText("Dynamic", new RectangleF(position.X, position.Y, maxTextWith, halfHeight), Color.White, FontFlags.Center, fontSize);
        RendererManager.DrawText("combo", new RectangleF(position.X, position.Y + halfHeight, maxTextWith, halfHeight), Color.White, FontFlags.Center, fontSize);
        var iconSize = position.Height * .80f;
        var padding = (position.Height - iconSize) / 2;
        var iconRect = new RectangleF(position.X + maxTextWith + extraWith, position.Y + padding, iconSize, iconSize);
        RendererManager.DrawImage(AbilityId.invoker_invoke.ToString(), iconRect, ImageType.RoundAbility, true);
        RendererManager.DrawCircle(iconRect.Center, iconRect.Width / 2,
            dynamicCombo.IsActive ? ChangeClr : EmptyClr, 3);
        dynamicCombo.ActivateBtnPosition = iconRect;
        return maxTextWith + extraWith + iconSize;
    }
    private void RenderSunStrikeBtn(RectangleF position, float halfHeight, IScreenToggleItem cataclysmInCombo)
    {
        
        var fontSize = CurrentMenu.SizeItem - 5;
        var measureDynamic = RendererManager.MeasureText("Use", fontSize);
        var measureCombo = RendererManager.MeasureText("Cataclysm", fontSize);
        var extraWith = 5;
        var maxTextWith = Math.Max(measureDynamic.X, measureCombo.X) + extraWith;
        var iconSize = position.Height * .80f;
        var totalWith = maxTextWith + extraWith + iconSize;
        var extra = (position.Width - totalWith)*0.85f;
        RendererManager.DrawText("Use", new RectangleF(position.X + extra, position.Y, maxTextWith, halfHeight), Color.White, FontFlags.Center, fontSize);
        RendererManager.DrawText("Cataclysm", new RectangleF(position.X+ extra, position.Y + halfHeight, maxTextWith, halfHeight), Color.White, FontFlags.Center, fontSize);
        var padding = (position.Height - iconSize) / 2;
        var iconRect = new RectangleF(position.X + maxTextWith + extraWith + (position.Width - totalWith) * 0.85f, position.Y + padding, iconSize, iconSize);
        RendererManager.DrawImage(AbilityId.invoker_sun_strike.ToString(), iconRect, ImageType.RoundAbility, true);
        RendererManager.DrawCircle(iconRect.Center, iconRect.Width / 2,
            cataclysmInCombo.IsActive ? ChangeClr : EmptyClr, 3);
        cataclysmInCombo.ActivateBtnPosition = iconRect;
    }

    protected void RenderFooter(RectangleF position, IScreenToggleItem dynamicCombo, IScreenToggleItem cataclysmInCombo)
    {
        RendererManager.DrawFilledRectangle(position, ContentBackgroundClr);
        var halfHeight = position.Height / 2;
        var with = RenderDynamicComboBtn(position, halfHeight, dynamicCombo);
        RenderSunStrikeBtn(new RectangleF(position.X + with, position.Y, position.Width - with, position.Height), halfHeight, cataclysmInCombo);
        // RendererManager.DrawFilledRectangle(new RectangleF(position.X + with, position.Y, position.Width - with, position.Height), Color.Red);
    }
    
    protected void RenderAbilityState(RectangleF rect, AbilityId abilityId)
    {
        if (abilityId == AbilityId.dota_base_ability)
        {
            return;
        }

        var ability = AbilityManager.GetAbility(abilityId);
        if (ability == null || !ability.IsValid)
        {
            RendererManager.DrawFilledRectangle(rect, new(255, 0, 0, 100));
            return;
        }
        var cd = ability.RemainingCooldown;
        if (cd >= 0.01f)
        {
            RendererManager.DrawFilledRectangle(rect, new(155, 155, 155, 200));
            RendererManager.DrawText($"{ability.RemainingCooldown:F1}", rect, Color.White,
                FontFlags.Center | FontFlags.VerticalCenter, cd <= 99 ? CurrentMenu.SizeItem / 2f : CurrentMenu.SizeItem / 2.5f);
        }
        else if (ability.ManaCost > ability.Owner.Mana)
        {
            RendererManager.DrawFilledRectangle(rect, new(0, 0, 255, 100));
        }
        else if (!ability.CanBeCasted())
        {
            RendererManager.DrawFilledRectangle(rect, new(255, 0, 0, 100));
        }
    }
}