using System;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>Which termini are supported by a modification.</summary>
    [Flags]
    public enum ModificationTerminalSpecificity
    {
        /// <summary>
        /// No termini are allowed.
        /// </summary>
        None = 0,

        /// <summary>
        /// (N)itrogen
        /// </summary>
        N = 1,

        /// <summary>
        /// (C)arbon
        /// </summary>
        C = 2,

        /// <summary>
        /// Both termini are allowed.
        /// </summary>
        Both = N | C
    }
}