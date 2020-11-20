using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Proteomics
{
    /// <summary>An observed proteoform with a sequence and modifications with possible ambiguity.</summary>
    public interface IProteoformGroup : IHasMass
    {
        /// <summary>The amino acid residues.</summary>
        IReadOnlyList<IResidue> Residues { get; }

        /// <summary>The N-terminal modification on this proteoform.</summary>
        IProteoformMassDelta? NTerminalModification { get; }

        /// <summary>The C-terminal modification on this proteoform.</summary>
        IProteoformMassDelta? CTerminalModification { get; }

        /// <summary>Localized modifications on this proteoform.</summary>
        IReadOnlyCollection<IProteoformLocalizedModification>? LocalizedModifications { get; }

        /// <summary>Unlocalized modifications on this proteoform.</summary>
        IReadOnlyCollection<IProteoformUnlocalizedModification>? UnlocalizedModifications { get; }

        /// <summary>Modification groups on this proteoform.</summary>
        IReadOnlyCollection<IProteoformModificationGroup>? ModificationGroups { get; }

        /// <summary>Global modifications on this proteoform.</summary>
        IReadOnlyCollection<IProteoformGlobalModification>? GlobalModifications { get; }
    }

    /// <summary>Utility methods for proteoform related taskes.</summary>
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