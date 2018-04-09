using System.Collections.Generic;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// An observed proteoform with a sequence and modifications with possible ambiguity.
    /// </summary>
    public interface IProteoformHypothesis
    {
        /// <summary>
        /// The amino acid sequence.
        /// </summary>
        string Sequence { get; }

        /// <summary>
        /// The modifications on this proteoform.
        /// </summary>
        ICollection<IProteoformModification> Modifications { get; }
    }

    /// <summary>
    /// Modification on a chemical proteoform.
    /// </summary>
    public interface IProteoformModification
    {

    }
}