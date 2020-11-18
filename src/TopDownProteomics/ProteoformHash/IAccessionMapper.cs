using System;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>Maps a modification accession to another accession.</summary>
    public interface IAccessionMapper
    {
        /// <summary>
        /// Maps the given accession to another accession.
        /// </summary>
        /// <param name="accession">The accession.</param>
        /// <returns></returns>
        public Tuple<ProFormaEvidenceType, string> MapAccession(string accession);
    }
}