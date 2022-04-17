using Divine.Entity.Entities.Units.Heroes;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Update;
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

    public ComboExecutorFeature(MenuConfig menuConfig, ComboConstructorFeature comboConstructorFeature, IAbilityExecutor abilityExecutor, IComboInfo comboInfo) : base(menuConfig.ComboExecutorMenu.Enable)
    {
        _comboConstructorFeature = comboConstructorFeature;
        _abilityExecutor = abilityExecutor;
        _comboInfo = comboInfo;
        CurrentMenu = menuConfig.ComboExecutorMenu;
        UpdateThread = UpdateManager.CreateIngameUpdate(10, false, Updater);
        CurrentMenu.HoldKey.ValueChanged += HoldKeyOnValueChanged;
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

    public StandardCombo? CurrentCombo { get; set; } = null;

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
            if (!_abilityExecutor.CastAbility(abilityToCast, Target))
            {
                IncreaseComboIndex();
            }
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