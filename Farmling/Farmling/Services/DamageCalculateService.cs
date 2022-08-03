using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Extensions;
using Farmling.CreepAbilities.Factory;
using Farmling.Interfaces;
using Farmling.Models;

namespace Farmling.Services;

public class DamageCalculateService : IDamageCalculateService
{
    private readonly Dictionary<uint, DamageEntity?> _modifierAbilities = new();

    public float GetDamage(Unit attacker, Unit target)
    {
        var attackerEntity = GetEntity(attacker);
        var targetEntity = GetEntity(target);
        float damage = attacker.MinimumDamage + attacker.BonusDamage;
        var itemById1 = attacker.GetItemById(AbilityId.item_quelling_blade);
        var isMelee = attacker.IsMelee;
        if (itemById1 != null)
            damage += itemById1.GetAbilitySpecialData(isMelee ? "damage_bonus" : "damage_bonus_ranged");
        var itemById2 = attacker.GetItemById(AbilityId.item_bfury);
        if (itemById2 != null)
            damage *= itemById2.GetAbilitySpecialData(isMelee ? "quelling_bonus" : "quelling_bonus_ranged") / 100f;
        if (attackerEntity.EntityType != DamageEntityType.Hero)
        {
            // Logger.Log($"Amp: {attackerEntity.Ability.GetIncomingAmp(target)}");
            damage += damage * attackerEntity.Ability.GetIncomingAmp(target);
        }

        if (targetEntity.EntityType != DamageEntityType.Hero)
        {
            // Logger.Log($"Out: {targetEntity.Ability.GetOutgoingAmp(attacker)}");
            damage += damage * targetEntity.Ability.GetOutgoingAmp(attacker);
        }

        damage *= 1 - target.PhysicalDamageResistance;
        // Logger.Log($"TotalDamage [{attacker.Name} -> {target.Name}]: {damage}");
        return damage;
    }

    public DamageEntity GetEntity(Unit unit)
    {
        var handle = unit.Handle;
        if (_modifierAbilities.TryGetValue(handle, out var attackerEntity))
        {
        }
        else
        {
            attackerEntity = DamageEntityFactory.GetAbility(unit);
            _modifierAbilities.Add(handle, attackerEntity);
        }

        return attackerEntity!;
    }
}
