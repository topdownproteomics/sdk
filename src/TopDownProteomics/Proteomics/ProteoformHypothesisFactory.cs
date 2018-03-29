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

            if (term.Tags != null && modificationLookup != null)
            {
                foreach (var tag in term.Tags)
                {
                    foreach (var descriptor in tag.Descriptors)
                    {
                        if (modificationLookup.CanHandleDescriptor(descriptor))
                        {
                            if (modifications == null)
                                modifications = new List<IProteoformModification>();

                            var modification = modificationLookup.GetModification(descriptor);

                            modifications.Add(modification);
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