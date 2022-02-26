namespace O9K.Core.Entities.Abilities.Heroes.Techies;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;

using O9K.Core.Entities.Abilities.Base;
using O9K.Core.Entities.Abilities.Base.Types;
using O9K.Core.Entities.Metadata;
using O9K.Core.Helpers;

[AbilityId(AbilityId.techies_sticky_bomb)]
public class StickyBomb : CircleAbility, INuke, IDebuff
{
    public StickyBomb(Ability baseAbility)
    : base(baseAbility)
    {
        this.RadiusData = new SpecialData(baseAbility, "radius");
        this.SpeedData = new SpecialData(baseAbility, "speed");
        this.DamageData = new SpecialData(baseAbility, "damage");
    }

    public string DebuffModifierName { get; } = "modifier_techies_sticky_bomb_slow";
}