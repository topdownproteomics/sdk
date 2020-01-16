using System;
using System.Collections.Generic;
using System.Text;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Proteomics
{
    /// <summary>Modification on a chemical proteoform that has a zero-based index.</summary>
    /// <seealso cref="TopDownProteomics.Proteomics.IProteoformModificationWithIndex" />
    public class ProteoformModificationWithIndex : IProteoformModificationWithIndex
    {
        private IProteoformModification _proteoformModification;

        /// <summary>Initializes a new instance of the <see cref="ProteoformModificationWithIndex"/> class.</summary>
        /// <param name="proteoformModification">The proteoform modification.</param>
        /// <param name="zeroBasedIndex">  zero-based index.</param>
        public ProteoformModificationWithIndex(IProteoformModification proteoformModification, int zeroBasedIndex)
        {
            _proteoformModification = proteoformModification;
            ZeroBasedIndex = zeroBasedIndex;
        }

        /// <summary>Gets the zero-based index in the sequence.</summary>
        public int ZeroBasedIndex { get; }

        /// <summary>Gets the chemical formula.</summary>
        /// <returns></returns>
        public IChemicalFormula GetChemicalFormula()
        {
            return this._proteoformModification.GetChemicalFormula();
        }
    }
}
