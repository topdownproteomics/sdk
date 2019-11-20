using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    class ChemicalProteoformHashTests
    {
        IElementProvider _elementProvider;
        IResidueProvider _residueProvider;
        IProteoformModificationLookup _lookup;
        IProteoformModification _acetyl;
        string _acetylDescriptorString = "[formula:C2H2O1]";

        [OneTimeSetUp]
        public void Setup()
        {
            _elementProvider = new MockElementProvider();
            _residueProvider = new IupacAminoAcidProvider(_elementProvider);

            _lookup = new BrnoModificationLookup(_elementProvider);
            _acetyl = _lookup.GetModification(new ProFormaDescriptor("ac(BRNO)"));
        }

        [Test]
        public void NoMods()
        {
            string sequence = "SEQUENCE";
            MockProteoformGroup proteoformGroup = this.GetProteoformGroup(sequence);

            ChemicalProteoformHashGenerator chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator();
            string chemicalProteoformHash = chemicalProteoformHashGenerator.Generate(proteoformGroup);
            Assert.AreEqual(sequence, chemicalProteoformHash);
        }

        [Test]
        public void NTerminalMod()
        {
            string sequence = "SEQUENCE";
            MockProteoformGroup proteoformGroup = this.GetProteoformGroup(sequence);
            proteoformGroup.NTerminalModification = _acetyl;

            ChemicalProteoformHashGenerator chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator();
            string chemicalProteoformHash = chemicalProteoformHashGenerator.Generate(proteoformGroup);
            Assert.AreEqual($"{_acetylDescriptorString}-{sequence}", chemicalProteoformHash);
        }

        [Test]
        public void CTerminalMod()
        {
            string sequence = "SEQUENCE";
            MockProteoformGroup proteoformGroup = this.GetProteoformGroup(sequence);
            proteoformGroup.CTerminalModification = _acetyl;

            ChemicalProteoformHashGenerator chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator();
            string chemicalProteoformHash = chemicalProteoformHashGenerator.Generate(proteoformGroup);
            Assert.AreEqual($"{sequence}-{_acetylDescriptorString}", chemicalProteoformHash);
        }

        [Test]
        public void OneModification()
        {
            string sequence = "SEQUENCE";
            MockProteoformGroup proteoformGroup = this.GetProteoformGroup(sequence);
            proteoformGroup.AddModification(_acetyl, 2);

            ChemicalProteoformHashGenerator chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator();
            string chemicalProteoformHash = chemicalProteoformHashGenerator.Generate(proteoformGroup);
            Assert.AreEqual($"SEQ{_acetylDescriptorString}UENCE", chemicalProteoformHash);
        }

        [Test]
        public void MultipleModifications()
        {
            string sequence = "SEQUENCE";
            MockProteoformGroup proteoformGroup = this.GetProteoformGroup(sequence);
            proteoformGroup.NTerminalModification = _acetyl;
            proteoformGroup.CTerminalModification = _acetyl;
            proteoformGroup.AddModification(_acetyl, 2);

            ChemicalProteoformHashGenerator chemicalProteoformHashGenerator = new ChemicalProteoformHashGenerator();
            string chemicalProteoformHash = chemicalProteoformHashGenerator.Generate(proteoformGroup);
            Assert.AreEqual($"{_acetylDescriptorString}-SEQ{_acetylDescriptorString}UENCE-{_acetylDescriptorString}", chemicalProteoformHash);
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
