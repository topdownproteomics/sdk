using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.IO.Resid;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for RESID modifications.
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class ResidModificationLookup : IProteoformModificationLookup
    {
        IProteoformModification[] _modifications;

        private ResidModificationLookup(IProteoformModification[] modifications)
        {
            _modifications = modifications;
        }

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="elementProvider">The element provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<ResidModification> modifications,
            IElementProvider elementProvider)
        {
            if (modifications == null) throw new ArgumentNullException(nameof(modifications));

            var modArray = new IProteoformModification[1024]; // More RESID IDs than will ever exist
            int maxId = -1;
            foreach (var modification in modifications)
            {
                IChemicalFormula chemicalFormula = ParseResidFormula(modification.DiffFormula, elementProvider);

                if (chemicalFormula != null)
                    modArray[modification.Id] = new ResidModificationWrapper(modification, chemicalFormula);

                // Keep all the way up to the max passed in, even if it turns out to be NULL
                maxId = Math.Max(maxId, modification.Id);
            }

            Array.Resize(ref modArray, maxId + 1);

            return new ResidModificationLookup(modArray);
        }

        private static IChemicalFormula ParseResidFormula(string formula, IElementProvider elementProvider)
        {
            if (string.IsNullOrEmpty(formula))
                return null;

            var cells = formula.Split(' ');

            var elements = new List<IEntityCardinality<IElement>>();

            for (int i = 0; i < cells.Length; i += 2)
            {
                if (cells[i] == "+")
                    continue;

                int count = Convert.ToInt32(cells[i + 1]);

                if (count != 0)
                    elements.Add(new EntityCardinality<IElement>(elementProvider.GetElement(cells[i]), count));
            }

            return new ChemicalFormula(elements);
        }

        /// <summary>
        /// Determines whether this instance [can handle descriptor] the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns>
        /// <c>true</c> if this instance [can handle descriptor] the specified descriptor; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandleDescriptor(ProFormaDescriptor descriptor)
        {
            return descriptor.Key == ProFormaKey.Resid ||
                (descriptor.Key == ProFormaKey.Mod && descriptor.Value.EndsWith(this.GetModNameDatabaseTag()));
        }

        /// <summary>
        /// Gets the modification.
        /// </summary>
        /// <param name="descriptor">The descriptor.</param>
        /// <returns></returns>
        public IProteoformModification GetModification(ProFormaDescriptor descriptor)
        {
            if (descriptor.Value == null)
                throw new ProteoformModificationLookupException($"Value is NULL in descriptor {descriptor.Key}:{descriptor.Value}.");

            if (descriptor.Key == ProFormaKey.Resid)
            {
                var value = descriptor.Value;

                // Remove prefix AA
                if (value.StartsWith("AA"))
                    value = value.Substring(2);

                if (int.TryParse(value, out int resid))
                {
                    if (resid < 0 || resid > _modifications.Length - 1 || _modifications[resid] == null)
                        throw new ProteoformModificationLookupException($"Could not find modification using ID in descriptor {descriptor.Key}:{descriptor.Value}.");

                    return _modifications[resid];
                }

                throw new ProteoformModificationLookupException($"Invalid integer in descriptor {descriptor.Key}:{descriptor.Value}.");
            }
            else if (descriptor.Key == ProFormaKey.Mod)
            {
                int index = descriptor.Value.IndexOf(this.GetModNameDatabaseTag());

                if (index < 0)
                    throw new ProteoformModificationLookupException($"Couldn't find database name in descriptor {descriptor.Key}:{descriptor.Value}.");

                var value = descriptor.Value.Substring(0, index).Trim();

                var modification = _modifications
                    .SingleOrDefault(x => x != null && ((ResidModificationWrapper)x).Modification.Name == value);

                if (modification == null)
                    throw new ProteoformModificationLookupException($"Could not find modification using Name in descriptor {descriptor.Key}:{descriptor.Value}.");

                return modification;
            }

            throw new ProteoformModificationLookupException($"Couldn't handle value for descriptor {descriptor.Key}:{descriptor.Value}.");
        }

        private string GetModNameDatabaseTag() => $"({ProFormaKey.Resid})";

        private class ResidModificationWrapper : IProFormaProteoformModification
        {
            IChemicalFormula _chemicalFormula;

            public ResidModificationWrapper(ResidModification modification, IChemicalFormula chemicalFormula)
            {
                this.Modification = modification;
                _chemicalFormula = chemicalFormula;
            }

            public ResidModification Modification { get; }

            public IChemicalFormula GetChemicalFormula() => _chemicalFormula;

            public ProFormaDescriptor GetProFormaDescriptor()
            {
                throw new NotImplementedException();
            }
        }
    }
}