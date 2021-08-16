namespace O9K.Evader.Abilities.Heroes.Silencer.GlobalSilence
{
    using Base;
    using Base.Evadable;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;

    using Divine.Modifier.Modifiers;

    using Metadata;

    using Pathfinder.Obstacles.Modifiers;

    internal sealed class GlobalSilenceEvadable : GlobalEvadable, IModifierCounter
    {
        public GlobalSilenceEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
            : base(ability, pathfinder, menu)
        {
            this.Blinks.UnionWith(Abilities.Blink);
            this.Blinks.Remove(Abilities.Flicker);


            this.Disables.UnionWith(Abilities.Disable);

            this.Counters.Add(Abilities.AttributeShift);
            this.Counters.Add(Abilities.FortunesEnd);
            this.Counters.UnionWith(Abilities.StrongShield);
            this.Counters.UnionWith(Abilities.Invisibility);

            this.Counters.Remove(Abilities.Bristleback);
            this.Counters.Remove(Abilities.Enrage);
            this.Counters.Remove(Abilities.EulsScepterOfDivinity);
            this.Counters.Remove(Abilities.WindWaker);
            this.Counters.Remove(Abilities.Stormcrafter);
            this.Counters.Remove(Abilities.FatesEdict);
            this.Counters.Remove(Abilities.BlackKingBar);
            this.Counters.Remove(Abilities.MinotaurHorn);
            this.Counters.Remove(Abilities.Flicker);

            this.ModifierCounters.UnionWith(Abilities.SelfPurge);
            this.ModifierCounters.UnionWith(Abilities.AllyPurge);
            this.ModifierCounters.Add(Abilities.PressTheAttack);
        }

        public bool ModifierAllyCounter { get; } = true;

        public bool ModifierEnemyCounter { get; } = false;

        public void AddModifier(Modifier modifier, Unit9 modifierOwner)
        {
            var obstacle = new ModifierAllyObstacle(this, modifier, modifierOwner);
            this.Pathfinder.AddObstacle(obstacle);
        }
    }
}