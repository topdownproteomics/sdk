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
                UniprotModification currentModification = null;

                while (reader.Peek() > 0)
                {
                    string lineType = this.ReadCharacters(reader, 2);

                    if (lineType == "ID")
                    {
                        currentModification = new UniprotModification
                        {
                            Identifier = this.CleanString(reader.ReadLine())
                        };
                    }
                    else if (lineType == "AC") currentModification.Accession = this.CleanString(reader.ReadLine());
                    else if (lineType == "FT") currentModification.FeatureKey = UniprotUtility.GetFeatureFromKeyName(this.CleanString(reader.ReadLine()));
                    else if (lineType == "TG") currentModification.Target = this.CleanString(reader.ReadLine());
                    else if (lineType == "PA") currentModification.AminoAcidPosition = this.CleanString(reader.ReadLine());
                    else if (lineType == "PP") currentModification.PolypeptidePosition = this.CleanString(reader.ReadLine());
                    else if (lineType == "CF") currentModification.CorrectionFormula = this.CleanString(reader.ReadLine());
                    else if (lineType == "MM") currentModification.MonoisotopicMassDifference = this.SafeParseDouble(reader.ReadLine());
                    else if (lineType == "MA") currentModification.AverageMassDifference = this.SafeParseDouble(reader.ReadLine());
                    else if (lineType == "LC") currentModification.CellularLocation = this.CleanString(reader.ReadLine());
                    else if (lineType == "TR")
                    {
                        if (currentModification.TaxonomicRanges == null)
                            currentModification.TaxonomicRanges = new List<string>();

                        currentModification.TaxonomicRanges.Add(this.CleanString(reader.ReadLine()));
                    }
                    else if (lineType == "KW")
                    {
                        if (currentModification.Keywords == null)
                            currentModification.Keywords = new List<string>();

                        currentModification.Keywords.Add(this.CleanString(reader.ReadLine()));
                    }
                    else if (lineType == "DR")
                    {
                        string line = this.CleanString(reader.ReadLine());
                        string value = line.Substring(line.IndexOf(" ") + 1); // Anything after the first space

                        if (line.StartsWith("RESID"))
                            currentModification.Resid = value;
                        else if (line.StartsWith("PSI-MOD"))
                            currentModification.PsiMod = value;
                    }
                    else if (lineType == "//")
                    {
                        yield return currentModification;
                        reader.ReadLine();
                    }
                    else // Unknown line, read past this
                        reader.ReadLine();
                }
            }
        }

        private string ReadCharacters(TextReader reader, int numCharacters)
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