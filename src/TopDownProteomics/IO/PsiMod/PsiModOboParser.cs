using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.IO.Obo;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>
    /// OBO format parser for PSI-MOD modifications.
    /// </summary>
    public class PsiModOboParser
    {
        /// <summary>
        /// Parses the file at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<PsiModTerm> Parse(string path)
        {
            var oboParser = new OboParser();
            return oboParser.Parse(path).Select(term => this.ConvertToModification(term));
        }

        /// <summary>
        /// Parses the text directly.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<PsiModTerm> ParseText(string text)
        {
            var oboParser = new OboParser();
            return oboParser.ParseText(text).Select(term => this.ConvertToModification(term));
        }

        private PsiModTerm ConvertToModification(OboTerm oboTerm)
        {
            string? definition = null;
            ICollection<PsiModSynonym>? synonyms = null;
            ICollection<PsiModExternalReference>? externalReferences = null;
            ICollection<string>? isA = null;
            string? comment = null;
            bool isObsolete = false;
            string? remap = null;

            double? diffAvg = null;
            string? diffFormula = null;
            double? diffMono = null;
            double? massAvg = null;
            string? massFormula = null;
            double? massMono = null;

            int formalCharge = 0;
            char? origin = null;

            PsiModModificationSource? source = null;
            Terminus? terminus = null;

            if (oboTerm.ValuePairs != null)
            {
                foreach (OboTagValuePair pair in oboTerm.ValuePairs)
                {
                    switch (pair.Tag)
                    {
                        case "synonym":
                            // "Text" Score Type []
                            string[] splitQuote = pair.Value.Trim('"').Split('"');
                            string text = splitQuote[0];
                            string[] split = splitQuote[1].Trim(' ').Split(' ');
                            string scope = split[0];
                            string type = split[1];

                            Utility.LazyCreateAndAdd(ref synonyms, new PsiModSynonym(type, text, scope));

                            break;
                        case "def":
                            // "defString" [dbName1:acc1, dbName2:acc2]
                            string[] splitQuot = pair.Value.Substring(1).Split('"');
                            definition = splitQuot[0];

                            string refs = splitQuot[1];
                            refs = refs.Trim(' ', '[', ']');
                            if (refs != "")
                            {
                                string[] splitComma = refs.Split(',');
                                foreach (string reference in splitComma.Select(s => s.Trim(' ')))
                                {
                                    string[] splitColon2 = reference.Split(':');
                                    Utility.LazyCreateAndAdd(ref externalReferences, new PsiModExternalReference(splitColon2[0], splitColon2[1]));
                                }
                            }
                            break;
                        case "comment":
                            comment = pair.Value;
                            break;
                        case "xref":
                            int colonIndex = pair.Value.IndexOf(':');
                            string key = pair.Value[0..colonIndex];
                            string value = pair.Value[(colonIndex + 1)..].Trim(' ', '"');

                            if (value != "none")
                            {
                                switch (key)
                                {
                                    case "DiffAvg":
                                        diffAvg = Convert.ToDouble(value);
                                        break;
                                    case "DiffFormula":
                                        diffFormula = value;
                                        break;
                                    case "DiffMono":
                                        diffMono = Convert.ToDouble(value);
                                        break;
                                    case "MassAvg":
                                        massAvg = Convert.ToDouble(value);
                                        break;
                                    case "Formula":
                                        massFormula = value;
                                        break;
                                    case "MassMono":
                                        massMono = Convert.ToDouble(value);
                                        break;
                                    case "FormalCharge":
                                        char sign = value.Last();
                                        int coefficient = 0;
                                        if (sign == '+')
                                        {
                                            coefficient = 1;
                                        }
                                        else if (sign == '-')
                                        {
                                            coefficient = -1;
                                        }
                                        int num = Convert.ToInt32(value.Substring(0, value.Length - 1));
                                        formalCharge = coefficient * num;
                                        break;
                                    case "Origin":
                                        if (value.Length == 1 && value != "X")
                                            origin = value[0];
                                        break;
                                    case "Source":
                                        if (value == "natural")
                                            source = PsiModModificationSource.Natural;
                                        else if (value == "artifact")
                                            source = PsiModModificationSource.Artifact;
                                        else if (value == "hypothetical")
                                            source = PsiModModificationSource.Hypothetical;
                                        break;
                                    case "TermSpec":
                                        if (value == "N-term")
                                            terminus = Terminus.N;
                                        else if (value == "C-term")
                                            terminus = Terminus.C;
                                        break;
                                    case "Remap":
                                        remap = value;
                                        break;
                                }
                            }
                            break;
                        case "is_obsolete":
                            isObsolete = true;
                            break;
                        case "is_a":
                            // MOD:00000 ! description
                            string modNum = pair.Value.Split('!')[0].Trim(' ');
                            Utility.LazyCreateAndAdd(ref isA, modNum);
                            break;
                    }
                }
            }

            if (definition != null)
                return new PsiModTerm(oboTerm.Id, oboTerm.Name, definition, externalReferences, synonyms, comment, diffAvg, diffFormula,
                    diffMono, massFormula, massAvg, massMono, origin, source, terminus, isObsolete, remap, formalCharge, isA);

            throw new Exception("Could not find required 'definition' field.");
        }
    }
}