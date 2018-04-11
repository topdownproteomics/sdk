using TopDownProteomics.Proteomics;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// A proteoform modification that is aware of ProForma notation
    /// </summary>
    /// <seealso cref="IProteoformModification" />
    public interface IProFormaProteoformModification : IProteoformModification
    {
        /// <summary>
        /// Gets the ProForma descriptor.
        /// </summary>
        /// <returns></returns>
        ProFormaDescriptor GetProFormaDescriptor();
    }
}