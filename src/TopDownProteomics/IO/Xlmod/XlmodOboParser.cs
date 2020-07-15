using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.IO.Obo;

namespace TopDownProteomics.IO.Xlmod
{
    /// <summary>
    /// OBO format parser for PSI-MOD modifications.
    /// </summary>
    public class XlmodOboParser
    {
        /// <summary>
        /// Parses the file at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<XlmodTerm> Parse(string path)
        {
            var oboParser = new OboParser();
            return oboParser.Parse(path).Select(term => this.ConvertToModification(term));
        }

        /// <summary>
        /// Parses the text directly.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public IEnumerable<XlmodTerm> ParseText(string text)
        {
            var oboParser = new OboParser();
            return oboParser.ParseText(text).Select(term => this.ConvertToModification(term));
        }

        private XlmodTerm ConvertToModification(OboTerm oboTerm)
        {
            string? definition = null;
            ICollection<XlmodSynonym>? synonyms = null;
            ICollection<XlmodExternalReference>? externalReferences = null;
            ICollection<XlmodProperty>? properties = null;
            ICollection<XlmodRelationship>? relationships = null;
            ICollection<string>? isA = null;

            if (oboTerm.ValuePairs != null)
            {
                foreach (OboTagValuePair pair in oboTerm.ValuePairs)
                {
                    var valueSpan = pair.Value.AsSpan();

                    switch (pair.Tag)
                    {
                        case "synonym":
                            // "Text" Score Type []
                            {
                                int endOfQuote = valueSpan.LastIndexOf('"');
                                var text = valueSpan.Slice(1, endOfQuote - 1);
                                var type = valueSpan.Slice(endOfQuote + 2, valueSpan.LastIndexOf('[') - endOfQuote - 3);

                                Utility.LazyCreateAndAdd(ref synonyms, new XlmodSynonym(type.ToString(), text.ToString()));
                            }
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
                                    Utility.LazyCreateAndAdd(ref externalReferences, new XlmodExternalReference(splitColon2[0], splitColon2[1]));
                                }
                            }
                            break;
                        case "property_value":
                            // property_value: doubletDeltaMass: "8.05016" xsd:double
                            {
                                int startOfQuote = valueSpan.IndexOf('"');
                                int endOfQuote = valueSpan.LastIndexOf('"');
                                var name = valueSpan.Slice(0, startOfQuote - 2);
                                var value = valueSpan.Slice(startOfQuote + 1, endOfQuote - startOfQuote - 1);
                                var dataType = valueSpan.Slice(endOfQuote + 2);

                                Utility.LazyCreateAndAdd(ref properties, new XlmodProperty(name.ToString(), value.ToString(), dataType.ToString()));
                            }
                            break;
                        case "relationship":
                            // relationship: has_property XLMOD:00014 ! hydrophilic
                            {
                                int spaceIndex = valueSpan.IndexOf(' ');
                                var type = valueSpan.Slice(0, spaceIndex);
                                var id = valueSpan.Slice(spaceIndex + 1, valueSpan.IndexOf('!') - spaceIndex - 2);

                                Utility.LazyCreateAndAdd(ref relationships, new XlmodRelationship(type.ToString(), id.ToString()));
                            }
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
                return new XlmodTerm(oboTerm.Id, oboTerm.Name, definition, externalReferences, isA, 
                    relationships, synonyms, properties);

            throw new Exception("Could not find required 'definition' field.");
        }
    }
}