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

            Assert.AreEqual("SEQ[Info:test]UENCE", result);
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

            Assert.AreEqual("SEQ[Info:test]UEN[+14.05]CE", result);
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

            Assert.AreEqual("SEQ[Info:test|+14.05]UENCE", result);
        }

        [Test]
        public void WriteAmbiguousPossibleSitesDescriptors()
        {
            var term = new ProFormaTerm("SEQUENCE", tagGroups: new[]
            {
                new ProFormaTagGroup("test", ProFormaKey.Mass, "+14.05", new[]
                {
                    new ProFormaMembershipDescriptor(2),
                    new ProFormaMembershipDescriptor(5),
                }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[+14.05#test]UEN[#test]CE", result);
        }

        [Test]
        public void WriteAmbiguousRangeDescriptors()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, 5, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                })
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SE(QUEN)[+14.05]CE", result);
        }

        [Test]
        public void WriteAmbiguousUnlocalizedTags()
        {
            var term = new ProFormaTerm("SEQUENCE", unlocalizedTags: new[]
            {
                new ProFormaUnlocalizedTag(1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[+14.05]?SEQUENCE", result);
        }

        [Test]
        public void WriteMultipleAmbiguousUnlocalizedTags()
        {
            var term = new ProFormaTerm("SEQUENCE", unlocalizedTags: new[]
            {
                new ProFormaUnlocalizedTag(1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                }),
                new ProFormaUnlocalizedTag(2, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+79.98"),
                }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[+14.05][+79.98]^2?SEQUENCE", result);
        }

        [Test]
        public void WriteTerminalModsOnly()
        {
            var term = new ProFormaTerm("SEQUENCE", nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[Info:test]-SEQUENCE", result);

            term = new ProFormaTerm("SEQUENCE", cTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "test") });
            result = _writer.WriteString(term);

            Assert.AreEqual("SEQUENCE-[Info:test]", result);

            term = new ProFormaTerm("SEQUENCE",
                nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "testN") },
                cTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "testC") });
            result = _writer.WriteString(term);

            Assert.AreEqual("[Info:testN]-SEQUENCE-[Info:testC]", result);
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

            Assert.AreEqual("[Info:unknown]-SEQ[Info:test]UEN[+14.05]CE", result);
        }

        [Test]
        public void WritePossibleSitesAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", tagGroups: new[]
            {
                new ProFormaTagGroup("test", ProFormaKey.Mass, "+14.05", new[]
                {
                    new ProFormaMembershipDescriptor(2),
                    new ProFormaMembershipDescriptor(5),
                }),
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });

            var result = _writer.WriteString(term);

            Assert.AreEqual("[Info:unknown]-SEQ[+14.05#test]UEN[#test]CE", result);
        }

        [Test]
        public void WriteRangeAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, 5, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05"),
                })
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });

            var result = _writer.WriteString(term);

            Assert.AreEqual("[Info:unknown]-SE(QUEN)[+14.05]CE", result);
        }

        [Test]
        public void WriteUnlocalizedAmbiguousTagsTerminalMod()
        {
            var term = new ProFormaTerm("SEQUENCE", unlocalizedTags: new[]
            {
                new ProFormaUnlocalizedTag(1, new[]
                {
                    new ProFormaDescriptor(ProFormaKey.Mass, "+14.05")
                }),
            }, nTerminalDescriptors: new[] { new ProFormaDescriptor(ProFormaKey.Info, "unknown") });
            var result = _writer.WriteString(term);

            Assert.AreEqual("[+14.05]?[Info:unknown]-SEQUENCE", result);
        }

        [Test]
        public void WriteModificationNameAndIdentifiers()
        {
            // RESID
            var term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, "AA0420") }),
                new ProFormaTag(4, new[] { new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Resid, "Test") }),
            });
            var result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[RESID:AA0420]UE[R:Test]NCE", result);

            // PSI-MOD
            term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, "MOD:00232") }),
                new ProFormaTag(4, new[] { new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.PsiMod, "Test") }),
            });
            result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[MOD:00232]UE[M:Test]NCE", result);

            // Unimod
            term = new ProFormaTerm("SEQUENCE", tags: new[]
            {
                new ProFormaTag(2, new[] { new ProFormaDescriptor(ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, "UNIMOD:15") }),
                new ProFormaTag(4, new[] { new ProFormaDescriptor(ProFormaKey.Name, ProFormaEvidenceType.Unimod, "Test") }),
            });
            result = _writer.WriteString(term);

            Assert.AreEqual("SEQ[UNIMOD:15]UE[U:Test]NCE", result);
        }
    }
}