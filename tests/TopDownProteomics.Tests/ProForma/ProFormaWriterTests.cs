using NUnit.Framework;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class ProFormaWriterTests
    {
        public static ProFormaWriter _writer = new ProFormaWriter();

        [Test]
        public void WriteSequenceOnly()
        {
            var term = new ProFormaTerm("SEQUENCE");
            var result = _writer.WriteString(term);

            Assert.AreEqual(term.Sequence, result);
        }

        [Test]
        public void WriteSingleTag()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") })
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[info:test]UENCE", result);
        }

        [Test]
        public void WriteMultipleTags()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") }),
                new ProFormaTag(5, new[] { new ProFormaDescriptor(ProFormaKey.Mass, "+14.05") }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[info:test]UEN[mass:+14.05]CE", result);
        }

        [Test]
        public void WriteMultipleDescriptors()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Info, "test"),
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05")
                })
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[info:test|mass:+14.05]UENCE", result);
        }

        [Test]
        public void WriteAmbiguousPossibleSitesDescriptors()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] 
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                    new ProFormaAmbiguityDescriptor("#", "test")
                }),
                new ProFormaTag(5, new[] { new ProFormaAmbiguityDescriptor("#", "test") }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[mass:+14.05|#test]UEN[#test]CE", result);
        }

        [Test]
        public void WriteAmbiguousRangeDescriptors()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                    new ProFormaAmbiguityDescriptor("->", "test")
                }),
                new ProFormaTag(5, new[] { new ProFormaAmbiguityDescriptor("<-", "test") }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[mass:+14.05|test->]UEN[<-test]CE", result);
        }

        [Test]
        public void WriteAmbiguousUnlocalizedTags()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(-1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[mass:+14.05]?SEQUENCE", result);
        }

        [Test]
        public void WriteMultipleAmbiguousUnlocalizedTags()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(-1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                }),
                new ProFormaTag(-1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "79.98"),
                }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[mass:+14.05]?[mass:79.98]?SEQUENCE", result);
        }

        [Test]
        public void WriteTerminalModsOnly()
        {
            var term = new ProFormaTerm("SEQUENCE", nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[info:test]-SEQUENCE", result);

            term = new ProFormaTerm("SEQUENCE", cTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") });
            result = _writer.WriteString(term);

            Assert.AreEqual("SEQUENCE-[info:test]", result);

            term = new ProFormaTerm("SEQUENCE",
                nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "testN") }, 
                cTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "testC") });
            result = _writer.WriteString(term);

            Assert.AreEqual("[info:testN]-SEQUENCE-[info:testC]", result);
        }

        [Test]
        public void WriteMultipleTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") }),
                new ProFormaTag(5, new[] { new ProFormaDescriptor(ProFormaKey.Mass, "+14.05") }),
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[info:unknown]-SEQ[info:test]UEN[mass:+14.05]CE", result);
        }

        [Test]
        public void WritePossibleSitesAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "14.05"),
                    new ProFormaAmbiguityDescriptor("#", "test")
                }),
                new ProFormaTag(5, new[] { new ProFormaAmbiguityDescriptor("#", "test") }),
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[info:unknown]-SEQ[mass:14.05|#test]UEN[#test]CE", result);
        }

        [Test]
        public void WriteRangeAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "14.05"),
                    new ProFormaAmbiguityDescriptor("->", "test")
                }),
                new ProFormaTag(5, new[] { new ProFormaAmbiguityDescriptor("<-", "test") }),
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[info:unknown]-SEQ[mass:14.05|test->]UEN[<-test]CE", result);
        }

        [Test]
        public void WriteUnlocalizedAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", new[]
            {
                new ProFormaTag(-1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "14.05")
                }),
            }, new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") }, null, null);
            var result = _writer.WriteString(term);

            Assert.AreEqual("[mass:14.05]?[info:unknown]-SEQUENCE", result);
        }
    }
}