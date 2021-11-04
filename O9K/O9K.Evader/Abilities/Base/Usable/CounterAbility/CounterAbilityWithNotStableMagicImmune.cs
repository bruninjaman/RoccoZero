using Divine.Entity.Entities.Units.Components;
using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Units;
using O9K.Evader.Abilities.Base.Usable.CounterAbility;
using O9K.Evader.Metadata;
using O9K.Evader.Pathfinder.Obstacles;

namespace O9K.Evader.Abilities.Base.Usable.CounterAbility
{
    internal class CounterAbilityWithNotStableMagicImmune : CounterAbility
    {
        public CounterAbilityWithNotStableMagicImmune(Ability9 ability, IMainMenu menu) : base(ability, menu)
        {
        }

        public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
        {
            return base.CanBeCasted(ally, enemy, obstacle) && Ability is IShield {AppliesUnitState: UnitState.MagicImmune};
        }
    }

}
