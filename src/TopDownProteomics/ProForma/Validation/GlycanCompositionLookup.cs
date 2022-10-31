using System;
using System.Collections.Generic;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation;

/// <summary>Lookup for modifications given by glycan composition.</summary>
/// <seealso cref="IProteoformModificationLookup" />
public class GlycanCompositionLookup : IProteoformModificationLookup
{
    private readonly IGlycanResidueProvider _glycanResidueProvider;

    /// <summary>Initializes a new instance of the <see cref="GlycanCompositionLookup"/> class.</summary>
    /// <param name="glycanResidueProvider">The glycan residue provider.</param>
    public GlycanCompositionLookup(IGlycanResidueProvider glycanResidueProvider)
    {
        _glycanResidueProvider = glycanResidueProvider;
    }

    /// <summary>Determines whether this instance [can handle descriptor] the specified descriptor.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns>
    ///   <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.</returns>
    public bool CanHandleDescriptor(IProFormaDescriptor descriptor) => descriptor.Key == ProFormaKey.Glycan;

    /// <summary>Gets the modification.</summary>
    /// <param name="descriptor">The descriptor.</param>
    /// <returns></returns>
    public IProteoformMassDelta? GetModification(IProFormaDescriptor descriptor)
    {
        var input = descriptor.Value.Replace(" ", string.Empty);
        //int currentPosition = 0;
        Dictionary<IGlycanResidue, IEntityCardinality<IGlycanResidue>> residues = new();

        while (input.Length > 0)
        {
            // Big ugly hard coded thing because there is no way to generically handle tokenizing
            string atom;

            if (input.StartsWith("HexNAcS")) atom = "HexNAcS";
            else if (input.StartsWith("HexNAc")) atom = "HexNAc";
            else if (input.StartsWith("HexS")) atom = "HexS";
            else if (input.StartsWith("HexP")) atom = "HexP";
            else if (input.StartsWith("Hex")) atom = "Hex";
            else if (input.StartsWith("dHex")) atom = "dHex";
            else if (input.StartsWith("NeuAc")) atom = "NeuAc";
            else if (input.StartsWith("NeuGc")) atom = "NeuGc";
            else if (input.StartsWith("Pen")) atom = "Pen";
            else if (input.StartsWith("Fuc")) atom = "Fuc";
            else
                throw new ProteoformModificationLookupException($"Could not parse composition string for descriptor {descriptor}");

            int currentPosition = atom.Length;

            var residue = _glycanResidueProvider.GetGlycanResidue(atom);

            // Check for cardinality
            int numberEndIndex = currentPosition;

            while (numberEndIndex < input.Length && (char.IsDigit(input[numberEndIndex]) || input[numberEndIndex] == '-'))
                numberEndIndex++;

            int cardinality = 1;

            if (numberEndIndex != currentPosition)
            {
                string value = input.Substring(currentPosition, numberEndIndex - currentPosition);
                cardinality = Convert.ToInt32(value);

                currentPosition = numberEndIndex;
            }

            // Merge duplicates
            if (residues.ContainsKey(residue))
            {
                var existing = residues[residue];
                residues[residue] = new EntityCardinality<IGlycanResidue>(residue, existing.Count + cardinality);
            }
            else
            {
                residues.Add(residue, new EntityCardinality<IGlycanResidue>(residue, cardinality));
            }

            input = input.Substring(currentPosition);
        }

        return new GlycanModification(new GlycanComposition(residues.Values));
    }

    private class GlycanComposition : IGlycanComposition
    {
        readonly IReadOnlyCollection<IEntityCardinality<IGlycanResidue>> _residues;

        public GlycanComposition(IReadOnlyCollection<IEntityCardinality<IGlycanResidue>> residues)
        {
            _residues = residues;
        }

        public IChemicalFormula GetChemicalFormula()
        {
            IChemicalFormula total = ChemicalFormula.Empty;

            foreach (var residue in _residues)
            {
                var formula = residue.Entity.GetChemicalFormula().Multiply(residue.Count);

                total = total.Add(formula);
            }

            return total;
        }

        public IReadOnlyCollection<IEntityCardinality<IGlycanResidue>> GetResidues() => _residues;
    }

    private class GlycanModification : IProteoformGlycanCompositionDelta, IProteoformFormulaProteoformDelta
    {
        private readonly IGlycanComposition _composition;

        public GlycanModification(IGlycanComposition composition)
        {
            _composition = composition;
        }

        public IChemicalFormula GetChemicalFormula() => _composition.GetChemicalFormula();

        public IGlycanComposition GetGlycanComposition() => _composition;

        public double GetMass(MassType massType) => _composition.GetChemicalFormula().GetMass(massType);
    }
}