namespace O9K.Core.Entities.Abilities.Base.Components;

using Divine.Entity.Entities.Abilities.Components;

using Entities.Units;

public interface IHasDamageBlock
{
    DamageType BlockDamageType { get; }

    string BlockModifierName { get; }

    bool BlocksDamageAfterReduction { get; }

    bool IsDamageBlockPermanent { get; }

    bool IsValid { get; }

    float BlockValue(Unit9 target);
}