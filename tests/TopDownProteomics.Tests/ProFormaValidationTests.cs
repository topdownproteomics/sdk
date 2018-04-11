using NUnit.Framework;
using System.Collections.Generic;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class ProFormaValidationTests
    {
        ProteoformGroupFactory _factory;
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            _residueProvider = new IupacAminoAcidProvider(_elementProvider);
            _factory = new ProteoformGroupFactory(_elementProvider, _residueProvider);
        }

        [Test]
        public void NoTagsValid()
        {
            const string sequence = "SEQVENCE";
            var term = new ProFormaTerm(sequence, null, null, null);
            var proteoform = _factory.CreateProteoformGroup(term, null);

            Assert.IsNotNull(proteoform.Residues);
            Assert.AreEqual(8, proteoform.Residues.Count);
            Assert.AreEqual(sequence, proteoform.GetSequence());
            Assert.IsNull(proteoform.Modifications);

            // Residue masses plus water (approx)
            Assert.AreEqual(936.35, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(936.95, proteoform.GetMass(MassType.Average), 0.01);
        }

        [Test]
        public void TagsWithoutLookupThrowException()
        {
            var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") })
            });

            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, null));
        }

        [Test]
        public void IgnoreMassTag()
        {
            IProteoformModificationLookup modificationLookup = new IgnoreKeyModificationLookup(ProFormaKey.Mass);

            var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNull(proteoform.Modifications);
        }

        [Test]
        public void IgnoreMultipleTags()
        {
            var modificationLookup = new CompositeModificationLookup(new[]
            {
                new IgnoreKeyModificationLookup(ProFormaKey.Mass),
                new IgnoreKeyModificationLookup(ProFormaKey.Info)
            });

            var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") }),
                new ProFormaTag(5, new[] { new ProFormaDescriptor("info", "not important") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNull(proteoform.Modifications);

            term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[]
                {
                    new ProFormaDescriptor("mass", "14.05"),
                    new ProFormaDescriptor("info", "not important")
                })
            });
            proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNull(proteoform.Modifications);
        }

        [Test]
        public void HandleModificationNameTag()
        {
            const string sequence = "SEQVENCE";
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            var term = new ProFormaTerm(sequence, null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("ac(BRNO)") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNotNull(proteoform.Modifications);
            Assert.AreEqual(1, proteoform.Modifications.Count);

            // Residue masses plus modification plus water (approx)
            Assert.AreEqual(978.36, proteoform.GetMass(MassType.Monoisotopic), 0.01);
            Assert.AreEqual(978.98, proteoform.GetMass(MassType.Average), 0.01);
        }

        [Test]
        public void HandleBadModificationName()
        {
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("wrong(BRNO)") })
            });
            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, modificationLookup));
        }

        [Test]
        [Ignore("Advanced scenario ... keep waiting.")]
        public void MultipleModsOneSite()
        {
            var modificationLookup = new CompositeModificationLookup(new[]
            {
                new IgnoreKeyModificationLookup(ProFormaKey.Mass),
                new IgnoreKeyModificationLookup(ProFormaKey.Info)
            });

            // Modifications have same chemical formula ... OK
            var term = new ProFormaTerm("SEQVKENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    new ProFormaDescriptor("Acetyl"),
                    new ProFormaDescriptor("Unimod:1")
                })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);
            Assert.IsNotNull(proteoform.Modifications);
            Assert.AreEqual(1, proteoform.Modifications.Count);

            // Modifications have different chemical formulas ... throw!
            term = new ProFormaTerm("SEQVKENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    new ProFormaDescriptor("Methyl"),
                    new ProFormaDescriptor("Acetyl")
                })
            });
            Assert.Throws<ProteoformGroupCreateException>(() => _factory.CreateProteoformGroup(term, modificationLookup));
        }

        [Test]
        [Ignore("Need to wait for RESID modification lookup.")]
        public void HandleDatabaseAccessionTag()
        {
            var modificationLookup = new BrnoModificationLookup(_elementProvider);

            var term = new ProFormaTerm("SEQVENCE", null, null, new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("RESID", "AA0038") })
            });
            var proteoform = _factory.CreateProteoformGroup(term, modificationLookup);

            Assert.IsNotNull(proteoform.Modifications);
        }
    }
}