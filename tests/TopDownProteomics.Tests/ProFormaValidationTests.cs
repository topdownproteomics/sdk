using NUnit.Framework;
using System.Collections.Generic;
using TopDownProteomics.ProForma;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests
{
    [TestFixture]
    public class ProFormaValidationTests
    {
        public static ProteoformHypothesisFactory _factory = new ProteoformHypothesisFactory();

        [Test]
        public void NoTagsValid()
        {
            const string sequence = "SEQVENCE";
            var term = new ProFormaTerm(sequence, null);
            var proteoform = _factory.CreateHypothesis(term, null);

            Assert.AreEqual(sequence, proteoform.Sequence);
            Assert.IsNull(proteoform.Modifications);
        }

        [Test]
        public void TagsWithoutLookupThrowException()
        {
            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") })
            });

            Assert.Throws<ProteoformHypothesisCreateException>(() => _factory.CreateHypothesis(term, null));
        }

        [Test]
        public void IgnoreMassTag()
        {
            IProteoformModificationLookup modificationLookup = new IgnoreKeyModificationLookup(ProFormaKey.Mass);

            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") })
            });
            var proteoform = _factory.CreateHypothesis(term, modificationLookup);

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

            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("mass", "14.05") }),
                new ProFormaTag(5, new[] { new ProFormaDescriptor("info", "not important") })
            });
            var proteoform = _factory.CreateHypothesis(term, modificationLookup);

            Assert.IsNull(proteoform.Modifications);

            term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[]
                {
                    new ProFormaDescriptor("mass", "14.05"),
                    new ProFormaDescriptor("info", "not important")
                })
            });
            proteoform = _factory.CreateHypothesis(term, modificationLookup);

            Assert.IsNull(proteoform.Modifications);
        }

        [Test]
        public void HandleModificationNameTag()
        {
            var modificationLookup = new BrnoModificationLookup();

            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("ac(BRNO)") })
            });
            var proteoform = _factory.CreateHypothesis(term, modificationLookup);

            Assert.IsNotNull(proteoform.Modifications);
        }

        [Test]
        public void HandleBadModificationName()
        {
            var modificationLookup = new BrnoModificationLookup();

            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("wrong(BRNO)") })
            });
            Assert.Throws<ProteoformHypothesisCreateException>(() => _factory.CreateHypothesis(term, modificationLookup));
        }

        [Test]
        [Ignore("Need to wait for chemical formula parsing.")]
        public void MultipleModsOneSite()
        {
            var modificationLookup = new CompositeModificationLookup(new[]
            {
                new IgnoreKeyModificationLookup(ProFormaKey.Mass),
                new IgnoreKeyModificationLookup(ProFormaKey.Info)
            });

            // Modifications have same chemical formula ... OK
            var term = new ProFormaTerm("SEQVKENCE", new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    new ProFormaDescriptor("Acetyl"),
                    new ProFormaDescriptor("Unimod:1")
                })
            });
            var proteoform = _factory.CreateHypothesis(term, modificationLookup);
            Assert.IsNotNull(proteoform.Modifications);
            Assert.AreEqual(1, proteoform.Modifications.Count);

            // Modifications have different chemical formulas ... throw!
            term = new ProFormaTerm("SEQVKENCE", new List<ProFormaTag>
            {
                new ProFormaTag(4, new[]
                {
                    new ProFormaDescriptor("Methyl"),
                    new ProFormaDescriptor("Acetyl")
                })
            });
            Assert.Throws<ProteoformHypothesisCreateException>(() => _factory.CreateHypothesis(term, modificationLookup));
        }

        [Test]
        [Ignore("Need to wait for RESID modification lookup.")]
        public void HandleDatabaseAccessionTag()
        {
            var modificationLookup = new BrnoModificationLookup();

            var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
            {
                new ProFormaTag(3, new[] { new ProFormaDescriptor("RESID", "AA0038") })
            });
            var proteoform = _factory.CreateHypothesis(term, modificationLookup);

            Assert.IsNotNull(proteoform.Modifications);
        }
    }
}