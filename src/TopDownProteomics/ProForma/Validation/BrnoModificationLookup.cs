using System.Collections.Generic;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for Brno nomenclature for histone modifications.
    /// https://doi.org/10.1038/nsmb0205-110
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class BrnoModificationLookup : IProteoformModificationLookup
    {
        IElementProvider elementProvider;
        BrnoModification[] _modifications;

        /// <summary>
        /// Initializes a new instance of the <see cref="BrnoModificationLookup"/> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        public BrnoModificationLookup(IElementProvider elementProvider)
        {
            this.elementProvider = elementProvider;
            _modifications = this.CreateModificationArray(elementProvider);
        }

        /// <summary>
        /// Determines whether this instance [can handle descriptor] the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        /// <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandleDescriptor(ProFormaDescriptor descriptor)
        {
            return descriptor.Key == ProFormaKey.KnownModificationName && descriptor.Value != null && descriptor.Value.EndsWith("(BRNO)");
        }

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification GetModification(ProFormaDescriptor descriptor)
        {
            string abbreviation = descriptor.Value.Substring(0, descriptor.Value.IndexOf("("));

            switch (abbreviation)
            {
                case "ac": return _modifications[0];
                case "me1": return _modifications[1];
                case "me2s": return _modifications[2];
                case "me2a": return _modifications[3];
                case "me2": return _modifications[4];
                case "me3": return _modifications[5];
                case "ph": return _modifications[6];

                default:
                    throw new ProteoformModificationLookupException($"Couldn't handle value for descriptor {descriptor.ToString()}.");
            }
        }

        private BrnoModification[] CreateModificationArray(IElementProvider elementProvider)
        {
            var mods = new BrnoModification[7];

            var h = elementProvider.GetElement(1);
            var c = elementProvider.GetElement(6);
            var o = elementProvider.GetElement(8);
            var p = elementProvider.GetElement(15);

            mods[0] = new BrnoModification("ac", new[]
            {
                new EntityCardinality<IElement>(c, 2),
                new EntityCardinality<IElement>(h, 2),
                new EntityCardinality<IElement>(o, 1)
            });
            mods[1] = new BrnoModification("me1", new[]
            {
                new EntityCardinality<IElement>(c, 1),
                new EntityCardinality<IElement>(h, 2)
            });

            var me2 = new[]
            {
                new EntityCardinality<IElement>(c, 2),
                new EntityCardinality<IElement>(h, 4)
            };
            mods[2] = new BrnoModification("me2s", me2);
            mods[3] = new BrnoModification("me2a", me2);
            mods[4] = new BrnoModification("me2", me2);

            mods[5] = new BrnoModification("me3", new[]
            {
                new EntityCardinality<IElement>(c, 3),
                new EntityCardinality<IElement>(h, 6)
            });
            mods[6] = new BrnoModification("ph", new[]
            {
                new EntityCardinality<IElement>(h, 1),
                new EntityCardinality<IElement>(o, 3),
                new EntityCardinality<IElement>(p, 1),
            });

            return mods;
        }

        private class BrnoModification : IProFormaProteoformModification
        {
            public BrnoModification(string abbreviation, IReadOnlyCollection<IEntityCardinality<IElement>> elements)
            {
                this.Abbreviation = abbreviation;
                _elements = elements;
            }

            private IReadOnlyCollection<IEntityCardinality<IElement>> _elements;
            string Abbreviation { get; }

            public IChemicalFormula GetChemicalFormula() => new ChemicalFormula(_elements);

            public ProFormaDescriptor GetProFormaDescriptor()
            {
                return new ProFormaDescriptor(ProFormaKey.Brno, this.Abbreviation);
            }
        }
    }
}