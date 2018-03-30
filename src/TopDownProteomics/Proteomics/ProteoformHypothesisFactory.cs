using System.Collections.Generic;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// Creates IProteoformHypothesis objects from a ProForma Term.
    /// </summary>
    public class ProteoformHypothesisFactory
    {
        /// <summary>
        /// Creates the hypothesis.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="modificationLookup">The modification lookup.</param>
        /// <returns></returns>
        public IProteoformHypothesis CreateHypothesis(ProFormaTerm term, IProteoformModificationLookup modificationLookup)
        {
            ICollection<IProteoformModification> modifications = null;

            if (term.Tags != null && term.Tags.Count > 0)
            {
                if (modificationLookup == null)
                    throw new ProteoformHypothesisCreateException("Cannot lookup tag because lookup wasn't provided.");

                foreach (var tag in term.Tags)
                {
                    foreach (var descriptor in tag.Descriptors)
                    {
                        if (modificationLookup.CanHandleDescriptor(descriptor))
                        {
                            var modification = modificationLookup.GetModification(descriptor);

                            if (modification != null)
                            {
                                if (modifications == null)
                                    modifications = new List<IProteoformModification>();

                                modifications.Add(modification);
                            }
                        }
                        else
                        {
                            throw new ProteoformHypothesisCreateException($"Couldn't handle descriptor {descriptor.Key}:{descriptor.Value}.");
                        }
                    }
                }
            }

            return new ProteoformHypothesis(term.Sequence, modifications);
        }

        private class ProteoformHypothesis : IProteoformHypothesis
        {
            public ProteoformHypothesis(string sequence, ICollection<IProteoformModification> modifications)
            {
                Sequence = sequence;
                Modifications = modifications;
            }

            public string Sequence { get; }
            public ICollection<IProteoformModification> Modifications { get; }
        }
    }
}