using System;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>Maps a modification accession to a PSI-MOD accession.</summary>
    public interface IAccessionMapper
    {
        /// <summary>
        /// Maps the given accession to a PSI-MOD accession.
        /// </summary>
        /// <param name="accession">The accession.</param>
        /// <returns></returns>
        public Tuple<ProFormaEvidenceType, string> MapAccession(string accession);
    }
}