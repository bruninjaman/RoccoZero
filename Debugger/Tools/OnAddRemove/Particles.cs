namespace Debugger.Tools.OnAddRemove;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Debugger.Menus;

using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.EventArgs;
using Divine.Particle.Particles;
using Divine.Update;

using Logger;

internal class Particles : IDebuggerTool
{
    private readonly HashSet<string> ignored = new HashSet<string>
    {
        "ui_mouseactions",
        "generic_hit_blood",
        "base_attacks",
        "generic_gameplay",
        "ensage_ui"
    };

    private MenuSwitcher addEnabled;

    private MenuSwitcher ignoreUseless;

    private MenuSwitcher ignoreZeroCp;

    private readonly ILog log;

    private IMainMenu mainMenu;

    private Menu menu;

    private MenuSwitcher showCpValues;

    public Particles(IMainMenu mainMenu, ILog log)
    {
        this.mainMenu = mainMenu;
        this.log = log;
    }

    public int LoadPriority { get; } = 98;

    public void Activate()
    {
        this.menu = this.mainMenu.OnAddRemoveMenu.CreateMenu("Particles");

        this.addEnabled = this.menu.CreateSwitcher("On add enabled", false);
        this.addEnabled.SetTooltip("Entity.OnParticleEffectAdded");
        this.addEnabled.ValueChanged += this.AddEnabledPropertyChanged;

        this.showCpValues = this.menu.CreateSwitcher("Show CP values", false);
        this.ignoreZeroCp = this.menu.CreateSwitcher("Ignore zero CP values", true);
        this.ignoreUseless = this.menu.CreateSwitcher("Ignore useless", true);

        this.AddEnabledPropertyChanged(null, null);
    }

    public void Dispose()
    {
        this.addEnabled.ValueChanged -= this.AddEnabledPropertyChanged;
        ParticleManager.ParticleAdded -= this.EntityOnParticleEffectAdded;
        //Entity.OnParticleEffectReleased -= this.EntityOnParticleEffectReleased;
    }

    private void AddEnabledPropertyChanged(MenuSwitcher switcher, SwitcherEventArgs e)
    {
        UpdateManager.BeginInvoke(() =>
        {
            if (this.addEnabled)
            {
                this.menu.AddAsterisk();
                ParticleManager.ParticleAdded += this.EntityOnParticleEffectAdded;
                //Entity.OnParticleEffectReleased += this.EntityOnParticleEffectReleased;
            }
            else
            {
                this.menu.RemoveAsterisk();
                ParticleManager.ParticleAdded -= this.EntityOnParticleEffectAdded;
                //Entity.OnParticleEffectReleased -= this.EntityOnParticleEffectReleased;
            }
        });
    }

    private async void EntityOnParticleEffectAdded(ParticleAddedEventArgs e)
    {
        if (e.IsCollection)
        {
            return;
        }

        var particle = e.Particle;

        if (!this.IsValid(particle))
        {
            return;
        }

        var item = new LogItem(LogType.Particle, Color.LightGreen, "Particle added");

        item.AddLine("Name: " + particle.Name, particle.Name);

        await Task.Delay(1);

        if (this.IsValid(particle))
        {
            item.AddLine("Highest control point: " + particle.HighestControlPoint, particle.HighestControlPoint);

            if (this.showCpValues)
            {
                for (var i = 0; i <= particle.HighestControlPoint; i++)
                {
                    var point = particle.GetControlPoint(i);
                    if (this.ignoreZeroCp && point == Vector3.Zero)
                    {
                        continue;
                    }

                    item.AddLine("CP " + i + ": " + point, point);
                }
            }
        }

        this.log.Display(item);
    }

    /*private void EntityOnParticleEffectReleased(Entity sender, ParticleEffectReleasedEventArgs args)
    {
        var particle = args.ParticleEffect;

        if (!this.IsValid(sender, particle, particle.Name))
        {
            return;
        }

        var item = new LogItem(LogType.Particle, Color.LightGreen, "Particle added/released");

        item.AddLine("Name: " + particle.Name, particle.Name);
        item.AddLine("Highest control point: " + particle.HighestControlPoint, particle.HighestControlPoint);

        if (this.showCpValues)
        {
            for (var i = 0u; i <= args.ParticleEffect.HighestControlPoint; i++)
            {
                var point = args.ParticleEffect.GetControlPoint(i);
                if (this.ignoreZeroCp && point.IsZero)
                {
                    continue;
                }

                item.AddLine("CP " + i + ": " + point, point);
            }
        }

        this.log.Display(item);
    }*/

    private bool IsValid(Particle particle)
    {
        /*if (sender?.IsValid != true)
        {
            return false;
        }*/

        if (particle?.IsValid != true)
        {
            return false;
        }

        if (this.ignoreUseless && this.ignored.Any(particle.Name.Contains))
        {
            return false;
        }

        return true;
    }
}