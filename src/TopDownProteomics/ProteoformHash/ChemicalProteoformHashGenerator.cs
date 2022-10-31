using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDownProteomics.Biochemistry;
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
                IProteoformGroup simpleGroup = this._proteoformGroupFactory.CreateProteoformGroup(originalProFormaTerm.Sequence);

                return new ChemicalProteoformHash(originalProFormaTerm.Sequence, simpleGroup);
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
            IList<ProFormaDescriptor>? labileDescriptors = null;
            IList<ProFormaUnlocalizedTag>? unlocalizedTags = null;
            IList<ProFormaTagGroup>? tagGroups = null;
            IList<ProFormaGlobalModification>? globalModifications = null;

            if (proteoformGroup.LocalizedModifications?.Count > 0)
            {
                foreach (var mod in proteoformGroup.LocalizedModifications)
                {
                    ProFormaDescriptor? descriptor = this.CreateDescriptor(mod.ModificationDelta);

                    if (descriptor != null)
                    {
                        tags ??= new List<ProFormaTag>();
                        tags.Add(new ProFormaTag(mod.ZeroBasedStartIndex, mod.ZeroBasedEndIndex, new[] { descriptor }));
                    }
                }
            }

            if (proteoformGroup.UnlocalizedModifications?.Count > 0)
            {
                foreach (var mod in proteoformGroup.UnlocalizedModifications)
                {
                    ProFormaDescriptor? descriptor = this.CreateDescriptor(mod.ModificationDelta);

                    if (descriptor != null)
                    {
                        if (mod.IsLabile)
                        {
                            labileDescriptors ??= new List<ProFormaDescriptor>();

                            for (int i = 0; i < mod.Count; i++)
                                labileDescriptors.Add(descriptor);
                        }
                        else
                        {
                            unlocalizedTags ??= new List<ProFormaUnlocalizedTag>();
                            unlocalizedTags.Add(new ProFormaUnlocalizedTag(mod.Count, new[] { descriptor }));
                        }
                    }
                }
            }

            if (proteoformGroup.ModificationGroups?.Count > 0)
            {
                foreach (var mod in proteoformGroup.ModificationGroups)
                {
                    ProFormaDescriptor? descriptor = this.CreateDescriptor(mod.ModificationDelta);

                    if (descriptor != null)
                    {
                        tagGroups ??= new List<ProFormaTagGroup>();
                        tagGroups.Add(new ProFormaTagGroup(mod.GroupName, descriptor.Key, descriptor.EvidenceType, descriptor.Value,
                            mod.Members.Select(x => new ProFormaMembershipDescriptor(x.ZeroBasedStartIndex, x.ZeroBasedEndIndex, x.Weight)).ToList()));
                    }
                }
            }

            if (proteoformGroup.GlobalModifications?.Count > 0)
            {
                foreach (var mod in proteoformGroup.GlobalModifications)
                {
                    ProFormaDescriptor? descriptor = this.CreateDescriptor(mod.ModificationDelta);

                    if (descriptor != null)
                    {
                        globalModifications ??= new List<ProFormaGlobalModification>();
                        globalModifications.Add(new ProFormaGlobalModification(new[] { descriptor }, mod.TargetAminoAcids));
                    }
                }
            }

            string? sequence = proteoformGroup.GetSequence();

            if (sequence != null)
            {
                var proFormaTerm = new ProFormaTerm(sequence, tags: tags?.OrderBy(t => t.Descriptors.First().Value).ToArray(),
                    nTerminalDescriptors: nTermDescriptors,
                    cTerminalDescriptors: cTermDescriptors,
                    labileDescriptors: labileDescriptors,
                    unlocalizedTags: unlocalizedTags,
                    tagGroups: tagGroups,
                    globalModifications: globalModifications);
                string hash = new ProFormaWriter().WriteString(proFormaTerm);
                return new ChemicalProteoformHash(hash, proteoformGroup);
            }

            throw new Exception("Cannot get amino acid sequence for the proteoform group.");
        }

        private ProFormaDescriptor? CreateDescriptor(IProteoformMassDelta? proteoformModification)
        {
            // TODO: Standardize group notations and unlocalized cardinality

            return proteoformModification switch
            {
                null => null,
                IProteoformOntologyDelta ontologyMod => this.GetOntologyDescriptor(ontologyMod),
                IProteoformGlycanCompositionDelta glycanComp => this.GetGlycanCompDescriptor(glycanComp),
                IProteoformFormulaProteoformDelta formulaMod => this.GetFormulaDescriptor(formulaMod),
                _ => new ProFormaDescriptor(ProFormaKey.Mass, this.GetMassString(proteoformModification.GetMass(MassType.Monoisotopic)))
            };
        }

        private ProFormaDescriptor GetOntologyDescriptor(IIdentifiable ontologyMod)
        {
            var (evidence, accession) = _mapper.MapAccession(ontologyMod.Id);

            return new ProFormaDescriptor(ProFormaKey.Identifier, evidence, accession);
        }
        private ProFormaDescriptor GetGlycanCompDescriptor(IHasGlycanComposition hasComposition)
        {
            var composition = hasComposition.GetGlycanComposition();

            StringBuilder sb = new();

            foreach (var residue in composition.GetResidues().OrderBy(x => x.Entity.Symbol))
                sb.Append($"{residue.Entity.Symbol}{(residue.Count > 1 || residue.Count < 0 ? residue.Count : string.Empty)}");

            return new ProFormaDescriptor(ProFormaKey.Glycan, sb.ToString());
        }
        private ProFormaDescriptor GetFormulaDescriptor(IHasChemicalFormula formulaMod)
        {
            return new ProFormaDescriptor(ProFormaKey.Formula, ChemistryUtility.GetChemicalFormulaString(formulaMod.GetChemicalFormula()));
        }
        private string GetMassString(double mass)
        {
            if (mass >= 0.0)
                return $"+{mass:N4}";

            return mass.ToString("N4");
        }

        private class ChemicalProteoformHash : IChemicalProteoformHash, IHasMass
        {
            private IHasMass _hasMass;

            public ChemicalProteoformHash(string hash, IHasMass hasMass)
            {
                Hash = hash;
                _hasMass = hasMass;
            }

            public string Hash { get; }

            public bool HasProForma => true;

            public string ProForma => Hash;

            public double GetMass(MassType massType) => _hasMass.GetMass(massType);
        }
    }
}