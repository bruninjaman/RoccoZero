namespace O9K.Core.Entities.Abilities.Heroes.Marci;

using Base;
using Base.Types;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Components;

using Metadata;

[AbilityId(AbilityId.marci_guardian)]
public class Sidekick : RangedAbility, IShield, IBuff
{
    public Sidekick(Ability baseAbility)
        : base(baseAbility)
    {

    }

    public string BuffModifierName { get; } = "modifier_marci_guardian_buff";

    public bool BuffsAlly { get; } = true;

    public bool BuffsOwner { get; } = true;

    public UnitState AppliesUnitState
    {
        get
        {
            var talent = this.Owner.GetAbilityById(AbilityId.special_bonus_unique_marci_guardian_magic_immune);
            if (talent?.Level > 0)
            {
                return UnitState.MagicImmune;
            }

            return UnitState.None;
        }
    }

    public string ShieldModifierName { get; } = "modifier_marci_guardian_magic_immunity";

    public bool ShieldsAlly { get; } = true;

    public bool ShieldsOwner { get; } = true;
}