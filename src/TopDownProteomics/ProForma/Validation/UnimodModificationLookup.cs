using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.Chemistry;
using TopDownProteomics.Chemistry.Unimod;
using TopDownProteomics.IO.Unimod;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma.Validation
{
    /// <summary>
    /// Lookup for Unimod modifications.
    /// </summary>
    /// <seealso cref="IProteoformModificationLookup" />
    public class UnimodModificationLookup : IProteoformModificationLookup
    {
        private IProteoformModification[] _modifications;

        private UnimodModificationLookup(IProteoformModification[] modifications)
        {
            _modifications = modifications;
        }

        /// <summary>
        /// Initializes the <see cref="ResidModificationLookup" /> class.
        /// </summary>
        /// <param name="modifications">The modifications.</param>
        /// <param name="atomProvider">The atom provider.</param>
        /// <returns></returns>
        public static IProteoformModificationLookup CreateFromModifications(IEnumerable<UnimodModification> modifications,
            IUnimodCompositionAtomProvider atomProvider)
        {
            if (modifications == null)
                throw new ArgumentNullException(nameof(modifications));

            var modArray = new IProteoformModification[4096]; // More IDs than will ever exist (famous last words...)
            int maxId = -1;

            foreach (UnimodModification modification in modifications)
            {
                var composition = UnimodComposition.CreateFromFormula(modification.DeltaComposition, atomProvider);
                IChemicalFormula chemicalFormula = composition.GetChemicalFormula();

                if (chemicalFormula != null)
                    modArray[modification.Id] = new UnimodModificationWrapper(modification, chemicalFormula);

                // Keep all the way up to the max passed in, even if it turns out to be NULL
                maxId = Math.Max(maxId, modification.Id);
            }

            Array.Resize(ref modArray, maxId + 1);

            return new UnimodModificationLookup(modArray);
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
            return descriptor.Key == ProFormaKey.Unimod ||
                (descriptor.Key == ProFormaKey.Mod && !descriptor.Value.TrimEnd().EndsWith(")")) ||
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

            if (descriptor.Key == ProFormaKey.Unimod)
            {
                string value = descriptor.Value;

                // Remove prefix AA
                if (value.StartsWith("UNIMOD:"))
                    value = value.Substring(7);

                if (int.TryParse(value, out int id))
                {
                    if (id < 0 || id > _modifications.Length - 1 || _modifications[id] == null)
                        throw new ProteoformModificationLookupException($"Could not find modification using ID in descriptor {descriptor.Key}:{descriptor.Value}.");

                    return _modifications[id];
                }

                throw new ProteoformModificationLookupException($"Invalid integer in descriptor {descriptor.Key}:{descriptor.Value}.");
            }
            else if (descriptor.Key == ProFormaKey.Mod)
            {
                int index = descriptor.Value.IndexOf(this.GetModNameDatabaseTag());

                // Don't need this check for Unimod because it is the default mod type and can be left off
                //if (index < 0)
                //    throw new ProteoformModificationLookupException($"Couldn't find database name in descriptor {descriptor.Key}:{descriptor.Value}.");

                string value = descriptor.Value;

                if (index >= 0)
                    value = value.Substring(0, index).Trim();

                IProteoformModification modification = _modifications
                    .SingleOrDefault(x => x != null && ((UnimodModificationWrapper)x).Modification.Name == value);

                if (modification == null)
                    throw new ProteoformModificationLookupException($"Could not find modification using Name in descriptor {descriptor.Key}:{descriptor.Value}.");

                return modification;
            }

            throw new ProteoformModificationLookupException($"Couldn't handle value for descriptor {descriptor.Key}:{descriptor.Value}.");
        }

        private string GetModNameDatabaseTag() => $"({ProFormaKey.Unimod})";

        private class UnimodModificationWrapper : IProFormaProteoformModification
        {
            private IChemicalFormula _chemicalFormula;

            public UnimodModificationWrapper(UnimodModification modification, IChemicalFormula chemicalFormula)
            {
                this.Modification = modification;
                _chemicalFormula = chemicalFormula;
            }

            public UnimodModification Modification { get; }

            public IChemicalFormula GetChemicalFormula() => _chemicalFormula;

            public ProFormaDescriptor GetProFormaDescriptor()
            {
                throw new NotImplementedException();
            }
        }
    }
}