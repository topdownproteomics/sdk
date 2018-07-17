using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.PsiMod
{
    /// <summary>
    /// Parser for PSI-MOD modifications.
    /// </summary>
    public class PsiModParser
    {
        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<PsiModTerm> Parse(string path)
        {
            using (FileStream fs = File.OpenRead(path))
            {
                using (var reader = XmlReader.Create(fs))
                {
                    reader.MoveToContent();

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                            continue;

                        if (reader.Name == "term")
                        {
                            yield return this.ParseTerm(reader);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<PsiModTerm> ParseText(string text)
        {
            using (var sr = new StringReader(text))
            {
                using (var reader = XmlReader.Create(sr))
                {
                    reader.MoveToContent();

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                            continue;

                        if (reader.Name == "term")
                        {
                            yield return this.ParseTerm(reader);
                        }
                    }
                }
            }
        }

        private PsiModTerm ParseTerm(XmlReader reader)
        {
            var term = new PsiModTerm();

            while (reader.Read())
            {
                // End case
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "term")
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                switch (reader.Name)
                {
                    case "id":
                        string idString = reader.ReadElementContentAsString();
                        term.Id = Convert.ToInt32(idString.Substring(4));
                        break;
                    case "name":
                        term.Name = reader.ReadElementContentAsString();
                        break;
                    case "synonym":
                        this.ParseSynonym(reader, term);
                        break;
                    case "defstr":
                        term.Definition = reader.ReadElementContentAsString();
                        break;
                    case "comment":
                        term.Comment = reader.ReadElementContentAsString();
                        break;
                    case "dbxref":
                        this.ParseDbXref(reader, term);
                        break;
                    case "xref_analog":
                        this.ParseXrefAnalog(reader, term);
                        break;
                    case "is_obsolete":
                        term.IsObsolete = true;
                        break;
                    case "is_a":
                        term.IsA.Add(Convert.ToInt32(reader.ReadElementContentAsString().Substring(4)));
                        break;
                }
            }

            return term;
        }
        private void ParseXrefAnalog(XmlReader reader, PsiModTerm term)
        {
            while (reader.Read())
            {
                // End case
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "xref_analog")
                    break;

                if (reader.NodeType != XmlNodeType.Element) continue;

                if (reader.Name == "dbname")
                {
                    string name = reader.ReadElementContentAsString();

                    reader.ReadToNextSibling("name");

                    switch (name)
                    {
                        case "DiffAvg":
                            term.DiffAvg = this.ReadDouble(reader);
                            break;
                        case "DiffFormula":
                            term.DiffFormula = reader.ReadElementContentAsString();
                            break;
                        case "DiffMono":
                            term.DiffMono = this.ReadDouble(reader);
                            break;
                        case "MassAvg":
                            term.MassAvg = this.ReadDouble(reader);
                            break;
                        case "Formula":
                            term.Formula = reader.ReadElementContentAsString();
                            break;
                        case "MassMono":
                            term.MassMono = this.ReadDouble(reader);
                            break;
                        case "FormalCharge":
                            term.FormalCharge = this.ReadFormulaCharge(reader);
                            break;
                        case "Origin":
                            string x = reader.ReadElementContentAsString();

                            if (x.Length == 1 && x != "X")
                                term.Origin = x[0];

                            // Some PSI-MOD terms have another term as their origin ... I'm ignoring this for now
                            //else
                            //{
                            //    if (x.Contains("MOD:"))
                            //        Console.WriteLine(term.Id + " " + x);
                            //}

                            break;
                        case "Source":
                            string sourceString = reader.ReadElementContentAsString();

                            if (sourceString == "natural")
                                term.Source = PsiModModificationSource.Natural;
                            else if (sourceString == "artifact")
                                term.Source = PsiModModificationSource.Artifact;
                            else if (sourceString == "hypothetical")
                                term.Source = PsiModModificationSource.Hypothetical;

                            break;
                        case "TermSpec":
                            string terminusString = reader.ReadElementContentAsString();

                            if (terminusString == "N-term")
                                term.Terminus = Terminus.N;
                            else if (terminusString == "C-term")
                                term.Terminus = Terminus.C;

                            break;
                    }
                }
            }
        }
        private void ParseDbXref(XmlReader reader, PsiModTerm term)
        {
            var externalReference = new PsiModExternalReference();

            while (reader.Read())
            {
                // End case
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "dbxref")
                    break;

                switch (reader.Name)
                {
                    case "acc":
                        externalReference.Id = reader.ReadElementContentAsString();
                        break;
                    case "dbname":
                        externalReference.Name = reader.ReadElementContentAsString();
                        break;
                }
            }

            term.ExternalReferences.Add(externalReference);
        }
        private void ParseSynonym(XmlReader reader, PsiModTerm term)
        {
            var synonym = new PsiModSynonym();
            synonym.Scope = reader.GetAttribute("scope");

            while (reader.Read())
            {
                // End case
                if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "synonym")
                    break;

                switch (reader.Name)
                {
                    case "synonym_type":
                        synonym.Type = reader.ReadElementContentAsString();
                        break;
                    case "synonym_text":
                        synonym.Text = reader.ReadElementContentAsString();
                        break;
                }
            }

            term.Synonyms.Add(synonym);
        }

        private double? ReadDouble(XmlReader reader)
        {
            string doubleString = reader.ReadElementContentAsString();

            if (double.TryParse(doubleString, out double x))
                return x;

            return null;
        }
        private int ReadFormulaCharge(XmlReader reader)
        {
            string x = reader.ReadElementContentAsString();

            if (x.EndsWith("+"))
                return Convert.ToInt32(x[0].ToString());
            if (x.EndsWith("-"))
                return -Convert.ToInt32(x[0].ToString());

            return 0;
        }
    }
}