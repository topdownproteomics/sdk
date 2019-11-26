using System;
using System.Collections.Generic;
using System.Text;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{

    /// <summary>Lookup for modifications given by chemical formula.</summary>
    /// <seealso cref="TopDownProteomics.ProForma.Validation.IProteoformModificationLookup" />
    public class FormulaLookup : IProteoformModificationLookup
    {
        private IElementProvider _elementProvider;

        /// <summary>Initializes a new instance of the <see cref="FormulaLookup"/> class.</summary>
        /// <param name="elementProvider">The element provider.</param>
        public FormulaLookup(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
        }

        /// <summary>Determines whether this instance [can handle descriptor] the specified descriptor.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        ///   <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.</returns>
        public bool CanHandleDescriptor(ProFormaDescriptor descriptor)
        {
            return descriptor.Key == ProFormaKey.Formula;
        }

        /// <summary>Gets the modification.</summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification GetModification(ProFormaDescriptor descriptor)
        {
            if (ChemicalFormula.TryParseString(descriptor.Value, this._elementProvider, out ChemicalFormula chemicalFormula))
            {
                return new FormulaModification(chemicalFormula);
            }
            else
            {
                throw new ProteoformModificationLookupException($"Could not parse formula string for descriptor {descriptor.ToString()}");
            }
        }

        private class FormulaModification : IProteoformModification
        {
            private IChemicalFormula _chemicalFormula;

            public FormulaModification(IChemicalFormula chemicalFormula)
            {
                _chemicalFormula = chemicalFormula;
            }

            public IChemicalFormula GetChemicalFormula()
            {
                return this._chemicalFormula;
            }
        }
    }
}
