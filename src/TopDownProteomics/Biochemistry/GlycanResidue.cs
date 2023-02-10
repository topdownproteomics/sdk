using System.Collections.Generic;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>A building block of glycan structures.</summary>
    /// <seealso cref="IGlycanResidue" />
    public class GlycanResidue : IGlycanResidue
    {
        private IReadOnlyCollection<IEntityCardinality<IElement>> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="GlycanResidue"/> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="name">The name.</param>
        /// <param name="elements">The elements.</param>
        public GlycanResidue(string symbol, string name, IReadOnlyCollection<IEntityCardinality<IElement>> elements)
        {
            Name = name;
            Symbol = symbol;
            _elements = elements;
        }

        /// <summary>The name of the glycan residue</summary>
        public string Name { get; }

        /// <summary>The glycan residue's symbol</summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <returns></returns>
        public ChemicalFormula GetChemicalFormula() => new ChemicalFormula(_elements);

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IEntityCardinality<IElement>> GetElements() => _elements;
    }
}