// <copyright file="IHasProcChance.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.Components
{
    using Ensage.SDK.Utils;

    public interface IHasProcChance
    {
        /// <summary>
        ///     Gets a value indicating whether the <see cref="ProcChance" /> is not the actual proc chance. To get the actual
        ///     chance see <see cref="Utils.GetPseudoChance" />.
        /// </summary>
        bool IsPseudoChance { get; }

        /// <summary>
        ///     Gets the proc chance with from 0 to 1.
        /// </summary>
        float ProcChance { get; }
    }
}