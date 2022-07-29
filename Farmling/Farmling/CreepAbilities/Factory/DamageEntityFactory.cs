using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Farmling.Models;

namespace Farmling.CreepAbilities.Factory;

public static class DamageEntityFactory
{
    public static DamageEntity GetAbility(Unit unit)
    {
        var spells = unit.Spellbook.Spells;
        DamageEntity? entity;
        foreach (var spell in spells)
            switch (spell.Id)
            {
                case AbilityId.creep_piercing:
                {
                    entity = new DamageEntity
                    {
                        EntityType = DamageEntityType.RangeCreep,
                        Ability = new Piercing(spell)
                    };
                    return entity;
                }
                case AbilityId.creep_siege:
                {
                    entity = new DamageEntity
                    {
                        EntityType = DamageEntityType.Siege,
                        Ability = new Reinforced(spell)
                    };
                    return entity;
                }
                case AbilityId.creep_irresolute:
                {
                    entity = new DamageEntity
                    {
                        EntityType = DamageEntityType.MeleeCreep,
                        Ability = new Runty(spell)
                    };
                    return entity;
                }
            }

        entity = new DamageEntity
        {
            EntityType = DamageEntityType.Hero
        };
        return entity;
    }
}
