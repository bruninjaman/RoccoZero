namespace O9K.Hud.Modules.Particles.Abilities;

using System;
using System.Collections.Generic;

using Core.Entities.Metadata;
using Core.Entities.Units;
using Core.Logger;
using Core.Managers.Entity;
using Divine.Particle;
using Divine.Update;
using Divine.Particle.Components;
using Divine.Particle.Particles;
using Divine.Entity.Entities.Abilities.Components;

using Helpers.Notificator;

using MainMenu;

[AbilityId(AbilityId.roshan_spell_block)]
internal class RoshanSpellBlock : AbilityModule
{
    private readonly Dictionary<uint, Particle> effects = new Dictionary<uint, Particle>();

    private readonly List<Unit9> units = new List<Unit9>();

    public RoshanSpellBlock(INotificator notificator, IHudMenu hudMenu)
        : base(notificator, hudMenu)
    {
    }

    protected override void Disable()
    {
        EntityManager9.UnitAdded -= this.OnUnitAdded;
        EntityManager9.UnitRemoved -= this.OnUnitRemoved;
        UpdateManager.DestroyIngameUpdate(this.OnUpdate);

        foreach (var effect in this.effects)
        {
            effect.Value.Dispose();
        }

        this.effects.Clear();
        this.units.Clear();
    }

    protected override void Enable()
    {
        EntityManager9.UnitAdded += this.OnUnitAdded;
        EntityManager9.UnitRemoved += this.OnUnitRemoved;
        UpdateManager.CreateIngameUpdate(500, this.OnUpdate);
    }

    private void OnUnitAdded(Unit9 hero)
    {
        try
        {
            if (hero is not Core.Entities.Units.Unique.Roshan)
            {
                 return;
            }

            this.units.Add(hero);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUnitRemoved(Unit9 hero)
    {
        try
        {
            if (hero is not Core.Entities.Units.Unique.Roshan)
            {
                return;
            }

            this.units.Remove(hero);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    private void OnUpdate()
    {
        try
        {
            foreach (var unit in this.units)
            {
                if (!unit.IsValid)
                {
                    continue;
                }

                bool roshanIsblockingSpells = false;

                foreach (var ability in unit.Abilities)
                {
                    if (ability.Id == AbilityId.roshan_spell_block)
                        if (ability.IsReady)
                            roshanIsblockingSpells = true;
                }
                if (roshanIsblockingSpells && unit.IsVisible && unit.IsAlive)
                {
                    if (this.effects.ContainsKey(unit.Handle))
                    {
                        continue;
                    }

                    this.effects[unit.Handle] = ParticleManager.CreateParticle(
                        "particles/items_fx/immunity_sphere_buff.vpcf",
                        Attachment.CenterFollow,
                        unit.BaseUnit);
                }
                else
                {
                    if (!this.effects.TryGetValue(unit.Handle, out var effect))
                    {
                        continue;
                    }

                    effect.Dispose();
                    this.effects.Remove(unit.Handle);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }
}