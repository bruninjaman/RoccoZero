namespace O9K.Evader.Abilities.Heroes.Tinker.DefenseMatrix
{
    using Base;
    using Base.Usable.CounterAbility;

    using Core.Entities.Abilities.Base;
    using Core.Entities.Metadata;

    using Divine.Entity.Entities.Abilities.Components;

    [AbilityId(AbilityId.tinker_defense_matrix)]
    internal class DefenseMatrixBase : EvaderBaseAbility, IUsable<CounterAbility>
    {
        public DefenseMatrixBase(Ability9 ability)
            : base(ability)
        {
        }

        public CounterAbility GetUsableAbility()
        {
            return new DefenseMatrixUsable(this.Ability, this.ActionManager, this.Menu);
        }
    }
}