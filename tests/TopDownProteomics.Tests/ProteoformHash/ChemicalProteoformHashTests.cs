using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;
using TopDownProteomics.ProteoformHash;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.Tests.ProteoformHash
{
    [TestFixture]
    public class ChemicalProteoformHashTests
    {
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;
        IProteoformModificationLookup _lookup;
        string _acetylDescriptorString = "[formula:C(2) H(2) O]";
        ChemicalProteoformHashGenerator _chemicalProteoformHashGenerator;
        string _acetylBrnoString = "[mod:ac(BRNO)]";

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            _residueProvider = new IupacAminoAcidProvider(_elementProvider);
            //_lookup = new FormulaLookup(_elementProvider);
            _lookup = new CompositeModificationLookup(new IProteoformModificationLookup[]
                {
                    new FormulaLookup(_elementProvider),
                    new BrnoModificationLookup(_elementProvider)
                });

            ProFormaParser proFormaParser = new ProFormaParser();
            ProteoformGroupFactory proteoformGroupFactory = new ProteoformGroupFactory(_elementProvider, _residueProvider);
            _chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator(proFormaParser, proteoformGroupFactory, _lookup);
        }

        [Test]
        public void NoMods()
        {
            this.TestHash("SEQUENCE", "SEQUENCE");
        }

        [Test]
        public void NTerminalMod()
        {
            string proForma = $"{_acetylDescriptorString}-SEQUENCE";
            this.TestHash(proForma, proForma);
        }

        [Test]
        public void CTerminalMod()
        {
            string proForma = $"SEQUENCE-{_acetylDescriptorString}";
            this.TestHash(proForma, proForma);
        }

        [Test]
        public void OneModification()
        {
            string proForma = $"SEQ{_acetylDescriptorString}UENCE";
            this.TestHash(proForma, proForma);
        }

        [Test]
        public void MultipleModifications()
        {
            string proForma = $"{_acetylDescriptorString}-SEQ{_acetylDescriptorString}UENCE-{_acetylDescriptorString}";
            this.TestHash(proForma, proForma);
        }

        [Test]
        public void NonFormula()
        {
            string proForma = $"SEQUE{_acetylBrnoString}NCE";
            this.TestHash(proForma, $"SEQUE{_acetylDescriptorString}NCE");
        }

        [Test]
        public void NullArgument()
        {
            Assert.Throws<ArgumentNullException>(() => _chemicalProteoformHashGenerator.Generate(null));
        }

        [Test]
        public void EmptyString()
        {
            Assert.Throws<ArgumentNullException>(() => _chemicalProteoformHashGenerator.Generate(""));
        }

        private void TestHash(string proForma, string expectedHash)
        {
            IChemicalProteoformHash chemicalProteoformHash = _chemicalProteoformHashGenerator.Generate(proForma);
            Assert.AreEqual(expectedHash, chemicalProteoformHash.Hash);
            Assert.IsTrue(chemicalProteoformHash.HasProForma);
            Assert.AreEqual(expectedHash, chemicalProteoformHash.ProForma);
        }

        private MockProteoformGroup GetProteoformGroup(string sequence, IProteoformModification nTermMod = null, IProteoformModification cTermMod = null)
        {
            MockProteoformGroup mockProteoformGroup = new MockProteoformGroup();
            mockProteoformGroup.Residues = sequence.Select(_residueProvider.GetResidue).ToList();
            mockProteoformGroup.NTerminalModification = nTermMod;
            mockProteoformGroup.CTerminalModification = cTermMod;

            return mockProteoformGroup;
        }

        private class MockProteoformGroup : IProteoformGroup
        {
            private double _waterMono = 18.010565;
            private double _waterAvg = 18.015;
            private List<IProteoformModificationWithIndex> _modifications = null;

            public IReadOnlyList<IResidue> Residues { get; set; }

            public IProteoformModification NTerminalModification { get; set; }

            public IProteoformModification CTerminalModification { get; set; }

            public IReadOnlyCollection<IProteoformModificationWithIndex> Modifications => this._modifications;

            public double GetMass(MassType massType)
            {
                return this.GetWaterMass(massType) +
                    this.Residues.Sum(x => x.GetChemicalFormula().GetMass(massType)) +
                    (this.Modifications?.Sum(x => x.GetChemicalFormula().GetMass(massType)) ?? 0.0) +
                    (this.NTerminalModification?.GetChemicalFormula().GetMass(massType) ?? 0.0) +
                    (this.CTerminalModification?.GetChemicalFormula().GetMass(massType) ?? 0.0);
            }

            private double GetWaterMass(MassType massType)
            {
                return massType == MassType.Monoisotopic ? this._waterMono : this._waterAvg;
            }

            public void AddModification(ProFormaDescriptor descriptor, IProteoformModificationLookup lookup, int index)
            {
                IProteoformModification proteoformModification = lookup.GetModification(descriptor);
                this.AddModification(proteoformModification, index);
            }

            public void AddModification(IProteoformModification proteoformModification, int index)
            {
                IProteoformModificationWithIndex proteoformModificationWithIndex = new ProteoformModificationWithIndex(proteoformModification, index);

                if (this._modifications == null)
                {
                    this._modifications = new List<IProteoformModificationWithIndex>();
                }

                this._modifications.Add(proteoformModificationWithIndex);
            }
        }
    }
}