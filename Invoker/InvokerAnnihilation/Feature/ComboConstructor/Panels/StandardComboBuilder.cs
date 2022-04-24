using Divine.Entity.Entities.Abilities.Components;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Numerics;
using Divine.Renderer;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor.Combos;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Factory;
using InvokerAnnihilation.Feature.ComboConstructor.Emum;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;
using InvokerAnnihilation.Feature.ComboConstructor.Panels.Base;
using InvokerAnnihilation.Feature.ComboExecutor;

namespace InvokerAnnihilation.Feature.ComboConstructor.Panels;

public class StandardComboBuilder : BaseComboBuilder
{
    public List<ICombo?> Combos { get; set; }
    public List<StandardCombo?> CustomCombos { get; set; }
    public override DynamicCombo DynamicComboSettings { get; set; }
    public override CataclysmInCombo CataclysmInCombo { get; set; } = new CataclysmInCombo();
    public override void Start()
    {
        InputManager.MouseKeyDown += InputOnMouseClick;
        InputManager.MouseMove += InputManagerOnMouseMove;
    }

    public override void Stop()
    {
        InputManager.MouseKeyDown -= InputOnMouseClick;
        InputManager.MouseMove -= InputManagerOnMouseMove;
    }

    public int ComboCount => CustomCombos.Count;
    public int MaxAbilities { get; } = 6;

    public StandardComboBuilder(MenuConfig menuConfig, IAbilityManager abilityManager, IComboInfo comboInfo) : base(
        menuConfig.ComboConstructorMenu, "Standard Combo", abilityManager, comboInfo)
    {
        Combos = new List<ICombo?>
        {
            new StandardCombo(abilityManager, AbilityId.invoker_ice_wall, AbilityId.invoker_cold_snap,
                    AbilityId.invoker_alacrity)
                {IsActive = true},
            // new StandardCombo(abilityManager, AbilityId.invoker_cold_snap, AbilityId.invoker_forge_spirit,
            //         AbilityId.invoker_alacrity)
            //     {IsActive = true},
            new StandardCombo(abilityManager, AbilityId.invoker_cold_snap, AbilityId.invoker_forge_spirit,
                AbilityId.invoker_sun_strike),
            new StandardCombo(abilityManager, AbilityId.invoker_tornado, AbilityId.invoker_sun_strike),
            new StandardCombo(abilityManager, AbilityId.invoker_tornado, AbilityId.invoker_chaos_meteor,
                AbilityId.invoker_ice_wall),
            new StandardCombo(abilityManager, AbilityId.invoker_tornado, AbilityId.invoker_chaos_meteor,
                AbilityId.invoker_deafening_blast),
            new StandardCombo(abilityManager, AbilityId.invoker_tornado, AbilityId.invoker_chaos_meteor,
                AbilityId.invoker_cold_snap),
            new ComboFactory(abilityManager).SetAbilities(AbilityId.item_cyclone, AbilityId.invoker_sun_strike,
                AbilityId.invoker_chaos_meteor, AbilityId.invoker_deafening_blast).Build(),
            new ComboFactory(abilityManager)
                .SetAbilities(AbilityId.invoker_tornado, AbilityId.invoker_emp, AbilityId.invoker_chaos_meteor,
                    AbilityId.invoker_deafening_blast).SetItemsToHave(AbilityId.item_refresher).Build(),
            new ComboFactory(abilityManager)
                .SetAbilities(AbilityId.invoker_tornado, AbilityId.invoker_emp, AbilityId.invoker_chaos_meteor,
                    AbilityId.item_refresher, AbilityId.invoker_deafening_blast)
                .SetItemsToHave(AbilityId.item_refresher).Build(),
            new ComboFactory(abilityManager)
                .SetAbilities(AbilityId.invoker_tornado, AbilityId.invoker_sun_strike, AbilityId.invoker_chaos_meteor,
                    AbilityId.item_refresher, AbilityId.invoker_deafening_blast)
                .SetItemsToHave(AbilityId.item_refresher).Build(),
            new ComboFactory(abilityManager)
                .SetAbilities(AbilityId.item_blink, AbilityId.invoker_deafening_blast, AbilityId.invoker_chaos_meteor,
                    AbilityId.invoker_sun_strike, AbilityId.item_refresher)
                .SetItemsToHave(AbilityId.item_blink, AbilityId.item_refresher).Build(),
            new ComboFactory(abilityManager)
                .SetAbilities(AbilityId.invoker_tornado, AbilityId.invoker_emp, AbilityId.invoker_chaos_meteor,
                    AbilityId.invoker_cold_snap, AbilityId.invoker_deafening_blast, AbilityId.item_refresher)
                .SetItemsToHave(AbilityId.item_refresher).Build(),
        };
        DynamicComboSettings = new DynamicCombo(AbilityManager);
        // CataclysmInCombo = new CataclysmInCombo();
        CustomCombos = new List<StandardCombo?>();
        Combos.ForEach(combo =>
        {
            if (combo is StandardCombo customCombo)
            {
                CustomCombos.Add(customCombo);
            }
        });
    }

    private void InputManagerOnMouseMove(MouseMoveEventArgs e)
    {
        var mousePos = e.Position;
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth);

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth,
            maxHeight + CurrentMenu.SizeItem + ExtraWidth * 2);

        var isIn = rect.Contains(mousePos);

        e.Process = !isIn;
    }

    private void InputOnMouseClick(MouseEventArgs e)
    {
        var mousePos = e.Position;
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth);

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth,
            maxHeight + CurrentMenu.SizeItem + ExtraWidth * 2);

        var isIn = rect.Contains(mousePos);

        if (isIn)
        {
            var isLeft = e.MouseKey == MouseKey.Left;
            e.Process = false;
            foreach (var customCombo in CustomCombos)
            {
                if (customCombo.ActivateBtnPosition.Contains(mousePos))
                {
                    CustomCombos.ForEach(z => z.IsActive = false);
                    DynamicComboSettings.IsActive = false;
                    customCombo.IsActive = true;
                }

                var (key, value) = customCombo.Abilities.FirstOrDefault(z => z.Value.Position.Contains(mousePos));
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (value != null)
                {
                    CustomCombos.ForEach(z => z.ChangeIndex = -1);
                    if (isLeft)
                    {
                        customCombo.ChangeIndex = key;
                        // Console.WriteLine($"Получили клик по абилке {key}");
                    }
                    else
                    {
                        customCombo.Abilities[key].AbilityId = AbilityId.dota_base_ability;
                    }

                    return;
                }
            }

            if (CataclysmInCombo.ActivateBtnPosition.Contains(mousePos))
            {
                CataclysmInCombo.IsActive = !CataclysmInCombo.IsActive;
            }

            if (DynamicComboSettings.ActivateBtnPosition.Contains(mousePos))
            {
                // CustomCombos.ForEach(z => z.IsActive = false);
                // DynamicComboSettings.IsActive = true;
            }

            CustomCombos.ForEach(z => z.ChangeIndex = -1);
        }
    }

    public override ComboBuildType Type => ComboBuildType.StandardCombo;

    public override void Render()
    {
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth);
        var step = CurrentMenu.SizeItem + ExtraWidth;

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth, maxHeight);
        RendererManager.DrawFilledRoundedRectangle(rect, ContentBackgroundClr, new Vector2(RoundValue));
        RenderTitle(rect);
        var currentY = rect.Y;
        var maxY = rect.Y;
        for (var i = 0; i < ComboCount; i++)
        {
            var customCombo = CustomCombos[i];

            currentY += step;
            maxY = currentY + step + ExtraWidth / 2;
            var startPos = new Vector2(rect.X, currentY);
            RenderLine(startPos, customCombo);
        }

        var footerRect = new RectangleF(rect.X, maxY, maxWidth, CurrentMenu.SizeItem + ExtraWidth * 2);
        RenderFooter(footerRect, DynamicComboSettings, CataclysmInCombo);
    }

    public override ComboBase? GetCurrentCombo()
    {
        return CustomCombos.First(x => x.IsActive);
    }

    private void RenderLine(Vector2 startPos, ICombo? customCombo)
    {
        var size = CurrentMenu.SizeItem;
        var iconSize = size / 1.5f;
        var btnPos = new RectangleF(startPos.X + iconSize / 2f, startPos.Y + iconSize / 3, size / 1.5f, size / 1.5f);
        customCombo!.ActivateBtnPosition = btnPos;
        RenderCircle(btnPos, customCombo.IsActive ? CircleType.Red : CircleType.Gray);
        for (var i = 0; i < MaxAbilities; i++)
        {
            var rect = new RectangleF(startPos.X + size * (i + 1) + ExtraWidth * (i + 1), startPos.Y, size, size);
            if (customCombo.Abilities.TryGetValue(i, out var abilityStruct))
            {
                RenderAbility(rect, abilityStruct, customCombo.IsActive, i);
                RendererManager.DrawRectangle(rect, EmptyClr);
            }
        }
    }

    private void RenderAbility(RectangleF rect, CustomComboStruct comboStruct, bool customComboIsActive, int i)
    {
        var abilityId = comboStruct.AbilityId;
        var abilityId2 = comboStruct.AbilityId;
        if (comboStruct.Ability != null)
        {
            abilityId = comboStruct.Ability?.AbilityId ?? comboStruct.AbilityId;
            if (comboStruct.Ability is BaseItemAbility itemAbility)
            {
                abilityId2 = itemAbility.OwnerAbility;
            }
        }

        RendererManager.DrawImage(abilityId.ToString(), rect, ImageType.Ability, true);
        RenderAbilityState(rect, abilityId2);
        if (customComboIsActive && ComboInfo.ComboIndex == i && ComboInfo.IsInCombo)
        {
            RendererManager.DrawRectangle(rect, new Color(255, 0, 0, 255), 3);
        }
    }

    public override void Dispose()
    {
        InputManager.MouseKeyDown -= InputOnMouseClick;
        InputManager.MouseMove -= InputManagerOnMouseMove;
    }
}