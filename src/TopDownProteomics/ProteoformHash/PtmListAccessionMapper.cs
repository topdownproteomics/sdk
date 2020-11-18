using System;
using System.Collections.Generic;
using TopDownProteomics.IO.UniProt;
using TopDownProteomics.ProForma;
using TopDownProteomics.ProForma.Validation;

namespace TopDownProteomics.ProteoformHash
{
    /// <summary>
    /// A modification mapper that uses ptmlist.txt to map everything to PSI-MOD accessions.
    /// </summary>
    /// <seealso cref="IAccessionMapper" />
    public class PtmListAccessionMapper : IAccessionMapper
    {
        IList<UniprotModification> _uniprotModifications;
        Dictionary<string, string> _residMap;
        Dictionary<string, string> _unimodMap; 
        Dictionary<string, string> _brnoMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="PtmListAccessionMapper"/> class.
        /// </summary>
        /// <param name="uniprotModifications">The uniprot modifications.</param>
        public PtmListAccessionMapper(IList<UniprotModification> uniprotModifications)
        {
            _uniprotModifications = uniprotModifications;
            _residMap = new Dictionary<string, string>();
            _unimodMap = new Dictionary<string, string>();

            foreach (var mod in uniprotModifications)
            {
                if (mod.PsiMod != null)
                {
                    if (mod.Resid != null)
                    {
                        if (!_residMap.ContainsKey(mod.Resid))
                            _residMap.Add(mod.Resid, mod.PsiMod);
                        else
                            Console.WriteLine($"{mod.Resid}->{mod.PsiMod}");
                    }

                    if (mod.Unimod != null)
                    {
                        if (!_unimodMap.ContainsKey(mod.Unimod))
                            _unimodMap.Add(mod.Unimod, mod.PsiMod);
                        else
                            Console.WriteLine($"{mod.Unimod}->{mod.PsiMod}");
                    }
                }
            }

            // Add some custom things that aren't in ptmlist
            _unimodMap.Add("374", "MOD:00798");

            // Keep BRNO around
            _brnoMap = new Dictionary<string, string>
            {
                { "B:ac", "MOD:00394" }
            };
        }

        /// <summary>
        /// Maps the given accession to another accession.
        /// </summary>
        /// <param name="accession">The accession.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Can't map accession '{accession}'.</exception>
        public Tuple<ProFormaEvidenceType, string> MapAccession(string accession)
        {
            // Pass PSI-MOD accessions right through
            if (accession.StartsWith("MOD:"))
                return Tuple.Create(ProFormaEvidenceType.PsiMod, accession);

            // Defer to GNO for all things glycan
            if (accession.StartsWith("GNO:"))
                return Tuple.Create(ProFormaEvidenceType.Gno, accession);

            if (accession.StartsWith("AA"))
            {
                if (_residMap.ContainsKey(accession))
                    return Tuple.Create(ProFormaEvidenceType.PsiMod, _residMap[accession]);
            }

            if (accession.StartsWith(UnimodModificationLookup.Prefix, StringComparison.OrdinalIgnoreCase))
            {
                string key = accession.Substring(7);

                if (_unimodMap.ContainsKey(key))
                    return Tuple.Create(ProFormaEvidenceType.PsiMod, _unimodMap[key]);
            }

            if (accession.StartsWith("B:"))
            {
                if (_brnoMap.ContainsKey(accession))
                    return Tuple.Create(ProFormaEvidenceType.PsiMod, _brnoMap[accession]);
            }

            throw new Exception($"Can't map accession '{accession}'.");
        }
    }
}