using NUnit.Framework;
using System.Collections.Generic;
using TopDownProteomics.ProForma;
using TopDownProteomics.Proteomics;

namespace TestProject1
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
        public void IgnoreMassInfoTags()
        {
            var modificationLookup = new BrnoModificationLookup();

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

        //[Test]
        //public void HandleDatabaseAccessionTag()
        //{
        //    var modificationLookup = new BrnoModificationLookup();

        //    var term = new ProFormaTerm("SEQVENCE", new List<ProFormaTag>
        //    {
        //        new ProFormaTag(3, new[] { new ProFormaDescriptor("RESID", "AA0038") })
        //    });
        //    var proteoform = _factory.CreateHypothesis(term, modificationLookup);

        //    Assert.IsNotNull(proteoform.Modifications);
        //}
    }
}