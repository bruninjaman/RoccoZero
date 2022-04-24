using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Renderer;
using InvokerAnnihilation.Config;
using InvokerAnnihilation.Feature.ComboConstructor.Interface;
using InvokerAnnihilation.Feature.ComboConstructor.Panels;
using InvokerAnnihilation.Feature.FeatureBase;

namespace InvokerAnnihilation.Feature.ComboConstructor;

public sealed class ComboConstructorFeature : FeatureBase<ComboConstructorMenu>
{
    public ComboConstructorFeature(MenuConfig menuConfig, IEnumerable<IComboBuilder> builders) : base(menuConfig.ComboConstructorMenu.Enable)
    {
        CurrentMenu = menuConfig.ComboConstructorMenu;
        var comboBuilders = builders as IComboBuilder[] ?? builders.ToArray();
        CustomComboBuilder = comboBuilders.First(z => z is CustomComboBuilder);
        StandardComboBuilder = comboBuilders.First(z => z is StandardComboBuilder);
        CurrentBuilder = StandardComboBuilder;
        CurrentBuilder.Start();
        CurrentMenu.UseCustomBuilder.ValueChanged += UseCustomBuilderOnValueChanged;
    }

    private void UseCustomBuilderOnValueChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        CurrentBuilder.Stop();
        CurrentBuilder = e.Value ? CustomComboBuilder : StandardComboBuilder;
        CurrentBuilder.Start();
    }

    public override ComboConstructorMenu CurrentMenu { get; set; }
    private IComboBuilder CustomComboBuilder { get; }
    private IComboBuilder StandardComboBuilder { get; }
    public IComboBuilder CurrentBuilder { get; set; }

    public override void Enable()
    {
        RendererManager.Draw += RendererManagerOnDraw;
    }

    private void RendererManagerOnDraw()
    {
        CurrentBuilder.Render();
    }

    public override void Disable()
    {
        RendererManager.Draw -= RendererManagerOnDraw;
    }
}