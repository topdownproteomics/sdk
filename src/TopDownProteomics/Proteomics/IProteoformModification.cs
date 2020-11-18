namespace TopDownProteomics.Proteomics
{
    /// <summary>Modification on a chemical proteoform.</summary>
    public interface IProteoformModification
    {
        /// <summary>Gets the modification delta.</summary>
        IProteoformMassDelta ModificationDelta { get; }
    }

    /// <summary>Base for a proteoform modification.</summary>
    /// <seealso cref="IProteoformModification" />
    public class ProteoformModificationBase : IProteoformModification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteoformModificationBase"/> class.
        /// </summary>
        /// <param name="modificationDelta">The modification delta.</param>
        public ProteoformModificationBase(IProteoformMassDelta modificationDelta)
        {
            ModificationDelta = modificationDelta;
        }

        /// <summary>Gets the modification delta.</summary>
        public IProteoformMassDelta ModificationDelta { get; }
    }
}