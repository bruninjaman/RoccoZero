    using Divine.Entity.Entities.Abilities.Components;
    using O9K.Core.Entities.Abilities.Base;
    using O9K.Core.Entities.Metadata;
    using O9K.Evader.Abilities.Base;
    using O9K.Evader.Abilities.Base.Usable.CounterAbility;

    namespace O9K.Evader.Abilities.Heroes.Marci.Sidekick
    {
        [AbilityId(AbilityId.marci_grapple)]
        internal class DisposeBase : EvaderBaseAbility, IUsable<CounterAbility>
        {
            public DisposeBase(Ability9 ability) : base(ability)
            {
            }

            CounterAbility IUsable<CounterAbility>.GetUsableAbility()
            {
                return new CounterEnemyAbility(this.Ability, this.Menu);
            }
        }
    
    }