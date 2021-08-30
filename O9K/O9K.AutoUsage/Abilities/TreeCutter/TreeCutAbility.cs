namespace O9K.AutoUsage.Abilities.TreeCutter
{
    using System.Collections.Generic;
    using System.Linq;

    using Core.Entities.Abilities.Base.Components.Base;
    using Core.Entities.Units;

    using Divine.Entity;
    using Divine.Entity.Entities.Trees;
    using Divine.Helpers;

    public abstract class TreeCutAbility : UsableAbility
    {

        protected TreeCutSettings settings;

        private static IEnumerable<TempTree> _trees;

        private static bool _usedCutAbility;

        public TreeCutAbility(IActiveAbility ability)
            : base(ability)
        {
        }

        public override bool UseAbility(List<Unit9> heroes)
        {

            var currentTempTrees = EntityManager.GetEntities<TempTree>();

            if (_trees?.Count() - 1 == currentTempTrees.Count() && _usedCutAbility)
            {
                _usedCutAbility = false;

                MultiSleeper<string>.Sleep("AutoUsage.Special.TreeCut",
                    this.settings.DelayBeforeNextActivation);
            }

            _trees = EntityManager.GetEntities<TempTree>();

            foreach (var tree in _trees
                .OrderBy(x => this.Owner.GetAngle(x.Position))
                .ThenBy(x => this.Owner.Distance(x.Position)))
            {
                if (this.Ability.CastRange < this.Owner.Distance(tree.Position) ||
                    MultiSleeper<string>.Sleeping("AutoUsage.Special.TreeCut"))
                {
                    continue;
                }

                if (this.Ability.UseAbility(tree))
                {
                    _usedCutAbility = true;

                    return true;
                }
            }

            return false;
        }
    }
}