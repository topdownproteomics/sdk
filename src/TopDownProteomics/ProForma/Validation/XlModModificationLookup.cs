using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Xlmod;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for XlMod modifications.
    /// </summary>
    public class XlModModificationLookup : ModificationLookupBase<XlmodTerm>
    {
        private readonly IElementProvider _elementProvider;

        private XlModModificationLookup(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default modification type.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default modification type; otherwise, <c>false</c>.
        /// </value>
        protected override bool IsDefaultModificationType => true;

        /// <summary>
        /// The ProForma key.
        /// </summary>
        protected override ProFormaEvidenceType EvidenceType => ProFormaEvidenceType.XlMod;

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<XlmodTerm> modifications,
            IElementProvider elementProvider)
        {
            var lookup = new XlModModificationLookup(elementProvider);

            lookup.SetupModificationArray(modifications);

            return lookup;
        }

        /// <summary>
        /// Gets the chemical formula.
        /// </summary>
        /// <param name="modification">The modification.</param>
        /// <returns></returns>
        protected override IChemicalFormula? GetChemicalFormula(XlmodTerm modification)
        {
            string? formula = modification.PropertyValues?.SingleOrDefault(x => x.Name == "bridgeFormula")?.Value;

            if (string.IsNullOrEmpty(formula))
                return null;

            string[] cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i++)
            {
                ReadOnlySpan<char> cell = cells[i].AsSpan();
                int index = 0;
                int? isotope = null;
                bool alreadySeenCharacters = false;

                for (int j = 0; j < cell.Length; j++)
                {
                    if (cell[j] == '-')
                    {
                        index = 1;
                    }
                    else if (char.IsLetter(cell[j]))
                    {
                        alreadySeenCharacters = true;
                    }
                    else if (char.IsDigit(cell[j]))
                    {
                        if (alreadySeenCharacters) // Symbol seen, finish up from here
                        {
                            ReadOnlySpan<char> elementSymbol = cell[index..j];
                            IElement element;

                            if (elementSymbol.Length == 1 && elementSymbol[0] == 'D')
                                element = _elementProvider.GetElement(1, 2);
                            else if (isotope.HasValue)
                                element = _elementProvider.GetElement(elementSymbol, isotope.Value);
                            else
                                element = _elementProvider.GetElement(elementSymbol);

                            int count = int.Parse(cell[j..]);

                            if (cell[j] == '-')
                                count *= -1;

                            elements.Add(new EntityCardinality<IElement>(element, count));
                            break;
                        }
                        else // Must be an isotope
                        {
                            int start = j;
                            while (char.IsDigit(cell[j]))
                                j++;

                            index = j;
                            isotope = int.Parse(cell[start..j]);
                            alreadySeenCharacters = true;
                        }
                    }
                }
            }

            return new ChemicalFormula(elements);
        }

        /// <summary>
        /// Removes the prefix.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        protected override string RemovePrefix(string value)
        {
            if (value.StartsWith("XLMOD:"))
                return value[6..];

            return value;
        }
    }
}