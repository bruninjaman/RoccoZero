namespace O9K.Core.Entities.Abilities.Heroes.Lifestealer;

using Base;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using Helpers;

using Metadata;

[AbilityId(AbilityId.life_stealer_feast)]
public class Feast : PassiveAbility
{
    public Feast(Ability baseAbility)
        : base(baseAbility)
    {
        this.DamageData = new SpecialData(baseAbility, "hp_leech_percent");
    }
}