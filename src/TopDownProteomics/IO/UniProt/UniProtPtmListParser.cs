using System.Collections.Generic;
using System.IO;

namespace TopDownProteomics.IO.UniProt
{
    /// <summary>
    /// A parser for UniProt's ptmlist.txt file (https://www.uniprot.org/docs/ptmlist)
    /// </summary>
    public class UniProtPtmListParser
    {
        /// <summary>
        /// Parses the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<UniprotModification> Parse(string text)
        {
            using (var reader = new StringReader(text))
            {
                string? identifier = null;
                string? accession = null;
                UniprotFeatureType? featureKey = null;
                string? target = null;
                string? aminoAcidPosition = null;
                string? polypeptidePosition = null;
                string? correctionFormula = null;
                double monoisotopicMassDifference = 0.0;
                double averageMassDifference = 0.0;
                string? cellularLocation = null;

                ICollection<string>? taxonomicRanges = null;
                ICollection<string>? keywords = null;

                string? resid = null;
                string? psiMod = null;
                string? unimod = null;

                while (reader.Peek() > 0)
                {
                    string? lineType = this.ReadCharacters(reader, 2);

                    if (lineType == "ID") identifier = this.CleanString(reader.ReadLine());
                    else if (lineType == "AC") accession = this.CleanString(reader.ReadLine());
                    else if (lineType == "FT") featureKey = UniprotUtility.GetFeatureFromKeyName(this.CleanString(reader.ReadLine()));
                    else if (lineType == "TG") target = this.CleanString(reader.ReadLine());
                    else if (lineType == "PA") aminoAcidPosition = this.CleanString(reader.ReadLine());
                    else if (lineType == "PP") polypeptidePosition = this.CleanString(reader.ReadLine());
                    else if (lineType == "CF") correctionFormula = this.CleanString(reader.ReadLine());
                    else if (lineType == "MM") monoisotopicMassDifference = this.SafeParseDouble(reader.ReadLine());
                    else if (lineType == "MA") averageMassDifference = this.SafeParseDouble(reader.ReadLine());
                    else if (lineType == "LC") cellularLocation = this.CleanString(reader.ReadLine());
                    else if (lineType == "TR")
                    {
                        Utility.LazyCreateAndAdd(ref taxonomicRanges, this.CleanString(reader.ReadLine()));
                    }
                    else if (lineType == "KW")
                    {
                        Utility.LazyCreateAndAdd(ref keywords, this.CleanString(reader.ReadLine()));
                    }
                    else if (lineType == "DR")
                    {
                        string line = this.CleanString(reader.ReadLine());
                        string value = line.Substring(line.IndexOf(" ") + 1); // Anything after the first space

                        if (line.StartsWith("RESID"))
                            resid = value;
                        else if (line.StartsWith("PSI-MOD"))
                            psiMod = value;
                        else if (line.StartsWith("Unimod"))
                            unimod = value;
                    }
                    else if (lineType == "//")
                    {
                        if (identifier != null && accession != null && featureKey.HasValue && target != null && aminoAcidPosition != null &&
                            polypeptidePosition != null && cellularLocation != null)
                        {
                            yield return new UniprotModification(identifier, accession, featureKey.Value, target, aminoAcidPosition, polypeptidePosition,
                                correctionFormula, monoisotopicMassDifference, averageMassDifference, cellularLocation, taxonomicRanges, keywords,
                                resid, psiMod, unimod);
                        }

                        // Reset collections
                        taxonomicRanges = null;
                        keywords = null;

                        reader.ReadLine();
                    }
                    else // Unknown line, read past this
                        reader.ReadLine();
                }
            }
        }

        private string? ReadCharacters(TextReader reader, int numCharacters)
        {
            char[] buffer = new char[numCharacters];
            reader.Read(buffer, 0, numCharacters);

            if (buffer[0] == '\0')
            {
                return null;
            }

            return new string(buffer);
        }

        private double SafeParseDouble(string rawDouble)
        {
            double value;

            if (double.TryParse(rawDouble, out value))
                return value;

            return -1.0;
        }

        private string CleanString(string x)
        {
            return x.TrimStart().TrimEnd('.');
        }
    }
}