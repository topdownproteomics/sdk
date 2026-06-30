using NUnit.Framework;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Tests.ProForma
{
    [TestFixture]
    public class ProFormaProteoformGroupTests
    {
        public static ProFormaParser _parser = new ProFormaParser();
        public static ProFormaWriter _writer = new ProFormaWriter();

        [Test]
        public void Charge()
        {
            var group = _parser.ParseProteoformGroupString("EMEVEESPEK/2");

            Assert.AreEqual(1, group.Peptidoforms.Count);
            Assert.AreEqual(1, group.Peptidoforms[0].Chains.Count);
            Assert.AreEqual(2, group.Peptidoforms[0].Charge);
            Assert.AreEqual("EMEVEESPEK/2", _writer.WriteString(group));
        }

        [Test]
        public void ChargeWithAdducts()
        {
            var group = _parser.ParseProteoformGroupString("EMEVEESPEK/2[+2Na+,+H+]");

            Assert.AreEqual(2, group.Peptidoforms[0].Charge);
            Assert.AreEqual("+2Na+,+H+", group.Peptidoforms[0].IonAdducts);
            Assert.AreEqual("EMEVEESPEK/2[+2Na+,+H+]", _writer.WriteString(group));
        }

        [Test]
        public void NegativeCharge()
        {
            var group = _parser.ParseProteoformGroupString("EMEVEESPEK/-2[2I-]");

            Assert.AreEqual(-2, group.Peptidoforms[0].Charge);
            Assert.AreEqual("EMEVEESPEK/-2[2I-]", _writer.WriteString(group));
        }

        [Test]
        public void Chimeric()
        {
            var group = _parser.ParseProteoformGroupString("EMEVEESPEK/2+ELVISLIVER/3");

            Assert.AreEqual(2, group.Peptidoforms.Count);
            Assert.AreEqual(2, group.Peptidoforms[0].Charge);
            Assert.AreEqual(3, group.Peptidoforms[1].Charge);
            Assert.AreEqual("EMEVEESPEK/2+ELVISLIVER/3", _writer.WriteString(group));
        }

        [Test]
        public void MultiChainSplitsOnDoubleSlash()
        {
            var group = _parser.ParseProteoformGroupString("SEK[XLMOD:02001#XL1]UENCE//EMEVTK[XLMOD:02001#XL1]SESPEK");

            Assert.AreEqual(1, group.Peptidoforms.Count);
            Assert.AreEqual(2, group.Peptidoforms[0].Chains.Count);
            Assert.IsNull(group.Peptidoforms[0].Charge);
        }

        [Test]
        public void SingleTermYieldsOnePeptidoformOneChain()
        {
            var group = _parser.ParseProteoformGroupString("EM[Oxidation]EVEES[Phospho]PEK");

            Assert.AreEqual(1, group.Peptidoforms.Count);
            Assert.AreEqual(1, group.Peptidoforms[0].Chains.Count);
            Assert.IsNull(group.Peptidoforms[0].Charge);
            Assert.AreEqual("EM[Oxidation]EVEES[Phospho]PEK", _writer.WriteString(group));
        }

        // The ProForma 2.0 specification examples above a single term (sections 4.2.3.2, 4.2.4, 7.1, 7.2).
        [TestCase("EMEVEESPEK/2")]
        [TestCase("EM[U:Oxidation]EVEES[U:Phospho]PEK/3")]
        [TestCase("EMEVEESPEK/2[+2Na+,+H+]")]
        [TestCase("EMEVEESPEK/1[+2Na+,-H+]")]
        [TestCase("EMEVEESPEK/-2[2I-]")]
        [TestCase("EMEVEESPEK/-1[+e-]")]
        [TestCase("EMEVEESPEK/2+ELVISLIVER/3")]
        [TestCase("SEK[XLMOD:02001#XL1]UENCE//EMEVTK[XLMOD:02001#XL1]SESPEK")]
        [TestCase("SEK[XLMOD:02001#XL1]UENCE//EMEVTK[#XL1]SESPEK")]
        [TestCase("ETFGD[MOD:00093#BRANCH]//R[#BRANCH]ATER")]
        public void RoundTripIsCanonicallyIdempotent(string proForma)
        {
            string first = _writer.WriteString(_parser.ParseProteoformGroupString(proForma));
            string second = _writer.WriteString(_parser.ParseProteoformGroupString(first));

            Assert.AreEqual(first, second);
        }
    }
}
