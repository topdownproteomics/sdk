using System;
using System.Collections.Generic;
using System.Linq;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Default implementation of a chemical formula.
    /// </summary>
    /// <seealso cref="IChemicalFormula" />
    public class ChemicalFormula : IChemicalFormula
    {
        /// <summary>The static empty chemical formula.</summary>
        public static IChemicalFormula Empty = new ChemicalFormula();

        private List<IEntityCardinality<IElement>> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChemicalFormula"/> class.
        /// </summary>
        private ChemicalFormula()
        {
            _elements = new List<IEntityCardinality<IElement>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChemicalFormula" /> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public ChemicalFormula(IEnumerable<IEntityCardinality<IElement>> elements)
        {
            _elements = elements.ToList();
        }

        ///// <summary>
        ///// Adds the element.
        ///// </summary>
        ///// <param name="element">The element.</param>
        ///// <param name="count">The count.</param>
        //public void AddElement(IElement element, int count)
        //{
        //    _elements.Add(new EntityCardinality<IElement>(element, count));
        //}

        /// <summary>
        /// Waters the specified element provider.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static ChemicalFormula Water(IElementProvider elementProvider)
        {
            return new ChemicalFormula(new[]
            {
                new EntityCardinality<IElement>(elementProvider.GetElement(1), 2),
                new EntityCardinality<IElement>(elementProvider.GetElement(8), 1)
            });
        }

        /// <summary>
        /// Gets the elements.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<IEntityCardinality<IElement>> GetElements() => _elements;

        /// <summary>
        /// Gets the mass.
        /// </summary>
        /// <param name="massType">Type of the mass.</param>
        /// <returns></returns>
        public double GetMass(MassType massType)
        {
            return _elements.Sum(x => x.Count * x.Entity.GetMass(massType));
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(IChemicalFormula other)
        {
            if (other == null)
                return false;

            if (this == other)
                return true;

            IReadOnlyCollection<IEntityCardinality<IElement>> otherElements = other.GetElements();

            if (_elements.Count != otherElements.Count)
                return false;

            if (_elements.Sum(x => x.Count) != otherElements.Sum(x => x.Count))
                return false;

            foreach (IEntityCardinality<IElement> element in _elements)
            {
                var otherElement = otherElements.SingleOrDefault(x => x.Entity.Equals(element.Entity));

                // Check that the other chemical formula has this element.
                if (otherElement == null)
                    return false;

                // Check the counts.
                if (element.Count != otherElement.Count)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is IChemicalFormula other)
                return this.Equals(other);

            return false;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            // RTF: TODO: This is a terrible hash code, but at least it favors a correct result.

            //return _elements.GetHashCode();
            return _elements.Count;
        }

        /// <summary>
        /// Adds the specified formula.
        /// </summary>
        /// <param name="otherFormula">The formula.</param>
        /// <returns></returns>
        public IChemicalFormula Add(IChemicalFormula otherFormula)
        {
            return this.Merge(otherFormula, true);
        }

        /// <summary>
        /// Subtracts the specified formula.
        /// </summary>
        /// <param name="otherFormula"></param>
        /// <returns></returns>
        public IChemicalFormula Subtract(IChemicalFormula otherFormula)
        {
            return this.Merge(otherFormula, false);
        }

        private IChemicalFormula Merge(IChemicalFormula otherFormula, bool add)
        {
            if (otherFormula == null)
                return this;

            var otherElements = otherFormula.GetElements().ToList();

            if (otherElements.Count == 0)
                return this;

            var formula = new ChemicalFormula();

            // Add everything from this formula
            foreach (var element in _elements)
            {
                var otherElement = otherElements.SingleOrDefault(x => x.Entity.Equals(element.Entity));

                if (otherElement == null)
                    formula._elements.Add(element);
                else
                {
                    int newCount = add ? element.Count + otherElement.Count : element.Count - otherElement.Count;

                    if (newCount != 0)
                        formula._elements.Add(new EntityCardinality<IElement>(element.Entity, newCount));

                    otherElements.Remove(otherElement);
                }
            }

            // Add unique things from other formula
            foreach (var otherElement in otherElements)
            {
                if (add)
                    formula._elements.Add(otherElement);
                else
                    formula._elements.Add(new EntityCardinality<IElement>(otherElement.Entity, -otherElement.Count));
            }

            return formula;
        }

        /// <summary>
        /// Multiplies the formula by the specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns></returns>
        public IChemicalFormula Multiply(int multiplier)
        {
            var formula = new ChemicalFormula();

            foreach (var element in _elements)
                formula._elements.Add(new EntityCardinality<IElement>(element.Entity, element.Count * multiplier));

            return formula;
        }

        /// <summary>Parses the string into a chemical formula.</summary>
        /// <param name="formula">The formula.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not parse '{formula.ToString()}' into a chemical formula.</exception>
        public static IChemicalFormula ParseString(ReadOnlySpan<char> formula, IElementProvider elementProvider)
        {
            if (TryParseString(formula, elementProvider, out IChemicalFormula result))
                return result;

            throw new InvalidChemicalFormula($"Could not parse '{formula.ToString()}' into a chemical formula.");
        }

        /// <summary>Attempts to parse the string into a ChemicalFormula.</summary>
        /// <param name="formula">The chemical formula as a string.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <param name="chemicalFormula">The chemical formula or null if string was not formatted correctly.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool TryParseString(ReadOnlySpan<char> formula, IElementProvider elementProvider, out IChemicalFormula chemicalFormula)
        {
            chemicalFormula = ChemicalFormula.Empty; // Set to null in case of failure.

            IDictionary<string, IEntityCardinality<IElement>> elementList = new Dictionary<string, IEntityCardinality<IElement>>();

            int symbolStart = 0, symbolEnd = 0;
            int digitStart = 0, digitEnd = 0;
            int isotopeStart = 0, isotopeEnd = 0;
            bool sawUpperCaseLetter = false;

            for (int i = 0; i < formula.Length; i++)
            {
                // Check to see if this is the start of a new element
                if (i > 0 && (formula[i] == '[' || (sawUpperCaseLetter && char.IsUpper(formula[i]))))
                {
                    if (!HandleNewElement(formula, elementProvider, elementList, symbolStart, symbolEnd, digitStart, digitEnd, isotopeStart, isotopeEnd))
                        return false;

                    // Reset things
                    sawUpperCaseLetter = false;
                    digitStart = 0;
                    digitEnd = 0;
                    isotopeStart = 0;
                    isotopeEnd = 0;
                }

                if (formula[i] == '[')
                {
                    isotopeStart = i++ + 1;
                }
                else if (formula[i] == ']')
                {
                    // Ignore because a new element is starting on the next loop
                }
                else if (char.IsLetter(formula[i]))
                {
                    if (char.IsUpper(formula[i]))
                    {
                        // New element
                        sawUpperCaseLetter = true;
                        symbolStart = i;
                        symbolEnd = i;

                        if (isotopeStart != 0)
                            isotopeEnd = i - 1;
                    }
                    else
                    {
                        // Lowercase letter continuing the current element.
                        symbolEnd = i;
                    }
                }
                else if (isotopeStart != 0 && isotopeEnd == 0)
                {
                    // In an isotope block looking for isotope numbers, do nothing
                }
                else if (formula[i] == '-')
                {
                    // Handle negative cardinalities
                    if (digitStart == 0)
                    {
                        digitStart = i;
                        digitEnd = i;
                    }
                }
                else if (char.IsNumber(formula[i]))
                {
                    if (digitStart == 0)
                        digitStart = i;

                    digitEnd = i;
                }
                else if (char.IsWhiteSpace(formula[i]))
                {
                    // Ignore white space
                }
                else
                {
                    // Known character, fail!
                    return false;
                }
            }

            // Add the last element
            if (!HandleNewElement(formula, elementProvider, elementList, symbolStart, symbolEnd, digitStart, digitEnd, isotopeStart, isotopeEnd))
                return false;

            // Success!
            chemicalFormula = new ChemicalFormula(elementList.Values);
            return true;
        }

        private static bool HandleNewElement(ReadOnlySpan<char> formula, IElementProvider elementProvider,
            IDictionary<string, IEntityCardinality<IElement>> elementList, int symbolStart, int symbolEnd,
            int digitStart, int digitEnd, int isotopeStart, int isotopeEnd)
        {
            if (formula.Length == 0)
                return false;

            // Handle cardinality
            int count = 1;
            if (digitStart != 0)
            {
                var digitSpan = formula.Slice(digitStart, digitEnd - digitStart + 1);
                if (!int.TryParse(digitSpan, out count))
                    //throw new Exception($"Can't convert {digitSpan.ToString()} to an integer.");
                    return false;

                if (count == 0)
                    return true; // A valid cardinality, but nothing should be added to the formula
            }

            // Handle isotopes
            int? isotope = null;
            if (isotopeStart != 0)
            {
                var isotopeSpan = formula.Slice(isotopeStart, isotopeEnd - isotopeStart + 1);
                int isotopeOut;
                if (!int.TryParse(isotopeSpan, out isotopeOut))
                    //throw new Exception($"Can't convert {digitSpan.ToString()} to an integer.");
                    return false;

                isotope = isotopeOut;
            }

            try
            {
                string symbol = formula.Slice(symbolStart, symbolEnd - symbolStart + 1).ToString();

                var element = GetElement(symbol, isotope, count, elementProvider);

                if (elementList.ContainsKey(element.Entity.Symbol))
                {
                    IEntityCardinality<IElement> entityCardinality = elementList[element.Entity.Symbol];
                    elementList[symbol] = new EntityCardinality<IElement>(entityCardinality.Entity, entityCardinality.Count + count);
                }
                else
                {
                    elementList.Add(element.Entity.Symbol, element);
                }

                return true;
            }
            catch // Failed to get element for some reason
            {
                return false;
            }
        }

        private static IEntityCardinality<IElement> GetElement(ReadOnlySpan<char> symbol, int? isotope, int count, IElementProvider elementProvider)
        {
            return new EntityCardinality<IElement>(elementProvider.GetElement(symbol, isotope), count);
        }

        /// <summary>
        /// Invalid Chemical Formula when parsing a text string.
        /// </summary>
        /// <seealso cref="Exception" />
        [Serializable]
        public class InvalidChemicalFormula : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidChemicalFormula"/> class.
            /// </summary>
            public InvalidChemicalFormula() { }

            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidChemicalFormula"/> class.
            /// </summary>
            /// <param name="message">The message that describes the error.</param>
            public InvalidChemicalFormula(string message) : base(message) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="InvalidChemicalFormula"/> class.
            /// </summary>
            /// <param name="message">The error message that explains the reason for the exception.</param>
            /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
            public InvalidChemicalFormula(string message, Exception innerException) : base(message, innerException) { }
        }
    }
}