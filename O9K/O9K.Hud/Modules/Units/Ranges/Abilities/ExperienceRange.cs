﻿namespace O9K.Hud.Modules.Units.Ranges.Abilities;

using Core.Data;
using Core.Entities.Units;
using Core.Managers.Menu;
using Core.Managers.Menu.EventArgs;
using Core.Managers.Menu.Items;

using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Particle.Particles;
using Divine.Update;

internal class ExperienceRange : IRange
{
    private readonly Unit9 unit;

    private MenuSlider blue;

    private MenuSlider green;

    private Particle particleEffect;

    private MenuSlider red;

    public ExperienceRange(Unit9 unit)
    {
        this.unit = unit;
        this.Handle = uint.MaxValue;
        this.Name = "o9k.exp_plus";
    }

    public float DrawRange { get; } = GameData.ExperienceRange * 1.1f;

    public uint Handle { get; }

    public bool IsEnabled { get; private set; }

    public string Name { get; }

    public void Dispose()
    {
        this.particleEffect?.Dispose();
        this.particleEffect = null;
        this.IsEnabled = false;
    }

    public void Enable(Menu heroMenu)
    {
        var abilityMenu = heroMenu.GetOrAdd(new Menu("Experience", "exp")).SetTexture("o9k.exp_plus");
        abilityMenu.AddTranslation(Lang.Ru, "Опыт");
        abilityMenu.AddTranslation(Lang.Cn, "经验");

        this.red = abilityMenu.GetOrAdd(new MenuSlider("Red", 255, 0, 255));
        this.red.AddTranslation(Lang.Ru, "Красный");
        this.red.AddTranslation(Lang.Cn, "红");

        this.green = abilityMenu.GetOrAdd(new MenuSlider("Green", 0, 0, 255));
        this.green.AddTranslation(Lang.Ru, "Зеленый");
        this.green.AddTranslation(Lang.Cn, "绿色");

        this.blue = abilityMenu.GetOrAdd(new MenuSlider("Blue", 0, 0, 255));
        this.blue.AddTranslation(Lang.Ru, "Синий");
        this.blue.AddTranslation(Lang.Cn, "蓝色");

        this.red.ValueChange += this.ColorOnValueChange;
        this.green.ValueChange += this.ColorOnValueChange;
        this.blue.ValueChange += this.ColorOnValueChange;

        this.IsEnabled = true;

        this.DelayedRedraw();
    }

    public void UpdateRange()
    {
    }

    private void ColorOnValueChange(object sender, SliderEventArgs e)
    {
        if (e.NewValue == e.OldValue)
        {
            return;
        }

        this.DelayedRedraw();
    }

    private void DelayedRedraw()
    {
        UpdateManager.BeginInvoke(this.RedrawRange);
    }

    private void RedrawRange()
    {
        if (this.particleEffect == null)
        {
            this.particleEffect = ParticleManager.CreateParticle(
                @"particles\ui_mouseactions\drag_selected_ring.vpcf",
                Attachment.AbsOriginFollow,
                this.unit.BaseUnit);
        }

        this.particleEffect.SetControlPoint(1, new Vector3(this.red, this.green, this.blue));
        this.particleEffect.SetControlPoint(2, new Vector3(-this.DrawRange, 255, 0));
        this.particleEffect.FullRestart();
    }
}