using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TopDownProteomics.Proteomics;

namespace TopDownProteomics.IO.Resid
{
    /// <summary>
    /// Parse RESID modification XML.
    /// </summary>
    public class ResidXmlParser
    {
        /// <summary>
        /// Parses the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<ResidModification> Parse(string path)
        {
            return SimpleStreamAxis(new StreamReader(File.OpenRead(path)), "Entry").Select(x => ConvertToModification(x));
        }

        /// <summary>
        /// Parses the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<ResidModification> ParseText(string text)
        {
            return SimpleStreamAxis(new StringReader(text), "Entry").Select(x => ConvertToModification(x));
        }

        private ResidModification ConvertToModification(XElement elem)
        {
            string code = elem.Attribute(XName.Get("id")).Value;
            int id = Convert.ToInt32(code.Substring(2));
            string name = (string)(elem.Element("Names").Element("Name"));

            string formula = (string)elem.XPathSelectElement("/FormulaBlock/Formula");
            int formalCharge = GetFormulaCharge(elem, "/FormulaBlock/FormalCharge");
            double? monoMass = GetMass(elem, "/FormulaBlock/Weight[@type='physical']");
            double? avMass = GetMass(elem, "/FormulaBlock/Weight[@type='chemical']");
            string diffFormula = null; //(string)elem.XPathSelectElement("/CorrectionBlock/Formula"); ;
            double? diffMonoMass = null; // this.GetMass(elem, "/CorrectionBlock/Weight[@type='physical']");
            double? diffAvMass = null; // this.GetMass(elem, "/CorrectionBlock/Weight[@type='chemical']");

            var correctionBlocks = new List<XElement>(elem.XPathSelectElements("/CorrectionBlock"));

            if (correctionBlocks.Count > 0 && id > 20) // Any ID less than 20 (amino acids) will not have a diff mass
            {
                // Find first one that isn't zero
                foreach (XElement correctionBlock in correctionBlocks)
                {
                    XAttribute attribute = correctionBlock.Attribute("uids");

                    // If the correction refers to this entry, skip
                    if (attribute.Value.Contains(code))
                        continue;

                    diffFormula = (string)correctionBlock.XPathSelectElement("Formula");
                    diffMonoMass = GetMass(correctionBlock, "Weight[@type='physical']");
                    diffAvMass = GetMass(correctionBlock, "Weight[@type='chemical']");

                    //if (diffAvMass.HasValue && diffAvMass.Value != 0.00)
                    //    break;
                }
            }

            var aminoAcids = new HashSet<string>(elem.XPathSelectElements("/SequenceCode/SequenceSpec").Select(x => x.Value));
            char? aminoAcid = null;

            if (aminoAcids.Count == 1 && !aminoAcids.First().Contains(","))
                aminoAcid = Convert.ToChar(aminoAcids.First());

            var conditions = new HashSet<string>(elem.XPathSelectElements("/SequenceCode/Condition").Select(x => x.Value));
            Terminus? terminus = null;

            if (conditions.Count > 0)
            {
                if (conditions.Contains("amino-terminal"))
                    terminus = Terminus.N;
                else if (conditions.Contains("carboxyl-terminal"))
                    terminus = Terminus.C;
            }

            var modification = new ResidModification(id, name, formula, monoMass.Value, avMass.Value,
                diffFormula, diffMonoMass, diffAvMass, terminus, aminoAcid, formalCharge);

            var features = new List<string>(elem.XPathSelectElements("/Features/Feature[@type='UniProt']").Select(x => x.Value));

            if (features.Count > 0)
            {
                foreach (string feature in features)
                {
                    if (feature.StartsWith("MOD_RES"))
                    {
                        modification.SwissprotTerm = feature.Substring(8);
                        break;
                    }
                }
            }

            return modification;
        }
        private double? GetMass(XElement elem, string xPath)
        {
            string mass = (string)elem.XPathSelectElement(xPath);

            if (mass == null)
                return null;

            // Remove + suffix
            mass = mass.EndsWith(" +") ? mass.Substring(0, mass.Length - 2) : mass;

            // Remove additional masses
            mass = mass.Contains(",") ? mass.Substring(0, mass.IndexOf(",")) : mass;

            double massValue = Convert.ToDouble(mass);

            return Math.Abs(massValue) < 0.000001 ? (double?)null : massValue;
        }
        private int GetFormulaCharge(XElement elem, string xPath)
        {
            string mass = (string)elem.XPathSelectElement(xPath);

            if (mass == null)
                return 0;

            if (mass.EndsWith("+"))
                return Convert.ToInt32(mass[0].ToString());
            if (mass.EndsWith("-"))
                return -Convert.ToInt32(mass[0].ToString());

            return 0;
        }

        /// <summary>
        /// Enables one to stream XElements from a reader.
        /// </summary>
        /// <param name="inputReader">The input reader.</param>
        /// <param name="matchName">Name of the match.</param>
        /// <returns></returns>
        public static IEnumerable<XElement> SimpleStreamAxis(TextReader inputReader, string matchName)
        {
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore };

            using (var reader = XmlReader.Create(inputReader, settings))
            {
                reader.MoveToContent();

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == matchName)
                            {
                                if (XNode.ReadFrom(reader) is XElement el)
                                    yield return el;
                            }

                            break;
                    }
                }
            }
        }
    }
}