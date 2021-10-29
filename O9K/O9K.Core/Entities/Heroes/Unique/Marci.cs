namespace O9K.Core.Entities.Heroes.Unique;

using System;

using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;

using Metadata;

using O9K.Core.Data;
using O9K.Core.Entities.Units;

[HeroId(HeroId.npc_dota_hero_marci)]
internal class Marci : Hero9
{
    private bool useUnleashBonusAttackSpeed;

    public Marci(Hero baseHero)
        : base(baseHero)
    {
    }

    internal override float GetAttackSpeed(Unit9 target = null)
    {
        if (this.useUnleashBonusAttackSpeed)
        {
            return Math.Max(this.BaseUnit.AttackSpeed, GameData.MinAttackSpeed);
        }

        return Math.Max(Math.Min(this.BaseUnit.AttackSpeed, GameData.MaxAttackSpeed), GameData.MinAttackSpeed);
    }

    internal void Unleashed(bool value)
    {
        this.useUnleashBonusAttackSpeed = value;
    }
}