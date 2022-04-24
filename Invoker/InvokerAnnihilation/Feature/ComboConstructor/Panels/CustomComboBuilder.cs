using Divine.Entity.Entities.Abilities.Components;
using Divine.Input;
using Divine.Input.EventArgs;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;
using InvokerAnnihilation.Abilities.AbilityManager;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor.Combos;
using InvokerAnnihilation.Feature.ComboConstructor.Combos.Dto;
using InvokerAnnihilation.Feature.ComboConstructor.Emum;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;
using InvokerAnnihilation.Feature.ComboConstructor.Panels.Base;
using InvokerAnnihilation.Feature.ComboExecutor;

namespace InvokerAnnihilation.Feature.ComboConstructor.Panels;

public class CustomComboBuilder : BaseComboBuilder
{
    private readonly Dictionary<AbilityId, RectangleF> _abilitiesToSelect = new List<AbilityId>
    {
        AbilityId.invoker_cold_snap,
        AbilityId.invoker_ghost_walk,
        AbilityId.invoker_ice_wall,
        AbilityId.invoker_emp,
        AbilityId.invoker_tornado,
        AbilityId.invoker_alacrity,
        AbilityId.invoker_deafening_blast,
        AbilityId.invoker_sun_strike,
        AbilityId.invoker_forge_spirit,
        AbilityId.invoker_chaos_meteor,
        AbilityId.item_cyclone,
        AbilityId.item_refresher,
        AbilityId.item_blink,
        AbilityId.item_sheepstick,
        AbilityId.item_orchid
    }.ToDictionary(z => z, z => new RectangleF());

    public CustomComboBuilder(MenuConfig menuConfig, IAbilityManager abilityManager, IComboInfo comboInfo) : base(menuConfig.ComboConstructorMenu, "Custom Combo", abilityManager, comboInfo)
    {
        Combos = new List<ICombo>();
        DynamicComboSettings = new DynamicCombo(AbilityManager);
        // CataclysmInCombo = new CataclysmInCombo();
        Combos.Add(DynamicComboSettings);
        CustomCombos = new List<CustomCombo>();

        // var isIn = size.Contains(e.Position);


        CurrentMenu.ComboCount.ValueChanged += ComboCountOnValueChanged;
        CurrentMenu.MaxAbilitiesPerCombo.ValueChanged += MaxAbilitiesPerComboOnValueChanged;
    }



    private void MaxAbilitiesPerComboOnValueChanged(MenuSlider slider, SliderEventArgs e)
    {
        var dif = e.NewValue - e.OldValue;
        switch (dif)
        {
            case > 0:
            {
                Console.WriteLine("Inc");
                for (var i = 0; i < dif; i++)
                {
                    CustomCombos.ForEach(x =>
                    {
                        x.Abilities.Add(x.Abilities.Count, new CustomComboStruct(AbilityId.dota_base_ability));
                    });
                }

                break;
            }
            case < 0:
            {
                Console.WriteLine("Dec");
                for (var i = 0; i < Math.Abs(dif); i++)
                {
                    CustomCombos.ForEach(x =>
                    {
                        x.Abilities.Remove(x.Abilities.Count - 1);
                    });
                }

                break;
            }
            default:
            {
                break;
            }
        }

    }

    private void ComboCountOnValueChanged(MenuSlider slider, SliderEventArgs e)
    {
        var dif = e.NewValue - e.OldValue;
        switch (dif)
        {
            case > 0:
            {
                for (var i = 0; i < dif; i++)
                {
                    Combos.Add(new CustomCombo(AbilityManager, MaxAbilities));
                }

                break;
            }
            case < 0:
            {
                for (var i = 0; i < Math.Abs(dif); i++)
                {
                    Combos.RemoveAt(Combos.Count - 1);
                }

                break;
            }
            default:
            {
                for (var i = 0; i < e.NewValue; i++)
                {
                    Combos.Add(new CustomCombo(AbilityManager, MaxAbilities));
                }

                break;
            }
        }

        CustomCombos = new List<CustomCombo>();
        Combos.ForEach(combo =>
        {
            if (combo is CustomCombo customCombo)
            {
                CustomCombos.Add(customCombo);
            }
        });
        CustomCombos.ForEach(x => x.IsActive = false);
        CustomCombos.First().IsActive = true;
    }

    public int ComboCount => CustomCombos.Count;
    public int MaxAbilities => CurrentMenu.MaxAbilitiesPerCombo.Value;
    public int ExtraInAbilitySelector { get; } = 5;
    private Color AbilitySelectorBackgroundClr { get; } = new(196, 196, 196, 255 / 100 * 25);
    private bool AnyComboWithAbilitySelector => CustomCombos.Any(z => z.ChangeIndex >= 0);
    public List<ICombo> Combos { get; set; }
    public List<CustomCombo> CustomCombos { get; set; } // (List<CustomCombo>) Combos.Where(x => x is CustomCombo);

    private void InputOnMouseMove(MouseMoveEventArgs e)
    {
        var mousePos = e.Position;
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth) + CurrentMenu.SizeItem + ExtraWidth * 2;
        var step = CurrentMenu.SizeItem + ExtraWidth;
        if (AnyComboWithAbilitySelector) maxHeight += step * 1.5f;

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth - ExtraWidth / 2, maxHeight);

        var isIn = rect.Contains(mousePos);

        if (isIn)
        {
            e.Process = false;
        }
    }

    private void InputOnMouseClick(MouseEventArgs e)
    {
        var mousePos = e.Position;
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth) + CurrentMenu.SizeItem + ExtraWidth * 2;
        var step = CurrentMenu.SizeItem + ExtraWidth;
        if (AnyComboWithAbilitySelector) maxHeight += step * 1.5f;

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth - ExtraWidth / 2, maxHeight);

        var isIn = rect.Contains(mousePos);

        if (isIn)
        {
            e.Process = false;
            // var isInAbility =
            // CustomCombos.FirstOrDefault(x => x.Abilities.Values.Any(z => z.Position.Contains(mousePos)));
            var isLeft = e.MouseKey == MouseKey.Left;
            foreach (var customCombo in CustomCombos)
            {
                if (customCombo.ChangeIndex >= 0 && isLeft)
                {
                    var (abilityToSelectKey, rectangleF) =
                        _abilitiesToSelect.FirstOrDefault(z => z.Value.Contains(mousePos));
                    // var blocked = customCombo.Abilities.Any(z => z.Value.AbilityId == abilityToSelectKey);
                    if (abilityToSelectKey != AbilityId.dota_base_ability)
                    {
                        // if (blocked) return;
                        // customCombo.ChangeIndex = key;
                        customCombo.Abilities[customCombo.ChangeIndex].AbilityId = abilityToSelectKey;
                        customCombo.Abilities[customCombo.ChangeIndex].SetAbility(AbilityManager.GetAbility(abilityToSelectKey));
                        customCombo.ChangeIndex++;
                        if (MaxAbilities <= customCombo.ChangeIndex)
                            customCombo.ChangeIndex = 0;
                        
                        return;
                    }
                }

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
                        customCombo.Abilities[key].SetAbility(null);
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

    public override ComboBuildType Type => ComboBuildType.CustomCombo;

    public override void Render()
    {
        var maxWidth = (MaxAbilities + 1) * (ExtraWidth + CurrentMenu.SizeItem);
        var maxHeight = (ComboCount + 1) * (CurrentMenu.SizeItem + ExtraWidth);
        var step = CurrentMenu.SizeItem + ExtraWidth;
        if (AnyComboWithAbilitySelector) maxHeight += step * 1.5f;

        var rect = new RectangleF(CurrentMenu.PositionXItem, CurrentMenu.PositionYItem, maxWidth/* - ExtraWidth / 2*/, maxHeight);
        if (CustomCombos.Any(x=>x.ChangeIndex >= 0))
        {
            rect.Height += (rect.Width / 8 - ExtraInAbilitySelector * 1.5f) * 2 + ExtraInAbilitySelector;
        }
        RendererManager.DrawFilledRoundedRectangle(rect, ContentBackgroundClr, new Vector2(RoundValue));
        RenderTitle(rect);
        var currentY = rect.Y;
        var maxY = rect.Y;
        for (var i = 0; i < ComboCount; i++)
        {
            var customCombo = CustomCombos[i];

            currentY += step;
            var startPos = new Vector2(rect.X, currentY);
            RenderLine(startPos, customCombo);
            maxY = currentY + step + ExtraWidth / 2;
            if (customCombo.ChangeIndex >= 0)
            {
                var size = CurrentMenu.SizeItem;
                var extra = size / 1.5f / 2;
                var abilitySelectorRect = new RectangleF(startPos.X + extra, startPos.Y + step - 15,
                    maxWidth - extra - ExtraWidth + 1,
                    (rect.Width / 8 - ExtraInAbilitySelector * 1.5f) * 2 + ExtraInAbilitySelector);
                currentY += abilitySelectorRect.Height;
                maxY = currentY + abilitySelectorRect.Height - ExtraWidth;
                var size2 = abilitySelectorRect.Width / 8 - ExtraInAbilitySelector * 1.5f;

                RenderAbilitySelector(customCombo, abilitySelectorRect, size2);
            }
        }

        var footerRect = new RectangleF(rect.X, maxY, maxWidth, CurrentMenu.SizeItem + ExtraWidth * 2);
        RenderFooter(footerRect, DynamicComboSettings, CataclysmInCombo);
    }

    public override ComboBase? GetCurrentCombo()
    {
        return CustomCombos.First(x => x.IsActive);
    }

    public override DynamicCombo DynamicComboSettings { get; set; }
    public override CataclysmInCombo CataclysmInCombo { get; set; } = new CataclysmInCombo();
    public override void Start()
    {
        InputManager.MouseKeyDown += InputOnMouseClick;
        InputManager.MouseMove += InputOnMouseMove;
    }

    public override void Stop()
    {
        InputManager.MouseKeyDown -= InputOnMouseClick;
        InputManager.MouseMove -= InputOnMouseMove;
    }

    public override void Dispose()
    {

    }

    private void RenderAbilityImage(AbilityId key, ref RectangleF abilityRect, float iconSize, bool blocked)
    {
        RendererManager.DrawImage(key.ToString(), abilityRect, ImageType.Ability, true);
        RendererManager.DrawRectangle(abilityRect, EmptyClr);
        // if (blocked)
            // RendererManager.DrawFilledRectangle(abilityRect, new Color(0, 0, 0, 255 / 100 * 70));
        _abilitiesToSelect[key] = abilityRect;
        // abilityRect.X += iconSize * 1.5f;
    }

    private void RenderAbilitySelector(CustomCombo customCombo, RectangleF rect, float size)
    {
        var first = _abilitiesToSelect.Take(8);
        var last = _abilitiesToSelect.Skip(8);
        RendererManager.DrawFilledRectangle(rect, AbilitySelectorBackgroundClr);

        // var iconSize = size / 1.3f;
        var abilityRect = new RectangleF(rect.X + ExtraInAbilitySelector, rect.Y + ExtraInAbilitySelector, size, size);
        foreach (var (key, _) in first)
        {
            var blocked = customCombo.Abilities.Any(z => z.Value.AbilityId == key);
            RenderAbilityImage(key, ref abilityRect, size, blocked);
            abilityRect.X += size + ExtraInAbilitySelector;
        }

        abilityRect = new RectangleF(rect.X + ExtraInAbilitySelector, rect.Y + ExtraInAbilitySelector * 2 + size, size,
            size);
        foreach (var (key, _) in last)
        {
            var blocked = customCombo.Abilities.Any(z => z.Value.AbilityId == key);
            RenderAbilityImage(key, ref abilityRect, size, blocked);
            abilityRect.X += size + ExtraInAbilitySelector;
        }
    }

    private void RenderEmptyAbilitySlot(RectangleF rect, bool isActiveChange)
    {
        var measureText = RendererManager.MeasureText("+", 30);
        RendererManager.DrawText("+",
            new Vector2(rect.Center.X - measureText.X / 2, rect.Center.Y - measureText.Y / 2 - 2),
            isActiveChange ? ChangeClr : EmptyClr, 30);
    }

    private void RenderAbility(RectangleF rect, CustomComboStruct comboStruct, bool customComboIsActive, int index)
    {
        var abilityId = comboStruct.AbilityId;
        var abilityId2= comboStruct.AbilityId;
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
        if (customComboIsActive && ComboInfo.ComboIndex == index && ComboInfo.IsInCombo)
        {
            RendererManager.DrawRectangle(rect, new Color(255, 0, 0, 255), 3);
        }
    }
    

    private void RenderAbilitySlot(RectangleF rect, ICombo customCombo, int index)
    {
        var isActiveChange = customCombo.ChangeIndex == index;
        if (customCombo.Abilities.TryGetValue(index, out var abilityStruct))
        {
            abilityStruct.Position = rect;
            if (abilityStruct.AbilityId == AbilityId.dota_base_ability)
                RenderEmptyAbilitySlot(rect, isActiveChange);
            else
                RenderAbility(rect, abilityStruct, customCombo.IsActive, index);
        }
        else
        {
            RenderEmptyAbilitySlot(rect, isActiveChange);
        }

        RendererManager.DrawRectangle(rect, isActiveChange ? ChangeClr : EmptyClr);
    }

    private void RenderLine(Vector2 startPos, ICombo customCombo)
    {
        var size = CurrentMenu.SizeItem;
        var iconSize = size / 1.5f;
        var btnPos = new RectangleF(startPos.X + iconSize / 2f, startPos.Y + iconSize / 3, size / 1.5f, size / 1.5f);
        customCombo.ActivateBtnPosition = btnPos;
        RenderCircle(btnPos, customCombo.IsActive ? CircleType.Red : CircleType.Gray);
        for (var i = 0; i < MaxAbilities; i++)
        {
            var rect = new RectangleF(startPos.X + size * (i + 1) + ExtraWidth * (i + 1), startPos.Y, size, size);
            RenderAbilitySlot(rect, customCombo, i);
        }
    }
}