using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Modification on a chemical proteoform that has a zero-based index.
    /// </summary>
    /// <seealso cref="TopDownProteomics.Proteomics.IProteoformModification" />
    public interface IProteoformModificationWithIndex : IProteoformModification
    {
        /// <summary>
        /// Gets the zero-based index in the sequence.
        /// </summary>
        int ZeroBasedIndex { get; }
    }
}
