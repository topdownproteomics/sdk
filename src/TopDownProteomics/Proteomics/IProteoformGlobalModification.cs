using System.Collections.Generic;

namespace TopDownProteomics.Proteomics
{
    /// <summary>A modification that is applied globally to a proteoform based on target amino acids.</summary>
    public interface IProteoformGlobalModification : IProteoformModification
    {
        /// <summary>The target amino acids of this global modification. If NULL, it is targetting isotopes instead.</summary>
        ICollection<char>? TargetAminoAcids { get; }
    }
}