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

            IReadOnlyCollection<IEntityCardinality<IElement>> otherElements = other.GetElements();

            if (_elements.Count != otherElements.Count)
                return false;

            if (_elements.Sum(x => x.Count) != otherElements.Sum(x => x.Count))
                return false;

            foreach (var element in _elements)
            {
                var otherElement = otherElements.SingleOrDefault(x => x.Entity.Equals(element.Entity));

                if (otherElement == null)
                    return false;

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
            if (!(obj is IChemicalFormula))
                return false;

            return this.Equals(obj as IChemicalFormula);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return _elements.GetHashCode();
        }

        /// <summary>
        /// Adds the specified formula.
        /// </summary>
        /// <param name="otherFormula">The formula.</param>
        /// <returns></returns>
        public IChemicalFormula Add(IChemicalFormula otherFormula)
        {
            if (otherFormula == null)
                return this;

            List<IEntityCardinality<IElement>> otherElements = otherFormula.GetElements().ToList();

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
                    formula._elements.Add(new EntityCardinality<IElement>(element.Entity, element.Count + otherElement.Count));
                    otherElements.Remove(otherElement);
                }
            }

            // Add unique things from other formula
            foreach (var otherElement in otherElements)
                formula._elements.Add(otherElement);

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

        /// <summary>  Attempts to parses the string into a CHemicalFormula.  The string must use the Unimod format.</summary>
        /// <param name="formula">The chemical formula. as a string</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <param name="chemicalFormula">The chemical formula or null if string was not formatted correctly.</param>
        /// <returns>True if successful, otherwise false.</returns>
        public static bool TryParseString(string formula, IElementProvider elementProvider, out ChemicalFormula chemicalFormula)
        {
            chemicalFormula = null; // Set to null in case of failure.

            IList<IEntityCardinality<IElement>> elementList = new List<IEntityCardinality<IElement>>();

            string[] elements = formula.Split(' ');

            foreach (string element in elements)
            {
                int index = 0;
                // Element symbol is first.
                string symbol = "";
                while (index < element.Length && char.IsLetter(element[index]))
                {
                    symbol += element[index];
                    index++;
                }

                if (index >= element.Length) // No count given, so it must be 1.
                {
                    IEntityCardinality<IElement> elementAndCount = GetElement(symbol, 1, elementProvider);
                    if (elementAndCount == null)
                    {
                        return false; // Could happen if the symbol found was an empty string.
                    }
                    else
                    {
                        elementList.Add(elementAndCount);
                    }
                }
                else
                {
                    string countString = "";

                    // There should be a left paren next, ignore it.
                    if (element[index] == '(')
                    {
                        index++;
                    }
                    else
                    {
                        return false;
                    }

                    // Next may be a minus sign.
                    if (element[index] == '-')
                    {
                        countString += '-';
                    }

                    // Rest should be digits and a right paren.  Ignore the right paren.
                    while (index < element.Length && char.IsDigit(element[index]))
                    {
                        countString += element[index];
                        index++;
                    }

                    int count;
                    if (int.TryParse(countString, out count))
                    {
                        elementList.Add(GetElement(symbol, count, elementProvider));
                    }
                    else
                    {
                        return false; // Parsing the count failed.
                    }

                    // Finally there should be a right paren.
                    if (element[index] == ')')
                    {
                        index++;
                    }
                    else
                    {
                        return false;
                    }

                    // This should be the end.
                    if (index != element.Length)
                    {
                        return false;
                    }
                }
            }

            // Success!
            chemicalFormula = new ChemicalFormula(elementList);
            return true;
        }

        private static IEntityCardinality<IElement> GetElement(string symbol, int count, IElementProvider elementProvider)
        {
            return new EntityCardinality<IElement>(elementProvider.GetElement(symbol), count);
        }
    }
}