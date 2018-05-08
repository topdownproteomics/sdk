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
        public ChemicalFormula()
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
    }
}