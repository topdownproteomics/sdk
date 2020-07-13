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
        /// <exception cref="NotImplementedException"></exception>
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
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<PsiModTerm> ParseText(string text)
        {
            var oboParser = new OboParser();
            return oboParser.ParseText(text).Select(term => this.ConvertToModification(term));
        }

        private PsiModTerm ConvertToModification(OboTerm oboTerm)
        {
            PsiModTerm term = new PsiModTerm();

            string idString = oboTerm.Id;
            term.Id = Convert.ToInt32(idString.Substring(4));
            term.Name = oboTerm.Name;

            foreach (OboTagValuePair pair in oboTerm.ValuePairs)
            {
                switch (pair.Tag)
                {
                    case "synonym":
                        // "Text" Score Type []
                        PsiModSynonym synonym = new PsiModSynonym();

                        string[] splitQuote = pair.Value.Trim('"').Split('"');
                        synonym.Text = splitQuote[0];
                        string[] split = splitQuote[1].Trim(' ').Split(' ');
                        synonym.Scope = split[0];
                        synonym.Type = split[1];

                        term.Synonyms.Add(synonym);
                        break;
                    case "def":
                        // "defString" [dbName1:acc1, dbName2:acc2]
                        string[] splitQuot = pair.Value.Substring(1).Split('"');
                        term.Definition = splitQuot[0];

                        string refs = splitQuot[1];
                        refs = refs.Trim(' ', '[', ']');
                        if (refs != "")
                        {
                            string[] splitComma = refs.Split(',');
                            foreach (string reference in splitComma.Select(s => s.Trim(' ')))
                            {
                                PsiModExternalReference psiModExternalReference = new PsiModExternalReference();
                                string[] splitColon = reference.Split(':');
                                psiModExternalReference.Name = splitColon[0];
                                psiModExternalReference.Id = splitColon[1];
                                term.ExternalReferences.Add(psiModExternalReference);
                            }
                        }
                        break;
                    case "comment":
                        term.Comment = pair.Value;
                        break;
                    case "xref":
                        this.HandleXref(pair.Value, term);
                        break;
                    case "is_obsolete":
                        term.IsObsolete = true;
                        break;
                    case "is_a":
                        // MOD:00000 ! description
                        string modNum = pair.Value.Split('!')[0].Trim(' ').Split(':')[1];
                        term.IsA.Add(Convert.ToInt32(modNum));
                        break;
                }
            }

            return term;
        }

        private void HandleXref(string xref, PsiModTerm term)
        {
            string[] splitColon = xref.Split(':');
            string value = splitColon[1].Trim(' ', '"');

            if (value != "none")
            {
                switch (splitColon[0])
                {
                    case "DiffAvg":
                        term.DiffAvg = Convert.ToDouble(value);
                        break;
                    case "DiffFormula":
                        term.DiffFormula = value;
                        break;
                    case "DiffMono":
                        term.DiffMono = Convert.ToDouble(value);
                        break;
                    case "MassAvg":
                        term.MassAvg = Convert.ToDouble(value);
                        break;
                    case "Formula":
                        term.Formula = value;
                        break;
                    case "MassMono":
                        term.MassMono = Convert.ToDouble(value);
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
                        term.FormalCharge = coefficient * num;
                        break;
                    case "Origin":
                        if (value.Length == 1 && value != "X")
                            term.Origin = value[0];
                        break;
                    case "Source":
                        if (value == "natural")
                            term.Source = PsiModModificationSource.Natural;
                        else if (value == "artifact")
                            term.Source = PsiModModificationSource.Artifact;
                        else if (value == "hypothetical")
                            term.Source = PsiModModificationSource.Hypothetical;
                        break;
                    case "TermSpec":
                        if (value == "N-term")
                            term.Terminus = Terminus.N;
                        else if (value == "C-term")
                            term.Terminus = Terminus.C;
                        break;
                }
            }

        }
    }
}