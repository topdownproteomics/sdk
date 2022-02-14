using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;

namespace TopDownProteomics.Biochemistry
{
    /// <summary>
    /// Provider with 22 IUPAC amino acids hard coded.
    /// </summary>
    /// <seealso cref="IResidueProvider" />
    public class IupacAminoAcidProvider : IResidueProvider
    {
        IResidue[] _aminoAcids = new IResidue[26];

        /// <summary>
        /// Initializes a new instance of the <see cref="IupacAminoAcidProvider" /> class.
        /// </summary>
        /// <param name="elementProvider">The element provider.</param>
        public IupacAminoAcidProvider(IElementProvider elementProvider)
        {
            _aminoAcids = this.CreateResideArray(elementProvider);
        }

        /// <summary>
        /// Gets the residue.
        /// </summary>
        /// <param name="symbol">The symbol of the residue.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IResidue GetResidue(char symbol)
        {
            if (!char.IsUpper(symbol))
                throw new ArgumentException("Not an upper case amino acid symbol", nameof(symbol));

            return _aminoAcids[symbol - 65];
        }

        /// <summary>
        /// Gets all resiudes.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IResidue> GetResidues() => _aminoAcids.Where(x => x != null);

        private IResidue[] CreateResideArray(IElementProvider elementProvider)
        {
            var aaMass = new AminoAcid[26];

            var h = elementProvider.GetElement(1);
            var c = elementProvider.GetElement(6);
            var n = elementProvider.GetElement(7);
            var o = elementProvider.GetElement(8);
            var s = elementProvider.GetElement(16);

            aaMass[0] = new AminoAcid('A', "Alanine", new[]
            {
                new EntityCardinality<IElement>(c, 3),
                new EntityCardinality<IElement>(h, 5),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[17] = new AminoAcid('R', "Arginine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 12),
                new EntityCardinality<IElement>(n, 4),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[13] = new AminoAcid('N', "Asparagine", new[]
            {
                new EntityCardinality<IElement>(c, 4),
                new EntityCardinality<IElement>(h, 6),
                new EntityCardinality<IElement>(n, 2),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[3] = new AminoAcid('D', "Aspartic Acid", new[]
            {
                new EntityCardinality<IElement>(c, 4),
                new EntityCardinality<IElement>(h, 5),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 3),
            });
            aaMass[2] = new AminoAcid('C', "Cysteine", new[]
            {
                new EntityCardinality<IElement>(c, 3),
                new EntityCardinality<IElement>(h, 5),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
                new EntityCardinality<IElement>(s, 1),
            });
            aaMass[4] = new AminoAcid('E', "Glutamic acid", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 7),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 3),
            });
            aaMass[16] = new AminoAcid('Q', "Glutamine", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 8),
                new EntityCardinality<IElement>(n, 2),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[6] = new AminoAcid('G', "Glycine", new[]
            {
                new EntityCardinality<IElement>(c, 2),
                new EntityCardinality<IElement>(h, 3),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[7] = new AminoAcid('H', "Histidine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 7),
                new EntityCardinality<IElement>(n, 3),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[8] = new AminoAcid('I', "Isoleucine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 11),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[11] = new AminoAcid('L', "Leucine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 11),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[10] = new AminoAcid('K', "Lysine", new[]
            {
                new EntityCardinality<IElement>(c, 6),
                new EntityCardinality<IElement>(h, 12),
                new EntityCardinality<IElement>(n, 2),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[12] = new AminoAcid('M', "Methionine", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 9),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
                new EntityCardinality<IElement>(s, 1),
            });
            aaMass[5] = new AminoAcid('F', "Phenylalanine", new[]
            {
                new EntityCardinality<IElement>(c, 9),
                new EntityCardinality<IElement>(h, 9),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[15] = new AminoAcid('P', "Proline", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 7),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[14] = new AminoAcid('O', "Pyrrolysine", new[]
            {
                new EntityCardinality<IElement>(c, 12),
                new EntityCardinality<IElement>(h, 19),
                new EntityCardinality<IElement>(n, 3),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[20] = new AminoAcid('U', "Selenocysteine", new[]
            {
                new EntityCardinality<IElement>(c, 3),
                new EntityCardinality<IElement>(h, 5),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
                new EntityCardinality<IElement>(elementProvider.GetElement("Se"), 1),
            });
            aaMass[18] = new AminoAcid('S', "Serine", new[]
            {
                new EntityCardinality<IElement>(c, 3),
                new EntityCardinality<IElement>(h, 5),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[19] = new AminoAcid('T', "Threonine", new[]
            {
                new EntityCardinality<IElement>(c, 4),
                new EntityCardinality<IElement>(h, 7),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[22] = new AminoAcid('W', "Tryptophan", new[]
            {
                new EntityCardinality<IElement>(c, 11),
                new EntityCardinality<IElement>(h, 10),
                new EntityCardinality<IElement>(n, 2),
                new EntityCardinality<IElement>(o, 1),
            });
            aaMass[24] = new AminoAcid('Y', "Tyrosine", new[]
            {
                new EntityCardinality<IElement>(c, 9),
                new EntityCardinality<IElement>(h, 9),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 2),
            });
            aaMass[21] = new AminoAcid('V', "Valine", new[]
            {
                new EntityCardinality<IElement>(c, 5),
                new EntityCardinality<IElement>(h, 9),
                new EntityCardinality<IElement>(n, 1),
                new EntityCardinality<IElement>(o, 1),
            });

            return aaMass;
        }

        private class AminoAcid : IResidue
        {
            private IReadOnlyCollection<IEntityCardinality<IElement>> _elements;

            public AminoAcid(char symbol, string name, IReadOnlyCollection<IEntityCardinality<IElement>> elements)
            {
                _elements = elements;
                this.Name = name;
                this.Symbol = symbol;
            }

            public string Name { get; }
            public char Symbol { get; }

            public IChemicalFormula GetChemicalFormula() => new ChemicalFormula(_elements);

            public double GetMass(MassType massType) => this.GetChemicalFormula().GetMass(massType);
        }
    }
}
