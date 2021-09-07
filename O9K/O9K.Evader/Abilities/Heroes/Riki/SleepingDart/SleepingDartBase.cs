namespace O9K.Evader.Abilities.Heroes.Riki.SleepingDart;

using Base;
using Base.Evadable;
using Base.Usable.CounterAbility;
using Base.Usable.DisableAbility;

using Core.Entities.Abilities.Base;
using Core.Entities.Metadata;

using Divine.Entity.Entities.Abilities.Components;

[AbilityId(AbilityId.riki_poison_dart)]
internal class SleepingDart : EvaderBaseAbility, IEvadable, IUsable<DisableAbility>, IUsable<CounterEnemyAbility>
{
    public SleepingDart(Ability9 ability)
        : base(ability)
    {
    }

    public EvadableAbility GetEvadableAbility()
    {
        return new SleepingDartEvadable(this.Ability, this.Pathfinder, this.Menu);
    }

    public DisableAbility GetUsableAbility()
    {
        return new DisableAbility(this.Ability, this.Menu);
    }

    CounterEnemyAbility IUsable<CounterEnemyAbility>.GetUsableAbility()
    {
        return new CounterEnemyAbility(this.Ability, this.Menu);
    }
}