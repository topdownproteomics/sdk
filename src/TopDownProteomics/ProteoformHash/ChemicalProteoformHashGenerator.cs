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
    /// <remarks>
    /// The basic idea is to handle 4 different levels of "hierarchy" and do some mapping.
    ///  1. Structural based modifications (e.g. symmetric dimethylation) -> map these to their proper term (usually PSI-MOD)
    ///  2. Named modifications (e.g. acetylation)                        -> map these to their proper term (usually PSI-MOD)
    ///  3. Chemical formulas                                             -> standardize expression (don't map to term)
    ///  4. Mass Differences                                              -> remove leading and trailing zeros (don't map to term)
    /// 
    /// One must also standardize the way group and unlocalized notations are written.
    /// Lastly, INFO tags should be stripped.
    /// </remarks>
    public class ChemicalProteoformHashGenerator : IChemicalProteoformHashGenerator
    {
        private ProFormaParser _proFormaParser;
        private ProteoformGroupFactory _proteoformGroupFactory;
        private IProteoformModificationLookup _proteoformModificationLookup;
        private IAccessionMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChemicalProteoformHashGenerator" /> class.
        /// </summary>
        /// <param name="proFormaParser">The proForma parser.</param>
        /// <param name="proteoformGroupFactory">The proteoform group factory.</param>
        /// <param name="proteoformModificationLookup">The proteoform modification lookup.</param>
        /// <param name="mapper">The mapper.</param>
        public ChemicalProteoformHashGenerator(ProFormaParser proFormaParser, ProteoformGroupFactory proteoformGroupFactory,
            IProteoformModificationLookup proteoformModificationLookup, IAccessionMapper mapper)
        {
            _proFormaParser = proFormaParser;
            _proteoformGroupFactory = proteoformGroupFactory;
            _proteoformModificationLookup = proteoformModificationLookup;
            _mapper = mapper;
        }

        /// <summary>Generates a chemical proteoform hash for the specified proteoform.</summary>
        /// <param name="proForma">The ProForma.</param>
        /// <returns></returns>
        public IChemicalProteoformHash Generate(string proForma)
        {
            // Parse string into term
            ProFormaTerm originalProFormaTerm = this._proFormaParser.ParseString(proForma);

            // Check to see if this is only a sequence
            if (originalProFormaTerm.NTerminalDescriptors == null &&
                originalProFormaTerm.CTerminalDescriptors == null &&
                originalProFormaTerm.Tags == null &&
                originalProFormaTerm.GlobalModifications == null &&
                originalProFormaTerm.LabileDescriptors == null &&
                originalProFormaTerm.TagGroups == null &&
                originalProFormaTerm.UnlocalizedTags == null)
            {
                return new ChemicalProteoformHash(originalProFormaTerm.Sequence);
            }

            // Create proteoform group (flattens all features into Ids)
            IProteoformGroup proteoformGroup = this._proteoformGroupFactory.CreateProteoformGroup(originalProFormaTerm,
                this._proteoformModificationLookup);

            ProFormaDescriptor? nTermDescriptor =
                this.CreateDescriptor(proteoformGroup.NTerminalModification);
            IList<ProFormaDescriptor>? nTermDescriptors = nTermDescriptor == null ? null : new[] { nTermDescriptor };

            ProFormaDescriptor? cTermDescriptor =
                this.CreateDescriptor(proteoformGroup.CTerminalModification);
            IList<ProFormaDescriptor>? cTermDescriptors = cTermDescriptor == null ? null : new[] { cTermDescriptor };

            IList<ProFormaTag>? tags = null;
            if (proteoformGroup.Modifications?.Count > 0)
            {
                tags = new List<ProFormaTag>();
                foreach (IProteoformModificationWithIndex proteoformModificationWithIndex in proteoformGroup.Modifications)
                {
                    ProFormaDescriptor? descriptor = this.CreateDescriptor(proteoformModificationWithIndex.Modification);

                    if (descriptor != null)
                        tags.Add(new ProFormaTag(proteoformModificationWithIndex.ZeroBasedIndex, new[] { descriptor }));
                }
            }

            string? sequence = proteoformGroup.GetSequence();

            if (sequence != null)
            {
                ProFormaTerm proFormaTerm = new ProFormaTerm(sequence, tags: tags?.OrderBy(t => t.Descriptors.First().Value).ToArray(),
                    nTerminalDescriptors: nTermDescriptors, cTerminalDescriptors: cTermDescriptors);
                string hash = new ProFormaWriter().WriteString(proFormaTerm);
                return new ChemicalProteoformHash(hash);
            }

            throw new Exception("Cannot get amino acid sequence for the proteoform group.");
        }

        private ProFormaDescriptor? CreateDescriptor(IProteoformModification? proteoformModification)
        {
            // TODO: Standardize group notations and unlocalized cardinality

            return proteoformModification switch
            {
                null => null,
                IIdentifiable ontologyMod => this.GetOntologyDescriptor(ontologyMod),
                IHasChemicalFormula formulaMod => this.GetFormulaDescriptor(formulaMod),
                _ => new ProFormaDescriptor(ProFormaKey.Mass, this.GetMassString(proteoformModification.GetMass(MassType.Monoisotopic)))
            };
        }

        private ProFormaDescriptor GetOntologyDescriptor(IIdentifiable ontologyMod)
        {
            var (evidence, accession) = _mapper.MapAccession(ontologyMod.Id);

            return new ProFormaDescriptor(ProFormaKey.Identifier, evidence, accession);
        }
        private ProFormaDescriptor GetFormulaDescriptor(IHasChemicalFormula formulaMod)
        {
            return new ProFormaDescriptor(ProFormaKey.Formula, ChemistryUtility.GetChemicalFormulaString(formulaMod.GetChemicalFormula()));
        }
        private string GetMassString(double mass)
        {
            if (mass >= 0.0)
                return $"+{mass}";

            return mass.ToString();
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