using System.Runtime.Serialization;
using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;
using InvokerAnnihilation.Abilities.Interfaces;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor;
using InvokerAnnihilation.Feature.ComboConstructor.Combos;
using InvokerAnnihilation.Feature.FeatureBase;

namespace InvokerAnnihilation.Feature.ComboExecutor;

public sealed class ComboExecutorFeature : FeatureBase<ComboExecutorMenu>
{
    private readonly ComboConstructorFeature _comboConstructorFeature;
    private readonly IAbilityExecutor _abilityExecutor;
    private readonly IComboInfo _comboInfo;
    public Hero? Target { get; set; } = null;

    public ComboExecutorFeature(MenuConfig menuConfig, ComboConstructorFeature comboConstructorFeature,
        IAbilityExecutor abilityExecutor, IComboInfo comboInfo) : base(menuConfig.ComboExecutorMenu.Enable)
    {
        _comboConstructorFeature = comboConstructorFeature;
        _abilityExecutor = abilityExecutor;
        _comboInfo = comboInfo;
        CurrentMenu = menuConfig.ComboExecutorMenu;
        UpdateThread = UpdateManager.CreateIngameUpdate(10, false, Updater);
        PrepareAbilitiesThread = UpdateManager.CreateIngameUpdate(150, false, PrepareAbilities);
        CurrentMenu.HoldKey.ValueChanged += HoldKeyOnValueChanged;
        CurrentMenu.PrepareKey.ValueChanged += PrepareKeyClick;
    }

    public UpdateHandler? PrepareAbilitiesThread { get; set; }

    private void PrepareAbilities()
    {
        if (_comboInfo.IsInCombo)
        {
            return;
        }

        var comboToPrepare = _comboConstructorFeature.CurrentBuilder.GetCurrentCombo();

        var abilities = comboToPrepare.ValidAbilities
            .Where(x => x.Value.Ability is IInvokableAbility).Take(2);
        var first = abilities.First().Value.Ability as BaseInvokableAbstractAbility;
        var last = abilities.Last().Value.Ability as BaseInvokableAbstractAbility;

        if ((first == null || first.IsInvoked) && (last == null || last.IsInvoked))
            return;

        if (first != null)
        {
            if (!first.IsInvoked && first.CanBeInvoked())
            {
                if (last != null)
                {
                    if (last.BaseAbility.AbilitySlot == AbilitySlot.Slot5)
                    {
                        last.Invoke();
                        return;
                    }
                }

                first.Invoke();
            }
        }

        if (last != null)
        {
            if (first != null)
            {
                if (first.BaseAbility.AbilitySlot == AbilitySlot.Slot5)
                {
                    first.Invoke();
                    return;
                }
            }

            last.Invoke();
        }
    }

    private void PrepareKeyClick(MenuHoldKey holdkey, HoldKeyEventArgs e)
    {
        if (PrepareAbilitiesThread != null)
        {
            PrepareAbilitiesThread.IsEnabled = e.Value;
        }
    }

    private void HoldKeyOnValueChanged(MenuHoldKey holdkey, HoldKeyEventArgs e)
    {
        if (UpdateThread != null)
        {
            _comboInfo.ResetComboIndex();
            UpdateThread.IsEnabled = e.Value;
            _comboInfo.IsInCombo = e.Value;
            if (!e.Value)
            {
                Target = null;
            }
            else
            {
                try
                {
                    CurrentCombo = _comboConstructorFeature.CurrentBuilder.GetCurrentCombo();
                }
                catch (Exception exception)
                {
                    _comboInfo.ResetComboIndex();
                    UpdateThread.IsEnabled = false;
                    throw;
                }
            }
        }
    }

    public ComboBase? CurrentCombo { get; set; } = null;

    private UpdateHandler? UpdateThread { get; }

    public override void Enable()
    {
    }


    private void Updater()
    {
        // Console.WriteLine("combo executing");
        if (Target == null)
        {
            Target = TargetSelector.GetTarget();
            if (Target == null)
                return;
        }

        if (!Target.IsVisible)
        {
            return;
        }

        var abilityToCast = CurrentCombo.Abilities[_comboInfo.ComboIndex].Ability;
        if (abilityToCast != null)
        {
            if (!_abilityExecutor.CastAbility(abilityToCast, Target) && abilityToCast.OnCooldown)
            {
                // Console.WriteLine($"[{abilityToCast.AbilityId}] next ability: {abilityToCast.OnCooldown}");
                IncreaseComboIndex();
            }
        }

        if (CurrentCombo.Abilities.All(x =>x.Value.Ability is null or {OnCooldown: true}))
        {
            if (MultiSleeper<string>.Sleeping("AttackCooldown"))
            {
                return;
            }

            MultiSleeper<string>.Sleep("AttackCooldown", 350);
            var me = EntityManager.LocalHero!;
            me.Attack(Target);
        }
    }

    public override void Disable()
    {
        if (UpdateThread != null)
        {
            UpdateThread.IsEnabled = false;
            CurrentMenu.HoldKey.ValueChanged -= HoldKeyOnValueChanged;
        }
    }

    private void IncreaseComboIndex()
    {
        _comboInfo.IncreaseComboIndex();
        if (_comboInfo.ComboIndex > CurrentCombo.ValidAbilities.Count - 1)
        {
            _comboInfo.ResetComboIndex();
        }
    }

    public override ComboExecutorMenu CurrentMenu { get; set; }
}