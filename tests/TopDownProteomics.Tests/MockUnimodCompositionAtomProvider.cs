using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;

namespace TopDownProteomics.Tests
{
    public class MockUnimodCompositionAtomProvider : IUnimodCompositionAtomProvider
    {
        private readonly IElementProvider _elementProvider;
        private readonly UnimodCompositionAtom _hydrogen;
        private readonly UnimodCompositionAtom _carbon;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockUnimodCompositionAtomProvider"/> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        public MockUnimodCompositionAtomProvider(IElementProvider elementProvider)
        {
            _elementProvider = elementProvider;
            _hydrogen = new UnimodCompositionAtom("H", "Hydrogen", new[]
            {
                new EntityCardinality<IElement>(_elementProvider.GetElement(1), 1)
            });
            _carbon = new UnimodCompositionAtom("C", "Carbon", new[]
            {
                new EntityCardinality<IElement>(_elementProvider.GetElement(6), 1)
            });
        }

        /// <summary>
        /// Gets the unimod composition atom.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public UnimodCompositionAtom GetUnimodCompositionAtom(string symbol)
        {
            if (symbol == "H")
            {
                return _hydrogen;
            }
            else if (symbol == "C")
            {
                return _carbon;
            }

            return null;
        }
    }
}