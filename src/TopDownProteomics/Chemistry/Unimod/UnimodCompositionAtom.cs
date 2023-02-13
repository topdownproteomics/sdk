using System.Collections.Generic;

namespace TopDownProteomics.Chemistry.Unimod
{
    /// <summary>
    /// Atomic piece of a Unimod composition. Atoms can be either elements or molecular sub-units.
    /// </summary>
    /// <seealso cref="IHasChemicalFormula" />
    public class UnimodCompositionAtom : IHasChemicalFormula
    {
        private IReadOnlyCollection<IEntityCardinality<IElement>> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnimodCompositionAtom" /> class.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="description">The description.</param>
        /// <param name="elements">The elements.</param>
        public UnimodCompositionAtom(string symbol, string description,
            IReadOnlyCollection<IEntityCardinality<IElement>> elements)
        {
            this.Symbol = symbol;
            this.Description = description;
            _elements = elements;
        }

        /// <summary>
        /// Gets the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public string Description { get; }

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