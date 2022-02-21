using System;
using System.Collections.Generic;
using TopDownProteomics.Biochemistry;
using TopDownProteomics.IO.Obo;

namespace TopDownProteomics.IO.Unimod
{
    /// <summary>
    /// Unimod obo parser
    /// </summary>
    public class UnimodOboParser
    {
        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<UnimodModification> Parse(string path)
        {
            var oboParser = new OboParser();
            return this.ConvertToModifications(oboParser.Parse(path));
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<UnimodModification> ParseText(string text)
        {
            var oboParser = new OboParser();
            return this.ConvertToModifications(oboParser.ParseText(text));
        }

        private IList<UnimodModification> ConvertToModifications(IEnumerable<OboTerm> terms)
        {
            IList<UnimodModification> modifications = new List<UnimodModification>();
            foreach (OboTerm term in terms)
            {
                // Skip the first modification as it is not a real one
                if (term.Id != "UNIMOD:0")
                {
                    UnimodModification modification = this.ConvertToModification(term);

                    modifications.Add(modification);
                }
            }

            return modifications;
        }

        private UnimodModification ConvertToModification(OboTerm term)
        {
            string code = term.Id;
            string name = term.Name;

            string? definition = null;
            string? definitionRemainder = null;
            string? diffFormula = null;
            double diffMonoMass = 0;
            double diffAvMass = 0;
            List<char>? allowedResidueSymbols = null;
            ModificationTerminalSpecificity termini = ModificationTerminalSpecificity.None;
            HashSet<string> classifications = new();

            if (term.ValuePairs != null)
            {
                foreach (OboTagValuePair pair in term.ValuePairs)
                {
                    if (pair.Tag == "def")
                    {
                        string rawDefinition = pair.Value.Replace("\\", string.Empty).Replace("\"", string.Empty);

                        int remainderStart = rawDefinition.IndexOf('[');

                        if (remainderStart >= 0)
                        {
                            definition = rawDefinition[..remainderStart].Replace(".", string.Empty).Trim();
                            definitionRemainder = rawDefinition[remainderStart..].Trim();
                        }
                        else
                        {
                            throw new Exception("Couldn't find Unimod definition remainder.");
                        }
                    }
                    else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_composition"))
                        diffFormula = pair.Value.Replace("\"", string.Empty).Substring(18);
                    else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_mono_mass"))
                        diffMonoMass = Convert.ToDouble(pair.Value.Replace("\"", string.Empty).Substring(16));
                    else if (pair.Tag == "xref" && pair.Value.StartsWith("delta_avge_mass"))
                        diffAvMass = Convert.ToDouble(pair.Value.Replace("\"", string.Empty).Substring(16));
                    else if (pair.Tag == "xref" && pair.Value.Contains("_site"))
                    {
                        string value = this.GetXRefValue(pair.Value);

                        if (value.Length == 1)
                        {
                            allowedResidueSymbols ??= new List<char>();
                            allowedResidueSymbols.Add(value[0]);
                        }
                    }
                    else if (pair.Tag == "xref" && pair.Value.Contains("N-term"))
                    {
                        termini |= ModificationTerminalSpecificity.N;
                    }
                    else if (pair.Tag == "xref" && pair.Value.Contains("C-term"))
                    {
                        termini |= ModificationTerminalSpecificity.C;
                    }
                    else if (pair.Tag == "xref" && pair.Value.Contains("_classification"))
                    {
                        string value = this.GetXRefValue(pair.Value);

                        classifications.Add(value);
                    }
                }
            }

            if (definition != null && definitionRemainder != null && diffFormula != null)
                return new UnimodModification(code, name, definition, definitionRemainder, diffFormula, diffMonoMass, diffAvMass, allowedResidueSymbols, termini, classifications);

            throw new Exception("Could not find required fields 'definition' (with square brackets) and 'diffFormula'.");
        }

        private string GetXRefValue(string fullValue)
        {
            // Return everything between the outermost double quotes
            int startIndex = fullValue.IndexOf('"');
            return fullValue.Substring(startIndex + 1, fullValue.LastIndexOf('"') - startIndex - 1);
        }
    }
}