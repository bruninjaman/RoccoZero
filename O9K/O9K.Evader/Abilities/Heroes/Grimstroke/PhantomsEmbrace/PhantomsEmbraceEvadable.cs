namespace O9K.Evader.Abilities.Heroes.Grimstroke.PhantomsEmbrace;

using Base;
using Base.Evadable;
using Base.Evadable.Components;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Entity.Entities.Units;
using Divine.Modifier.Modifiers;

using Metadata;

using O9K.Evader.Pathfinder.Obstacles.Abilities.Proactive;

using Pathfinder.Obstacles.Modifiers;

internal sealed class PhantomsEmbraceEvadable : EvadableAbility, IModifierCounter, IModifierObstacle, IProactiveCounter
{
    public PhantomsEmbraceEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
        : base(ability, pathfinder, menu)
    {
        this.Counters.Add(Abilities.MantaStyle);
        this.Counters.Add(Abilities.BallLightning);
        this.Counters.Add(Abilities.AttributeShift);
        this.Counters.UnionWith(Abilities.Invisibility);
        this.Counters.UnionWith(Abilities.StrongShield);
        this.Counters.UnionWith(Abilities.MagicShield);

        this.Counters.Remove(Abilities.BlackKingBar);
        this.Counters.Remove(Abilities.Bristleback);
        this.Counters.Remove(Abilities.Enrage);

        this.ModifierCounters.Add(Abilities.MantaStyle);
        this.ModifierCounters.Add(Abilities.EulsScepterOfDivinity);
        this.ModifierCounters.Add(Abilities.WindWaker);
        this.ModifierCounters.Add(Abilities.Stormcrafter);
        this.ModifierCounters.Add(Abilities.HeavenlyGrace);
        this.ModifierCounters.Add(Abilities.AphoticShield);

        this.ProactiveCounters.Add(Abilities.Counterspell);
    }

    public bool AllyModifierObstacle { get; } = true;

    public bool ModifierAllyCounter { get; } = true;

    public bool ModifierEnemyCounter { get; } = false;

    public void AddModifier(Modifier modifier, Unit9 modifierOwner)
    {
        var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
        this.Pathfinder.AddObstacle(obstacle);
    }

    public void AddModifierObstacle(Modifier modifier, Unit sender)
    {
        if (modifier.TextureName != "grimstroke_ink_creature")
        {
            return;
        }

        var obstacle = new PhantomsEmbraceObstacle(this, modifier, sender);
        this.Pathfinder.AddObstacle(obstacle);
    }

    protected override void AddObstacle()
    {
    }

    public void AddProactiveObstacle()
    {
        var obstacle = new ProactiveAbilityObstacle(this);
        this.Pathfinder.AddObstacle(obstacle);
    }
}