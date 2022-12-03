using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.Proteomics;
using TopDownProteomics.Tests.IO;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class ProFormaValidationTests
    {
        ProteoformGroupFactory _factory;
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;
        IProteoformModificationLookup _residLookup;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            _residueProvider = new IupacAminoAcidProvider(_elementProvider);
            _factory = new ProteoformGroupFactory(_elementProvider, _residueProvider);

            var parser = new ResidXmlParser();
            var modifications = parser.Parse(ResidXmlParserTest.GetResidFilePath()).ToArray();

            _residLookup = ResidModificationLookup.CreateFromModifications(modifications.Where(x => x.Id == "AA0038" || x.Id == "AA0074"),
                _elementProvider);
        }

        [Test]
        public void HandleModificationNameTag()
        {
            const string sequence = "SEQVENCE";
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            var term = new ProFormaTerm(sequence, tags: new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { this.CreateBrnoDescriptor("ac") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNotNull(proteoform.LocalizedModifications);
            Assert.AreEqual(1, proteoform.LocalizedModifications.Count);
            Assert.AreEqual(3, ((IProteoformLocalizedModification)proteoform.LocalizedModifications.Single()).ZeroBasedStartIndex);

            // Residue masses plus modification plus water (approx)
            Assert.AreEqual(978.36, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(978.98, proteoform.GetMass(MassType.Average), 0.01);
        }

        [Test]
        public void HandleTerminalModificationNameTag()
        {
            const string sequence = "SEQVENCE";
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            ProFormaDescriptor descriptor = this.CreateBrnoDescriptor("ac");
            var term = new ProFormaTerm(sequence, null, new[] { descriptor }, null, null);
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNull(proteoform.LocalizedModifications);
            Assert.IsNotNull(proteoform.NTerminalModification);
            Assert.IsNull(proteoform.CTerminalModification);
            
            // Residue masses plus modification plus water (approx)
            Assert.AreEqual(978.36, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(978.98, proteoform.GetMass(MassType.Average), 0.01);

            // C terminal case
            term = new ProFormaTerm(sequence, null, null, new[] { descriptor }, null);
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNull(proteoform.LocalizedModifications);
            Assert.IsNull(proteoform.NTerminalModification);
            Assert.IsNotNull(proteoform.CTerminalModification);

            // Residue masses plus modification plus water (approx)
            Assert.AreEqual(978.36, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(978.98, proteoform.GetMass(MassType.Average), 0.01);
        }

        [Test]
        public void HandleBadModificationName()
        {
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            var term = new ProFormaTerm("SEQVENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { this.CreateBrnoDescriptor("wrong") })
            });
            Assert.Throws<ProteoformModificationLookupException>(() => _factory.CreateProteoformGroup(term, modificationLookup));
        }

        [Test]
        public void MultipleModsOneSite()
        {
            var modificationLookup = new CompositeModificationLookup(new IProteoformModificationLookup[]
            {
                new IgnoreKeyModificationLookup(ProFormaKey.Mass),
                new IgnoreKeyModificationLookup(ProFormaKey.Info),
                new BrnoModificationLookup(_elementProvider),
                _residLookup
            });

            // Modifications have same chemical formula ... OK
            var term = new ProFormaTerm("SEQVKENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    this.CreateBrnoDescriptor("ph"),
                    new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0038")
                })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNotNull(proteoform.LocalizedModifications);
            Assert.AreEqual(1, proteoform.LocalizedModifications.Count);
            Assert.AreEqual(4, ((IProteoformLocalizedModification)proteoform.LocalizedModifications.Single()).ZeroBasedStartIndex);

            // Modifications have different chemical formulas ... throw!
            term = new ProFormaTerm("SEQVKENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    this.CreateBrnoDescriptor("me1"),
                    this.CreateBrnoDescriptor("ac")
                })
            });
            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, modificationLookup));

            // What about different mods at different indexes?
            term = new ProFormaTerm("SEQVKENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    this.CreateBrnoDescriptor("ac")
                }),
                new ProFormaTag(7, new[]
                {
                    this.CreateBrnoDescriptor("me1"),
                })
            });
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNotNull(proteoform.LocalizedModifications);
            Assert.AreEqual(2, proteoform.LocalizedModifications.Count);

            // What about descriptors that don't have chemical formulas?
            term = new ProFormaTerm("SEQVKENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(7, new[]
                {
                    this.CreateBrnoDescriptor("me1"),
                    new ProFormaDescriptor(ProFormaKey.Info, "hello!")
                })
            });
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNotNull(proteoform.LocalizedModifications);
            Assert.AreEqual(1, proteoform.LocalizedModifications.Count);
            Assert.AreEqual(7, ((IProteoformLocalizedModification)proteoform.LocalizedModifications.Single()).ZeroBasedStartIndex);

            // Multiple N terminal mods.
            term = new ProFormaTerm("SEQVKENCE", null,
                new[]
                {
                    this.CreateBrnoDescriptor("ph"),
                    new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0038")
                }, null, null
            );
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNull(proteoform.LocalizedModifications);
            Assert.IsNotNull(proteoform.NTerminalModification);

            term = new ProFormaTerm("SEQVKENCE", null,
                new[]
                {
                    this.CreateBrnoDescriptor("me1"),
                    this.CreateBrnoDescriptor("ac")
                }, null, null
            );
            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, modificationLookup));

            // Multiple C terminal mods.
            term = new ProFormaTerm("SEQVKENCE", null, null,
                new[]
                {
                    this.CreateBrnoDescriptor("ph"),
                    new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0038")
                }, null
            );
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNull(proteoform.LocalizedModifications);
            Assert.IsNotNull(proteoform.CTerminalModification);

            term = new ProFormaTerm("SEQVKENCE", null, null,
                new[]
                {
                    this.CreateBrnoDescriptor("me1"),
                    this.CreateBrnoDescriptor("ac")
                }, null
            );
            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, modificationLookup));
        }

        [Test]
        public void HandleDatabaseAccessionTag()
        {
            var term = new ProFormaTerm("SEQVENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0038") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, _residLookup);

            Assert.IsNotNull(proteoform.LocalizedModifications);
            
            // Residue masses plus water (approx)
            Assert.AreEqual(1016.32, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(1016.93, proteoform.GetMass(MassType.Average), 0.01);
        }

        /// <summary>
        /// A formal charge on a modification means that additional protons are present or have been removed.
        /// To account for this, we adjust the chemical formula to add/remove hydrogen atoms.
        /// </summary>
        [Test]
        public void HandleFormalCharge()
        {
            var term = new ProFormaTerm("KEQVENCE", tags: new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0074") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, _residLookup);

            Assert.IsNotNull(proteoform.LocalizedModifications);

            // Residue masses plus water (approx)
            Assert.AreEqual(1019.46, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(1020.12, proteoform.GetMass(MassType.Average), 0.01);
        }
    }
}
