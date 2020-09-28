using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// An observed proteoform with a sequence and modifications with possible ambiguity.
    /// </summary>
    public interface IProteoformGroup : IHasMass
    {
        /// <summary>The amino acid residues.</summary>
        IReadOnlyList<IResidue> Residues { get; }

        /// <summary>The N terminal modification on this proteoform.</summary>
        IProteoformModification? NTerminalModification { get; }
        
        /// <summary>The C terminal modification on this proteoform.</summary>
        IProteoformModification? CTerminalModification { get; }

        /// <summary>The modifications on this proteoform.</summary>
        IReadOnlyCollection<IProteoformModificationWithIndex>? Modifications { get; }
    }

    /// <summary>
    /// Utility methods for proteoform related taskes.
    /// </summary>
    public static class ProteoformUtility
    {
        /// <summary>
        /// Gets the sequence.
        /// </summary>
        /// <param name="proteoform">The proteoform.</param>
        /// <returns></returns>
        public static string? GetSequence(this IProteoformGroup proteoform)
        {
            if (proteoform?.Residues == null)
                return null;

            return new string(proteoform.Residues.Select(x => x.Symbol).ToArray());
        }
    }
}