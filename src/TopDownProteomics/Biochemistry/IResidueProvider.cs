using System.Collections.Generic;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>
    /// Anything that can provide residues.
    /// </summary>
    public interface IResidueProvider
    {
        /// <summary>
        /// Gets the residue.
        /// </summary>
        /// <param name="symbol">The symbol of the residue.</param>
        /// <returns></returns>
        IResidue GetResidue(char symbol);

        /// <summary>
        /// Gets all resiudes.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IResidue> GetResidues();
    }
}