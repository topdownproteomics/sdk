using System;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>A class containing biochemistry utility functions.</summary>
    public static class BiochemistryUtility
    {
        /// <summary>Determines whether the specified symbol corresponds to an ambiguous residue.</summary>
        /// <param name="symbol">The residue symbol.</param>
        public static bool IsAmbiguousAminoAcidResidue(char symbol)
        {
            return symbol == 'X' || symbol == 'J' || symbol == 'B' || symbol == 'Z';
        }

        /// <summary>Determines whether the specified sequence contains an ambiguous residue.</summary>
        /// <param name="sequence">The sequence.</param>
        public static bool ContainsAmbiguousAminoAcidResidue(this ReadOnlySpan<char> sequence)
        {
            for (int i = 0; i < sequence.Length; i++)
            {
                if (IsAmbiguousAminoAcidResidue(sequence[i]))
                    return true;
            }

            return false;
        }
    }
}