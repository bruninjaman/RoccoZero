namespace Divine.Core.Entities.Abilities.Components
{
    public interface IHasDot : IHasTargetModifier
    {
        /// <summary>
        ///     Gets the duration of the dot.
        /// </summary>
        float DamageDuration { get; }

        /// <summary>
        ///     Gets a value indicating whether the dot has an initial damage instance or applies its first tick immediately. Use
        ///     <see cref="ActiveAbility.GetDamage" /> to get the damage value.
        /// </summary>
        bool HasInitialDamage { get; }

        /// <summary>
        ///     Gets the raw tick damage without any amplification.
        /// </summary>
        float RawTickDamage { get; }

        /// <summary>
        ///     Gets the time between damage instances of the dot.
        /// </summary>
        float TickRate { get; }

        /// <summary>
        ///     Gets the dot's damage of each tick.
        /// </summary>
        /// <param name="targets">The target which has the dot.</param>
        /// <returns>Damage of each tick.</returns>
        float GetTickDamage(params CUnit[] targets);

        /// <summary>
        ///     Gets the total damage of the dot, including the initial damage and the total possible tick damage.
        /// </summary>
        /// <param name="targets">The target(s) which have the dot applied.</param>
        /// <returns>Total damage.</returns>
        float GetTotalDamage(params CUnit[] targets);
    }
}