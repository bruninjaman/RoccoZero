namespace VisibleByEnemyPlus;

using System.Collections.Generic;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.EventArgs;
using Divine.Extensions;
using Divine.Numerics;
using Divine.Particle;
using Divine.Particle.Components;
using Divine.Service;
using Divine.Update;

public class VisibleByEnemyPlus : Bootstrapper
{
    private readonly HashSet<Unit> Units = new();

    private Config Config { get; set; }

    private bool AddEffectType { get; set; }

    private int Red => Config.RedItem;

    private int Green => Config.GreenItem;

    private int Blue => Config.BlueItem;

    private int Alpha => Config.AlphaItem;

    private Hero Owner;

    protected override void OnActivate()
    {
        Config = new Config();

        Config.EffectTypeItem.ValueChanged += (selector, e) => { UpdateMenu(e.NewValue, Red, Green, Blue, Alpha); };
        Config.RedItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, e.NewValue, Green, Blue, Alpha); };
        Config.GreenItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, e.NewValue, Blue, Alpha); };
        Config.BlueItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, Green, e.NewValue, Alpha); };
        Config.AlphaItem.ValueChanged += (slider, e) => { UpdateMenu(Config.EffectTypeItem, Red, Green, Blue, e.NewValue); };

        Owner = EntityManager.LocalHero;

        EntityManager.EntityAdded += OnEntityAdded;
        EntityManager.EntityRemoved += OnEntityRemoved;

        UpdateManager.CreateIngameUpdate(50, OnIngameUpdate);
    }

    private void OnEntityAdded(EntityAddedEventArgs e)
    {
        var entity = e.Entity;
        if (entity is not Unit unit)
        {
            return;
        }

        if (unit is not Hero and not Courier || !unit.IsAlly(Owner))
        {
            return;
        }

        Units.Add(unit);
    }

    private void OnEntityRemoved(EntityRemovedEventArgs e)
    {
        var entity = e.Entity;
        if (entity is not Unit unit)
        {
            return;
        }

        if (unit is not Hero and not Courier)
        {
            return;
        }

        Units.Remove(unit);
    }

    protected override void OnDeactivate()
    {
        /*UpdateManager.Unsubscribe(LoopEntities);

        Config.EffectTypeItem.PropertyChanged -= ItemChanged;

        Config.RedItem.PropertyChanged -= ItemChanged;
        Config.GreenItem.PropertyChanged -= ItemChanged;
        Config.BlueItem.PropertyChanged -= ItemChanged;
        Config.AlphaItem.PropertyChanged -= ItemChanged;

        Config?.Dispose();
        ParticleManager.Dispose();*/
    }

    private void UpdateMenu(string selector, int red, int green, int blue, int alpha)
    {
        if (selector == "Default")
        {
            Config.RedItem.SetFontColor(Color.Black);
            Config.GreenItem.SetFontColor(Color.Black);
            Config.BlueItem.SetFontColor(Color.Black);
            Config.AlphaItem.SetFontColor(Color.Black);
        }
        else
        {
            Config.RedItem.SetFontColor(new Color(red, 0, 0, 255));
            Config.GreenItem.SetFontColor(new Color(0, green, 0, 255));
            Config.BlueItem.SetFontColor(new Color(0, 0, blue, 255));
            Config.AlphaItem.SetFontColor(new Color(185, 176, 163, alpha));
        }

        var localHero = EntityManager.LocalHero;
        if (localHero != null && localHero.IsValid)
        {
            HandleEffect(localHero, true);
            AddEffectType = false;
        }
    }

    private void OnIngameUpdate()
    {
        foreach (var unit in Units)
        {
            HandleEffect(unit, unit.IsVisibleToEnemies);
        }
    }

    private void HandleEffect(Unit unit, bool visible)
    {
        if (!AddEffectType /*&& Owner.Animation.Name != "idle"*/)
        {
            AddEffectType = true;
        }

        if (visible && unit.IsAlive /*&& unit.Position.IsOnScreen()*/)
        {
            ParticleManager.CreateParticle(
                $"VisibleByEnemyPlus.{unit.Handle}",
                Config.Effects[Config.EffectTypeItem],
                Attachment.AbsOriginFollow,
                unit,
                new ControlPoint(1, Red, Green, Blue),
                new ControlPoint(2, Alpha));
        }
        else if (AddEffectType)
        {
            ParticleManager.DestroyParticle($"VisibleByEnemyPlus.{unit.Handle}");
        }
    }
}