using System.Collections.Generic;

namespace TopDownProteomics.Chemistry.Unimod
{
    /// <summary>
    /// Atom provider with a lot of information hard coded.
    /// </summary>
    /// <seealso cref="IUnimodCompositionAtomProvider" />
    public class UnimodHardCodedAtomProvider : IUnimodCompositionAtomProvider
    {
        private Dictionary<string, UnimodCompositionAtom> _atoms;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnimodHardCodedAtomProvider"/> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        public UnimodHardCodedAtomProvider(IElementProvider elementProvider)
        {
            _atoms = this.CreateDictionary(elementProvider);
        }

        private Dictionary<string, UnimodCompositionAtom> CreateDictionary(IElementProvider elementProvider)
        {
            var atoms = new Dictionary<string, UnimodCompositionAtom>();

            var h = elementProvider.GetElement(1);
            var c = elementProvider.GetElement(6);
            var n = elementProvider.GetElement(7);
            var o = elementProvider.GetElement(8);
            var s = elementProvider.GetElement(16);

            this.AddElement(atoms, elementProvider, "13C", "Carbon 13", "C", 13);
            this.AddElement(atoms, elementProvider, "15N", "Nitrogen 15", "N", 15);
            this.AddElement(atoms, elementProvider, "18O", "Oxygen 18", "O", 18);
            this.AddElement(atoms, elementProvider, "2H", "Deuterium", "H", 2);

            atoms.Add("Ac", new UnimodCompositionAtom("Ac", "Acetate", new[]
            {
                new EntityCardinality<IElement>(c, 2),
                new EntityCardinality<IElement>(h, 3),
                new EntityCardinality<IElement>(o, 2),
            }));

            this.AddElement(atoms, elementProvider, "Ag", "Silver");
            this.AddElement(atoms, elementProvider, "Al", "Aluminium");
            this.AddElement(atoms, elementProvider, "As", "Arsenic");
            this.AddElement(atoms, elementProvider, "Au", "Gold");
            this.AddElement(atoms, elementProvider, "B", "Boron");
            this.AddElement(atoms, elementProvider, "Br", "Bromine");
            this.AddElement(atoms, elementProvider, "C", "Carbon");
            this.AddElement(atoms, elementProvider, "Ca", "Calcium");
            this.AddElement(atoms, elementProvider, "Cd", "Cadmium");
            this.AddElement(atoms, elementProvider, "Cl", "Chlorine");
            this.AddElement(atoms, elementProvider, "Co", "Cobalt");
            this.AddElement(atoms, elementProvider, "Cr", "Chromium");
            this.AddElement(atoms, elementProvider, "Cu", "Copper");

            atoms.Add("dHex", new UnimodCompositionAtom("dHex", "Deoxy-hexose", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 4),
            }));

            this.AddElement(atoms, elementProvider, "F", "Fluorine");
            this.AddElement(atoms, elementProvider, "Fe", "Iron");
            this.AddElement(atoms, elementProvider, "H", "Hydrogen");

            atoms.Add("Hep", new UnimodCompositionAtom("Hep", "Heptose", new[]
            {
                new EntityCardinality<IElement>(c, 7),
                new EntityCardinality<IElement>(h, 12),
                new EntityCardinality<IElement>(o, 6),
            }));
            atoms.Add("Hex", new UnimodCompositionAtom("Hex", "Hexose", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 5),
            }));

            // Not sure about C6H8O6
            atoms.Add("HexA", new UnimodCompositionAtom("HexA", "Hexuronic acid", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 8),
                new EntityCardinality<IElement>(o, 6),
            }));

            // Not sure about C6H11O4N
            atoms.Add("HexN", new UnimodCompositionAtom("HexN", "Hexosamine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 11),
                new EntityCardinality<IElement>(o, 4),
                new EntityCardinality<IElement>(n, 1),
            }));

            atoms.Add("HexNAc", new UnimodCompositionAtom("HexNAc", "N-Acetyl Hexosamine", new[]
            {
                new EntityCardinality<IElement>(c, 8),
                new EntityCardinality<IElement>(h, 13),
                new EntityCardinality<IElement>(o, 5),
                new EntityCardinality<IElement>(n, 1),
            }));

            this.AddElement(atoms, elementProvider, "Hg", "Mercury");
            this.AddElement(atoms, elementProvider, "I", "Iodine");
            this.AddElement(atoms, elementProvider, "K", "Potassium");

            // Should be Hex(2) + HexA 
            atoms.Add("Kdn", new UnimodCompositionAtom("Kdn", "3-deoxy-d-glycero-D-galacto-nonulosonic acid", new[]
            {
                new EntityCardinality<IElement>(c, 18),
                new EntityCardinality<IElement>(h, 28),
                new EntityCardinality<IElement>(o, 16),
            }));

            // Never used
            //atoms.Add("Kdo", new UnimodCompositionAtom("13C", "2-keto-3-deoxyoctulosonic acid", new[]
            //{
            //    new EntityCardinality<IElement>(c, 0),
            //    new EntityCardinality<IElement>(h, 0),
            //    new EntityCardinality<IElement>(o, 0),
            //}));

            this.AddElement(atoms, elementProvider, "Li", "Lithium");

            atoms.Add("Me", new UnimodCompositionAtom("Me", "Methyl", new[]
            {
                new EntityCardinality<IElement>(c, 1),
                new EntityCardinality<IElement>(h, 2),
            }));

            this.AddElement(atoms, elementProvider, "Mg", "Magnesium");
            this.AddElement(atoms, elementProvider, "Mn", "Manganese");
            this.AddElement(atoms, elementProvider, "Mo", "Molybdenum");
            this.AddElement(atoms, elementProvider, "N", "Nitrogen");
            this.AddElement(atoms, elementProvider, "Na", "Sodium");

            atoms.Add("NeuAc", new UnimodCompositionAtom("NeuAc", "N-acetyl neuraminic acid", new[]
            {
                new EntityCardinality<IElement>(c, 11),
                new EntityCardinality<IElement>(h, 17),
                new EntityCardinality<IElement>(o, 8),
                new EntityCardinality<IElement>(n, 1),
            }));
            atoms.Add("NeuGc", new UnimodCompositionAtom("NeuGc", "N-glycoyl neuraminic acid", new[]
            {
                new EntityCardinality<IElement>(c, 11),
                new EntityCardinality<IElement>(h, 17),
                new EntityCardinality<IElement>(o, 9),
                new EntityCardinality<IElement>(n, 1),
            }));

            this.AddElement(atoms, elementProvider, "Ni", "Nickel");
            this.AddElement(atoms, elementProvider, "O", "Oxygen");
            this.AddElement(atoms, elementProvider, "P", "Phosphorous");
            this.AddElement(atoms, elementProvider, "Pd", "Palladium");

            atoms.Add("Pent", new UnimodCompositionAtom("Pent", "Pentose", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 8),
                new EntityCardinality<IElement>(o, 4),
            }));

            // Never used
            //atoms.Add("Phos", new UnimodCompositionAtom("13C", "Phosphate", new[]
            //{
            //    new EntityCardinality<IElement>(c, 0),
            //    new EntityCardinality<IElement>(h, 0),
            //    new EntityCardinality<IElement>(o, 0),
            //    new EntityCardinality<IElement>(n, 0),
            //}));

            this.AddElement(atoms, elementProvider, "Pt", "Platinum");
            this.AddElement(atoms, elementProvider, "Ru", "Ruthenium");
            this.AddElement(atoms, elementProvider, "S", "Sulphur");
            this.AddElement(atoms, elementProvider, "Se", "Selenium");

            atoms.Add("Sulf", new UnimodCompositionAtom("Sulf", "Sulphate", new[]
            {
                new EntityCardinality<IElement>(s, 1),
                new EntityCardinality<IElement>(o, 3),
            }));
            atoms.Add("Water", new UnimodCompositionAtom("Water", "Water", new[]
            {
                new EntityCardinality<IElement>(h, 2),
                new EntityCardinality<IElement>(o, 1),
            }));
            this.AddElement(atoms, elementProvider, "Zn", "Zinc");

            return atoms;
        }

        private void AddElement(Dictionary<string, UnimodCompositionAtom> atoms, IElementProvider elementProvider,
            string symbol, string name)
        {
            IElement element = elementProvider.GetElement(symbol);
            atoms.Add(symbol, new UnimodCompositionAtom(symbol, name, new[]
            {
                new EntityCardinality<IElement>(element, 1)
            }));
        }

        private void AddElement(Dictionary<string, UnimodCompositionAtom> atoms, IElementProvider elementProvider,
            string symbol, string name, string elementSymbol, int? fixedIsotopeNumber = null)
        {
            IElement element = elementProvider.GetElement(elementSymbol, fixedIsotopeNumber);
            atoms.Add(symbol, new UnimodCompositionAtom(symbol, name, new[]
            {
                new EntityCardinality<IElement>(element, 1)
            }));
        }

        /// <summary>
        /// Gets the unimod composition atom.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        public UnimodCompositionAtom GetUnimodCompositionAtom(string symbol)
        {
            return _atoms[symbol];
        }
    }
}