namespace O9K.Core.Entities.Abilities.Heroes.StormSpirit
{
    using Base;
    using Base.Components;
    using Base.Types;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;

    using Entities.Units;

    using Helpers;
    using Helpers.Damage;

    using Metadata;

    [AbilityId(AbilityId.storm_spirit_overload)]
    public class Overload : ActiveAbility, IHasPassiveDamageIncrease, IBuff
    {
        public Overload(Ability baseAbility)
            : base(baseAbility)
        {
            DamageData = new SpecialData(baseAbility, "overload_damage");
        }

        public string BuffModifierName { get; } = "modifier_storm_spirit_electric_rave";

        public bool BuffsAlly { get; } = false;

        public bool BuffsOwner { get; } = true;

        public bool IsPassiveDamagePermanent { get; } = false;

        public bool MultipliedByCrit { get; } = false;

        public string PassiveDamageModifierName { get; } = "modifier_storm_spirit_overload";

        public override Damage GetRawDamage(Unit9 unit, float? remainingHealth = null)
        {
            var damage = new Damage();

            if (!unit.IsBuilding && unit.IsUnit && !unit.IsAlly(Owner))
            {
                damage[DamageType] = DamageData.GetValue(Level);
            }

            return damage;
        }
    }
}