using System;
using System.Collections.Generic;
using System.Linq;

namespace TopDownProteomics.Chemistry
{
    /// <summary>
    /// Default implementation of a chemical formula.
    /// </summary>
    /// <seealso cref="ChemicalFormula" />
    public class ChemicalFormula : IHasMass, IEquatable<ChemicalFormula>
    {
        /// <summary>The static empty chemical formula.</summary>
        public static ChemicalFormula Empty = new();

        private const int CommonElementLength = 6;

        // This is a fixed set of common things in this order: CHNOSP
        private ReadOnlyMemory<int>? _commonElements;
        private ReadOnlyMemory<IElement>? _commonElementEntities;

        private IReadOnlyList<IEntityCardinality<IElement>>? _uncommonElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChemicalFormula"/> class.
        /// </summary>
        private ChemicalFormula() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChemicalFormula" /> class.
        /// </summary>
        /// <param name="elements">The elements.</param>
        public ChemicalFormula(IEnumerable<IEntityCardinality<IElement>> elements)
        {
            int[]? common = null;
            IElement[]? entities = null;

            static void PopulateCommonElement(ref int[]? common, ref IElement[]? entities, int index, IEntityCardinality<IElement> cardinality)
            {
                common ??= new int[CommonElementLength];
                entities ??= new IElement[CommonElementLength];

                common[index] = cardinality.Count;
                entities[index] = cardinality.Entity;
            };

            foreach (var element in elements)
            {
                if (element.Entity.Symbol == "C") PopulateCommonElement(ref common, ref entities, 0, element);
                else if (element.Entity.Symbol == "H") PopulateCommonElement(ref common, ref entities, 1, element);
                else if (element.Entity.Symbol == "N") PopulateCommonElement(ref common, ref entities, 2, element);
                else if (element.Entity.Symbol == "O") PopulateCommonElement(ref common, ref entities, 3, element);
                else if (element.Entity.Symbol == "S") PopulateCommonElement(ref common, ref entities, 4, element);
                else if (element.Entity.Symbol == "P") PopulateCommonElement(ref common, ref entities, 5, element);
                else
                {
                    // Some type gymnastics here because I want the type to be read-only generally
                    _uncommonElements ??= new List<IEntityCardinality<IElement>>();
                    ((List<IEntityCardinality<IElement>>)_uncommonElements).Add(element);
                }
            }

            if (common is not null && entities is not null)
            {
                _commonElements = common;
                _commonElementEntities = entities;
            }
        }

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
        public IReadOnlyCollection<IEntityCardinality<IElement>> GetElements()
        {
            if (!_commonElements.HasValue && _uncommonElements is null)
                return Array.Empty<IEntityCardinality<IElement>>();

            var cardinalities = new List<IEntityCardinality<IElement>>(_commonElements?.Length + _uncommonElements?.Count ?? 0);

            if (_commonElements.HasValue && _commonElementEntities.HasValue)
            {
                ReadOnlySpan<int> thisCommonElements = _commonElements.Value.Span;
                ReadOnlySpan<IElement> thisCommonElementEntities = _commonElementEntities.Value.Span;

                for (int i = 0; i < CommonElementLength; i++)
                {
                    if (thisCommonElements[i] != 0)
                        cardinalities.Add(new EntityCardinality<IElement>(thisCommonElementEntities[i], thisCommonElements[i]));
                }
            }

            if (_uncommonElements is not null)
            {
                foreach (var cardinality in _uncommonElements)
                    cardinalities.Add(new EntityCardinality<IElement>(cardinality.Entity, cardinality.Count));
            }

            return cardinalities;
        }

        /// <summary>
        /// Gets the mass.
        /// </summary>
        /// <param name="massType">Type of the mass.</param>
        /// <returns></returns>
        public double GetMass(MassType massType)
        {
            double mass = 0;

            if (_commonElements.HasValue && _commonElementEntities.HasValue)
            {
                ReadOnlySpan<int> thisCommonElements = _commonElements.Value.Span;
                ReadOnlySpan<IElement> thisCommonElementEntities = _commonElementEntities.Value.Span;

                for (int i = 0; i < CommonElementLength; i++)
                    if (thisCommonElements[i] != 0)
                        mass += thisCommonElements[i] * thisCommonElementEntities[i].GetMass(massType);
            }

            if (_uncommonElements is not null)
                mass += _uncommonElements.Sum(x => x.Count * x.Entity.GetMass(massType));

            return mass;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other">other</paramref> parameter; otherwise, false.
        /// </returns>
        public bool Equals(ChemicalFormula other)
        {
            if (other is null)
                return false;

            if (this == other)
                return true;

            if (!_commonElements.HasValue && other._commonElements.HasValue)
                return false;

            if (_commonElements.HasValue && !other._commonElements.HasValue)
                return false;

            bool commonResult = _commonElements.HasValue && other._commonElements.HasValue &&
                _commonElements.Value.Span.SequenceEqual(other._commonElements.Value.Span);

            if (!commonResult)
                return false;

            // This will usually be true if both are null
            if (_uncommonElements == other._uncommonElements)
                return true;

            if (_uncommonElements?.Count != other._uncommonElements?.Count)
                return false;

            if (_uncommonElements.Sum(x => x.Count) != other._uncommonElements.Sum(x => x.Count))
                return false;

            // This if() block should never fail, but the compiler can't figure that out easily
            if (_uncommonElements is not null && other._uncommonElements is not null)
            {
                foreach (IEntityCardinality<IElement> element in _uncommonElements)
                {
                    var otherElement = other._uncommonElements.SingleOrDefault(x => x.Entity.Equals(element.Entity));

                    // Check that the other chemical formula has this element.
                    if (otherElement == null)
                        return false;

                    // Check the counts.
                    if (element.Count != otherElement.Count)
                        return false;
                }
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
            if (obj is ChemicalFormula other)
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
            if (!_commonElements.HasValue)
                return _uncommonElements?.Count ?? 0; // Not a good hash code, but fast and this should be an uncommon case

            int value = 0;

            for (int i = 0; i < CommonElementLength; i++)
                value = HashCode.Combine(_commonElements.Value.Span[i], value);

            if (_uncommonElements is not null)
                value = HashCode.Combine(_uncommonElements.Count, value);

            return value;
        }

        /// <summary>Converts to string.</summary>
        /// <returns>
        /// A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString() => this.GetChemicalFormulaString();

        /// <summary>
        /// Adds the specified formula.
        /// </summary>
        /// <param name="otherFormula">The formula.</param>
        /// <returns></returns>
        public ChemicalFormula Add(ChemicalFormula otherFormula)
        {
            return this.Merge(otherFormula, true);
        }

        /// <summary>
        /// Subtracts the specified formula.
        /// </summary>
        /// <param name="otherFormula"></param>
        /// <returns></returns>
        public ChemicalFormula Subtract(ChemicalFormula otherFormula)
        {
            return this.Merge(otherFormula, false);
        }

        private ChemicalFormula Merge(ChemicalFormula otherFormula, bool add)
        {
            if (otherFormula is null)
                return this;

            if (!otherFormula._commonElements.HasValue && otherFormula._uncommonElements is null)
                return this;

            var formula = new ChemicalFormula();

            // Handle common elements
            if (_commonElements.HasValue && _commonElementEntities.HasValue && otherFormula._commonElements.HasValue && otherFormula._commonElementEntities.HasValue)
            {
                int[]? newCommonElements = new int[CommonElementLength];
                IElement[]? newCommonElementEntities = new IElement[CommonElementLength];

                ReadOnlySpan<int> thisCommonElements = _commonElements.Value.Span;
                ReadOnlySpan<IElement> thisCommonElementEntities = _commonElementEntities.Value.Span;
                ReadOnlySpan<int> otherCommonElements = otherFormula._commonElements.Value.Span;
                ReadOnlySpan<IElement> otherCommonElementEntities = otherFormula._commonElementEntities.Value.Span;

                for (int i = 0; i < newCommonElements.Length; i++)
                {
                    if (add)
                        newCommonElements[i] = thisCommonElements[i] + otherCommonElements[i];
                    else
                        newCommonElements[i] = thisCommonElements[i] - otherCommonElements[i];

                    // RTF: This is a bit of a code smell. I'm taking the first entity 
                    newCommonElementEntities[i] = thisCommonElementEntities[i] is not null
                        ? thisCommonElementEntities[i]
                        : otherCommonElementEntities[i];
                }

                formula._commonElements = newCommonElements;
                formula._commonElementEntities = newCommonElementEntities;
            }
            else if (_commonElements.HasValue && !otherFormula._commonElements.HasValue)
            {
                formula._commonElements = _commonElements;
                formula._commonElementEntities = _commonElementEntities;
            }
            else if (!_commonElements.HasValue && otherFormula._commonElements.HasValue)
            {
                if (add) // If add, you are safe to use existing collection
                {
                    formula._commonElements = otherFormula._commonElements;
                }
                else // If subtract, you must make a new collection with negative values
                {
                    int[]? newCommonElements = new int[CommonElementLength];

                    ReadOnlySpan<int> otherCommonElements = otherFormula._commonElements.Value.Span;

                    for (int i = 0; i < newCommonElements.Length; i++)
                        newCommonElements[i] = -otherCommonElements[i];
                }

                formula._commonElementEntities = otherFormula._commonElementEntities;
            }

            // Handle uncommon elements
            if (_uncommonElements is not null && otherFormula._uncommonElements is not null)
            {
                Dictionary<IElement, int> combinedElements = new(_uncommonElements.Count + otherFormula._uncommonElements.Count);

                foreach (var element in _uncommonElements)
                    combinedElements.Add(element.Entity, element.Count);

                foreach (var element in otherFormula._uncommonElements)
                {
                    if (combinedElements.ContainsKey(element.Entity))
                    {
                        if (add)
                            combinedElements[element.Entity] += element.Count;
                        else
                            combinedElements[element.Entity] -= element.Count;
                    }
                    else
                    {
                        combinedElements.Add(element.Entity, element.Count);
                    }
                }

                var newUncommon = new List<IEntityCardinality<IElement>>(combinedElements.Count);

                foreach (var pair in combinedElements)
                    newUncommon.Add(new EntityCardinality<IElement>(pair.Key, pair.Value));

                formula._uncommonElements = newUncommon;
            }
            else if (_uncommonElements is not null && otherFormula._uncommonElements is null)
            {
                formula._uncommonElements = _uncommonElements;
            }
            else if (_uncommonElements is null && otherFormula._uncommonElements is not null)
            {
                if (add) // If add, you are safe to use existing collection
                {
                    formula._uncommonElements = otherFormula._uncommonElements;
                }
                else // If subtract, you must make a new collection with negative values
                {
                    var newUncommon = new List<IEntityCardinality<IElement>>(otherFormula._uncommonElements.Count);

                    foreach (var cardinality in otherFormula._uncommonElements)
                        newUncommon.Add(new EntityCardinality<IElement>(cardinality.Entity, -cardinality.Count));

                    formula._uncommonElements = newUncommon;
                }
            }

            return formula;
        }

        /// <summary>
        /// Multiplies the formula by the specified multiplier.
        /// </summary>
        /// <param name="multiplier">The multiplier.</param>
        /// <returns></returns>
        public ChemicalFormula Multiply(int multiplier)
        {
            if (multiplier == 0)
                return Empty;

            if (multiplier == 1)
                return this;

            var formula = new ChemicalFormula();

            if (_commonElements.HasValue)
            {
                var oldCommonElements = _commonElements.Value.Span;
                int[] newCommonElements = new int[CommonElementLength];

                for (int i = 0; i < CommonElementLength; i++)
                    newCommonElements[i] = oldCommonElements[i] * multiplier;

                formula._commonElements = new ReadOnlyMemory<int>(newCommonElements);
                formula._commonElementEntities = _commonElementEntities;
            }

            if (_uncommonElements is not null)
            {
                var newUncommon = new List<IEntityCardinality<IElement>>(_uncommonElements.Count);

                foreach (var cardinality in _uncommonElements)
                    newUncommon.Add(new EntityCardinality<IElement>(cardinality.Entity, cardinality.Count * multiplier));

                formula._uncommonElements = newUncommon;
            }

            return formula;
        }

        /// <summary>Parses the string into a chemical formula.</summary>
        /// <param name="formula">The formula.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not parse '{formula.ToString()}' into a chemical formula.</exception>
        public static ChemicalFormula ParseString(ReadOnlySpan<char> formula, IElementProvider elementProvider)
        {
            if (TryParseString(formula, elementProvider, out ChemicalFormula result))
                return result;

            throw new InvalidChemicalFormula($"Could not parse '{formula.ToString()}' into a chemical formula.");
        }

#if !NETSTANDARD2_1
        /// <summary>Attempts to parse the string into a ChemicalFormula.</summary>
        /// <param name="formula">The chemical formula as a string.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <param name="chemicalFormula">The chemical formula or null if string was not formatted correctly.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool TryParseString(string formula, IElementProvider elementProvider, out ChemicalFormula chemicalFormula)
        {
            return TryParseString(formula.AsSpan(), elementProvider, out chemicalFormula);
        }
#endif

        /// <summary>Attempts to parse the string into a ChemicalFormula.</summary>
        /// <param name="formula">The chemical formula as a string.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <param name="chemicalFormula">The chemical formula or null if string was not formatted correctly.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool TryParseString(ReadOnlySpan<char> formula, IElementProvider elementProvider, out ChemicalFormula chemicalFormula)
        {
            chemicalFormula = Empty; // Set to null in case of failure.

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

#if NETSTANDARD2_1
                if (!int.TryParse(digitSpan, out count)) return false;
#else
                if (!int.TryParse(digitSpan.ToString(), out count)) return false;
#endif

                if (count == 0)
                    return true; // A valid cardinality, but nothing should be added to the formula
            }

            // Handle isotopes
            int? isotope = null;
            if (isotopeStart != 0)
            {
                var isotopeSpan = formula.Slice(isotopeStart, isotopeEnd - isotopeStart + 1);

#if NETSTANDARD2_1
                if (!int.TryParse(isotopeSpan, out int isotopeOut)) return false;
#else
                if (!int.TryParse(isotopeSpan.ToString(), out int isotopeOut)) return false;
#endif

                isotope = isotopeOut;
            }

            try
            {
                string symbol = formula.Slice(symbolStart, symbolEnd - symbolStart + 1).ToString();

                var element = new EntityCardinality<IElement>(elementProvider.GetElement(symbol, isotope), count);

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