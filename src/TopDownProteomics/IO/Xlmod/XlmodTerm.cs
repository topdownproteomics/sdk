using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>A term from the XLMOD modification ontology.</summary>
    public class XlmodTerm : IIdentifiable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodTerm"/> class.
        /// </summary>
        public XlmodTerm(string id, string name, string definition)
        {
            this.Id = id;
            this.Name = name;
            this.Definition = definition;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XlmodTerm"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <param name="definition">The definition.</param>
        /// <param name="externalReferences">The external references.</param>
        /// <param name="isA">The is a.</param>
        /// <param name="relationships">The relationships.</param>
        /// <param name="synonyms">The synonyms.</param>
        /// <param name="propertyValues">The property values.</param>
        public XlmodTerm(string id, string name, string definition, ICollection<XlmodExternalReference>? externalReferences,
            ICollection<string>? isA, ICollection<XlmodRelationship>? relationships, ICollection<XlmodSynonym>? synonyms, ICollection<XlmodProperty>? propertyValues) 
            : this(id, name, definition)
        {
            ExternalReferences = externalReferences;
            IsA = isA;
            Relationships = relationships;
            Synonyms = synonyms;
            PropertyValues = propertyValues;
        }

        /// <summary>The XLMOD identifier.</summary>
        public string Id { get; }

        /// <summary>The name of the term.</summary>
        public string Name { get; }

        /// <summary>The term definition.</summary>
        public string Definition { get; }

        /// <summary>The external references.</summary>
        public ICollection<XlmodExternalReference>? ExternalReferences { get; set; }

        /// <summary>The is-a collection.</summary>
        public ICollection<string>? IsA { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodRelationship>? Relationships { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodSynonym>? Synonyms { get; set; }

        /// <summary>The synonyms.</summary>
        public ICollection<XlmodProperty>? PropertyValues { get; set; }

        /// <summary>Gets the chemical formula.</summary>
        public IChemicalFormula? GetChemicalFormula(IElementProvider elementProvider)
        {
            string? formula = this.PropertyValues?.SingleOrDefault(x => x.Name == "bridgeFormula")?.Value;

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
                                element = elementProvider.GetElement(1, 2);
                            else if (isotope.HasValue)
                                element = elementProvider.GetElement(elementSymbol, isotope.Value);
                            else
                                element = elementProvider.GetElement(elementSymbol);

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
    }
}