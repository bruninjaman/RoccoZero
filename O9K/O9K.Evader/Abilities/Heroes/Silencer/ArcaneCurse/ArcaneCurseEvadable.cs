namespace O9K.Evader.Abilities.Heroes.Silencer.ArcaneCurse
{
    using Base;
    using Base.Evadable;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Units;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Modifier.Modifiers;

    using Metadata;

    using Pathfinder.Obstacles.Modifiers;

    internal sealed class ArcaneCurseEvadable : LinearAreaOfEffectEvadable, IModifierCounter
    {
        public ArcaneCurseEvadable(Ability9 ability, IPathfinder pathfinder, IMainMenu menu)
            : base(ability, pathfinder, menu)
        {
            this.Counters.Add(Abilities.PhaseShift);

            this.ModifierCounters.UnionWith(Abilities.AllyPurge);
            this.ModifierCounters.UnionWith(Abilities.SelfPurge);
            this.ModifierCounters.Remove(AbilityId.item_black_king_bar);
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