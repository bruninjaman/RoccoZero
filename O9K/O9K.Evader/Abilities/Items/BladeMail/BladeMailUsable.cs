namespace O9K.Evader.Abilities.Items.BladeMail;

using Base.Usable.CounterAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Units;

using Divine.Entity.Entities.Abilities.Components;

using Metadata;

using Pathfinder.Obstacles;

internal class BladeMailUsable : CounterAbility
{
    public BladeMailUsable(Ability9 ability, IMainMenu menu)
        : base(ability, menu)
    {
    }

    public override bool CanBeCasted(Unit9 ally, Unit9 enemy, IObstacle obstacle)
    {
        if (!base.CanBeCasted(ally, enemy, obstacle))
        {
            return false;
        }

        return obstacle.EvadableAbility.Ability.DamageType switch
        {
            DamageType.Magical => !obstacle.EvadableAbility.Owner.IsMagicImmune,
            DamageType.Physical => !obstacle.EvadableAbility.Owner.IsAttackImmune,
            _ => true
        };
    }
}