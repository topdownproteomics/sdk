using System;
using System.Collections.Generic;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>A hard coded provider of glycan residues.</summary>
    /// <seealso cref="IGlycanResidueProvider" />
    public class HardCodedGlycanResidueProvider : IGlycanResidueProvider
    {
        private Dictionary<string, IGlycanResidue> _residues;

        /// <summary>
        /// Initializes a new instance of the <see cref="HardCodedGlycanResidueProvider"/> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        public HardCodedGlycanResidueProvider(IElementProvider elementProvider)
        {
            _residues = this.CreateDictionary(elementProvider);
        }

        private Dictionary<string, IGlycanResidue> CreateDictionary(IElementProvider elementProvider)
        {
            var atoms = new Dictionary<string, IGlycanResidue>();

            var h = elementProvider.GetElement(1);
            var c = elementProvider.GetElement(6);
            var n = elementProvider.GetElement(7);
            var o = elementProvider.GetElement(8);
            var s = elementProvider.GetElement(16);
            var p = elementProvider.GetElement(15);

            atoms.Add("Hex", new GlycanResidue("Hex", "Hexose", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 5),
            }));
            atoms.Add("HexNAc", new GlycanResidue("HexNAc", "N-Acetyl Hexose", new[]
            {
                new EntityCardinality<IElement>(c, 8),
                new EntityCardinality<IElement>(h, 13),
                new EntityCardinality<IElement>(o, 5),
                new EntityCardinality<IElement>(n, 1),
            }));
            atoms.Add("HexS", new GlycanResidue("HexS", "Hexose Sulfate", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 8),
                new EntityCardinality<IElement>(s, 1),
            }));
            atoms.Add("HexP", new GlycanResidue("HexP", "Hexose Phosphate", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 11),
                new EntityCardinality<IElement>(o, 8),
                new EntityCardinality<IElement>(p, 1),
            }));
            atoms.Add("HexNAcS", new GlycanResidue("HexNAcS", "N-Acetyl Hexose Sulfate", new[]
            {
                new EntityCardinality<IElement>(c, 8),
                new EntityCardinality<IElement>(h, 13),
                new EntityCardinality<IElement>(o, 8),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(s, 1),
            }));
            atoms.Add("dHex", new GlycanResidue("dHex", "Deoxy-Hexose", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 4),
            }));
            atoms.Add("NeuAc", new GlycanResidue("NeuAc", "N-acetyl Neuraminic Acid", new[]
            {
                new EntityCardinality<IElement>(c, 11),
                new EntityCardinality<IElement>(h, 17),
                new EntityCardinality<IElement>(o, 8),
                new EntityCardinality<IElement>(n, 1),
            }));
            atoms.Add("NeuGc", new GlycanResidue("NeuGc", "N-glycoyl Neuraminic Acid", new[]
            {
                new EntityCardinality<IElement>(c, 11),
                new EntityCardinality<IElement>(h, 17),
                new EntityCardinality<IElement>(o, 9),
                new EntityCardinality<IElement>(n, 1),
            }));
            atoms.Add("Pen", new GlycanResidue("Pen", "Pentose", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 8),
                new EntityCardinality<IElement>(o, 4),
            }));
            atoms.Add("Fuc", new GlycanResidue("Fuc", "Fuctose", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(o, 4),
            }));

            return atoms;
        }

        /// <summary>
        /// Gets the glycan residue for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Could not find glycan with symbol '{symbol}'.</exception>
        public IGlycanResidue GetGlycanResidue(string symbol)
        {
            if (_residues.ContainsKey(symbol)) return _residues[symbol];

            throw new Exception($"Could not find glycan with symbol '{symbol}'.");
        }
    }
}