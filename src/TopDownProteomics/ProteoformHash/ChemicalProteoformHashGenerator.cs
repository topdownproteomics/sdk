using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>Creates a chemical proteoform hash from a proteoform group.</summary>
    public class ChemicalProteoformHashGenerator : IChemicalProteoformHashGenerator
    {
        private ProFormaParser _proFormaParser;
        private ProteoformGroupFactory _proteoformGroupFactory;
        private IProteoformModificationLookup _proteoformModificationLookup;

        /// <summary>Initializes a new instance of the <see cref="ChemicalProteoformHashGenerator"/> class.</summary>
        /// <param name="proFormaParser">The proForma parser.</param>
        /// <param name="proteoformGroupFactory">The proteoform group factory.</param>
        /// <param name="proteoformModificationLookup">The proteoform modification lookup.</param>
        public ChemicalProteoformHashGenerator(ProFormaParser proFormaParser, ProteoformGroupFactory proteoformGroupFactory, IProteoformModificationLookup proteoformModificationLookup)
        {
            _proFormaParser = proFormaParser;
            _proteoformGroupFactory = proteoformGroupFactory;
            _proteoformModificationLookup = proteoformModificationLookup;
        }

        /// <summary>Generates a chemical proteoform hash for the specified proteoform.</summary>
        /// <param name="proForma">The ProForma.</param>
        /// <returns></returns>
        public IChemicalProteoformHash Generate(string proForma)
        {
            ProFormaTerm originalProFormaTerm = this._proFormaParser.ParseString(proForma);
            IProteoformGroup proteoformGroup = this._proteoformGroupFactory.CreateProteoformGroup(originalProFormaTerm, this._proteoformModificationLookup);

            if (proteoformGroup == null)
            {
                throw new ArgumentNullException(nameof(proteoformGroup));
            }

            ProFormaDescriptor? nTermDescriptor =
                this.GetFormulaDescriptor(proteoformGroup.NTerminalModification);
            IList<ProFormaDescriptor>? nTermDescriptors = nTermDescriptor == null ? null : new[] { nTermDescriptor };

            ProFormaDescriptor? cTermDescriptor =
                this.GetFormulaDescriptor(proteoformGroup.CTerminalModification);
            IList<ProFormaDescriptor>? cTermDescriptors = cTermDescriptor == null ? null : new[] { cTermDescriptor };

            IList<ProFormaTag>? tags = null;
            if (proteoformGroup.Modifications?.Count > 0)
            {
                tags = new List<ProFormaTag>();
                foreach (IProteoformModificationWithIndex proteoformModificationWithIndex in proteoformGroup.Modifications)
                {
                    ProFormaDescriptor? descriptor = this.GetFormulaDescriptor(proteoformModificationWithIndex);
                    
                    if (descriptor != null)
                        tags.Add(new ProFormaTag(proteoformModificationWithIndex.ZeroBasedIndex, new[] { descriptor }));
                }
            }

            string? sequence = proteoformGroup.GetSequence();

            if (sequence != null)
            {
                ProFormaTerm proFormaTerm = new ProFormaTerm(sequence, tags?.OrderBy(t => t.Descriptors.First().Value).ToArray(),
                    nTermDescriptors, cTermDescriptors);
                string hash = new ProFormaWriter().WriteString(proFormaTerm);
                return new ChemicalProteoformHash(hash);
            }

            throw new Exception("Cannot get amino acid sequence for the proteoform group.");
        }

        private ProFormaDescriptor? GetFormulaDescriptor(IProteoformModification? proteoformModification)
        {
            return proteoformModification == null
                    ? null
                    : new ProFormaDescriptor(ProFormaKey.Formula, ChemistryUtility.GetChemicalFormulaString(proteoformModification.GetChemicalFormula()));
        }

        private class ChemicalProteoformHash : IChemicalProteoformHash
        {
            public ChemicalProteoformHash(string hash)
            {
                Hash = hash;
            }

            public string Hash { get; }

            public bool HasProForma => true;

            public string ProForma => Hash;
        }
    }
}