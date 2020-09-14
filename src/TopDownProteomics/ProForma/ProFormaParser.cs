using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Parser for the ProForma proteoform notation (link here to published manuscript)
    /// </summary>
    public class ProFormaParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaParser"/> class.
        /// </summary>
        public ProFormaParser() { }

        /// <summary>
        /// Parses the ProForma string.
        /// </summary>
        /// <param name="proFormaString">The pro forma string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">proFormaString</exception>
        /// <exception cref="ProFormaParseException">
        /// X is not allowed.
        /// </exception>
        public ProFormaTerm ParseString(ReadOnlySpan<char> proFormaString)
        {
            if (proFormaString.Length == 0)
                throw new ArgumentNullException(nameof(proFormaString));

            IList<ProFormaTag>? tags = null;
            IList<ProFormaDescriptor>? nTerminalDescriptors = null;
            IList<ProFormaDescriptor>? cTerminalDescriptors = null;
            IList<ProFormaDescriptor>? labileDescriptors = null;
            IList<ProFormaUnlocalizedTag>? unlocalizedTags = null;
            IDictionary<string, ProFormaTagGroup>? tagGroups = null;

            var sequence = new StringBuilder();
            var tag = new StringBuilder();
            bool inTag = false;
            bool inCTerminalTag = false;
            int openLeftBrackets = 0;
            int openLeftBraces = 0;

            // Don't love doing a global index of performance wise, but would need to restructure things to handle multiple unlocalized tags
            int unlocalizedIndex = proFormaString.IndexOf('?');

            for (int i = 0; i < proFormaString.Length; i++)
            {
                if (unlocalizedIndex == i) continue; // Skip unlocalized separator

                char current = proFormaString[i];

                if (current == '{' && openLeftBraces++ == 0)
                    inTag = true;
                else if (current == '}' && --openLeftBraces == 0)
                {
                    string tagText = tag.ToString();

                    labileDescriptors = this.ProcessTag(tagText, sequence.Length - 1, ref tagGroups);

                    inTag = false;
                    tag.Clear();
                }
                else if (current == '[' && openLeftBrackets++ == 0)
                    inTag = true;
                else if (current == ']' && --openLeftBrackets == 0)
                {
                    string tagText = tag.ToString();

                    // Handle terminal modifications and prefix tags
                    if (inCTerminalTag)
                    {
                        cTerminalDescriptors = this.ProcessTag(tagText, -1, ref tagGroups);
                    }
                    else if (sequence.Length == 0 && proFormaString[i + 1] == '-')
                    {
                        nTerminalDescriptors = this.ProcessTag(tagText, -1, ref tagGroups);
                        i++; // Skip the - character
                    }
                    else if (unlocalizedIndex >= i)
                    {
                        // Make sure the prefix came before the N-terminal modification
                        if (nTerminalDescriptors != null)
                            throw new ProFormaParseException($"Unlocalized modification must come before an N-terminal modification.");

                        if (unlocalizedTags == null)
                            unlocalizedTags = new List<ProFormaUnlocalizedTag>();

                        var descriptors = this.ProcessTag(tagText, -1, ref tagGroups);

                        if (descriptors != null)
                        {
                            int count = 1;

                            // Check for higher count
                            if (proFormaString[i + 1] == '^')
                            {
                                int j = i + 2;
                                while (char.IsDigit(proFormaString[j]))
                                    j++;

                                if (!int.TryParse(proFormaString.Slice(i + 2, j - i - 2), out count))
                                    throw new ProFormaParseException("Can't process number after '^' character.");

                                i = j - 1; // Point i at the last digit
                            }

                            unlocalizedTags.Add(new ProFormaUnlocalizedTag(count, descriptors));
                        }

                        //i++; // skip the ? character
                    }
                    else
                    {
                        this.ProcessTag(tagText, sequence.Length - 1, ref tags, ref tagGroups);
                    }

                    inTag = false;
                    tag.Clear();
                }
                else if (inTag)
                {
                    tag.Append(current);
                }
                else if (current == '-')
                {
                    if (inCTerminalTag)
                        throw new ProFormaParseException($"- at index {i} is not allowed.");

                    inCTerminalTag = true;
                }
                else
                {
                    // Validate amino acid character
                    if (!char.IsUpper(current))
                        throw new ProFormaParseException($"{current} is not an upper case letter.");
                    //else if (current == 'X')
                    //    throw new ProFormaParseException("X is not allowed.");

                    sequence.Append(current);
                }
            }

            if (openLeftBrackets != 0)
                throw new ProFormaParseException($"There are {Math.Abs(openLeftBrackets)} open brackets in ProForma string {proFormaString.ToString()}");

            if (openLeftBraces != 0)
                throw new ProFormaParseException($"There are {Math.Abs(openLeftBraces)} open braces in ProForma string {proFormaString.ToString()}");

            return new ProFormaTerm(sequence.ToString(), tags, nTerminalDescriptors, cTerminalDescriptors, labileDescriptors,
                unlocalizedTags, tagGroups?.Values);
        }

        private void ProcessTag(string tag, int index, ref IList<ProFormaTag>? tags, ref IDictionary<string, ProFormaTagGroup>? tagGroups)
        {
            var descriptors = this.ProcessTag(tag, index, ref tagGroups);

            // Only add a tag if descriptors come back
            if (descriptors != null)
            {
                if (tags == null) tags = new List<ProFormaTag>();

                tags.Add(new ProFormaTag(index, descriptors));
            }
        }

        private IList<ProFormaDescriptor>? ProcessTag(string tag, int index, ref IDictionary<string, ProFormaTagGroup>? tagGroups)
        {
            IList<ProFormaDescriptor>? descriptors = null;
            var descriptorText = tag.Split('|');

            for (int i = 0; i < descriptorText.Length; i++)
            {
                var (key, evidence, value, group) = this.ParseDescriptor(descriptorText[i].TrimStart());

                if (!string.IsNullOrEmpty(group))
                {
                    if (tagGroups == null) tagGroups = new Dictionary<string, ProFormaTagGroup>();

                    if (!tagGroups.ContainsKey(group))
                    {
                        tagGroups.Add(group, new ProFormaTagGroupChangingValue(group, key, evidence, new List<ProFormaMembershipDescriptor>()));
                    }

                    var currentGroup = tagGroups[group];

                    // Fix up name of TagGroup
                    if (!string.IsNullOrEmpty(value) && currentGroup is ProFormaTagGroupChangingValue x)
                    {
                        // Only allow the value of the group to be set once
                        if (!string.IsNullOrEmpty(x.Value))
                            throw new ProFormaParseException($"You may only set the value of the group {group} once.");

                        x.ValueFlux = value;
                        x.Key = key;
                        x.EvidenceType = evidence;
                    }

                    tagGroups[group].Members.Add(new ProFormaMembershipDescriptor(index));
                }
                else if (key != ProFormaKey.None) // typical descriptor
                {
                    if (descriptors == null) descriptors = new List<ProFormaDescriptor>();

                    descriptors.Add(new ProFormaDescriptor(key, evidence, value));
                }
                else if (value.Length > 0) // keyless descriptor (UniMod or PSI-MOD annotation)
                {
                    if (descriptors == null) descriptors = new List<ProFormaDescriptor>();

                    descriptors.Add(new ProFormaDescriptor(value));
                }
                else
                {
                    throw new ProFormaParseException("Empty descriptor within tag " + tag);
                }
            }

            return descriptors;
        }

        private class ProFormaTagGroupChangingValue : ProFormaTagGroup
        {
            public ProFormaTagGroupChangingValue(string name, ProFormaKey key, ProFormaEvidenceType evidenceType, 
                IList<ProFormaMembershipDescriptor> members) : base(name, key, evidenceType, string.Empty, members)
            {
            }

            public string? ValueFlux 
            {
                get => this.Value;
                set
                {
                    if (value != null)
                        this.Value = value;
                }
            }

            public ProFormaKey? KeyFlux
            {
                get => this.Key;
                set
                {
                    if (value.HasValue)
                        this.Key = value.Value;
                }
            }

            public ProFormaEvidenceType? EvidenceFlux
            {
                get => this.EvidenceType;
                set
                {
                    if (value.HasValue)
                        this.EvidenceType = value.Value;
                }
            }
        }

        private Tuple<ProFormaKey, ProFormaEvidenceType, string, string?> ParseDescriptor(string text)
        {
            if (text.Length == 0)
                throw new ProFormaParseException("Cannot have an empty descriptor.");

            // Let's look for a group
            int groupIndex = text.IndexOf('#');
            string? groupName = null;

            if (groupIndex >= 0)
            {
                groupName = text.Substring(groupIndex + 1);
                text = text.Substring(0, groupIndex);

                if (string.IsNullOrEmpty(groupName))
                    throw new ProFormaParseException("Group name cannot be empty.");
            }

            // Check for naked group tag
            if (string.IsNullOrEmpty(text))
                return Tuple.Create(ProFormaKey.None, ProFormaEvidenceType.None, text, groupName);

            static ProFormaKey getKey(bool isMass) => isMass ? ProFormaKey.Mass : ProFormaKey.Name;

            // Let's look for a colon
            int colon = text.IndexOf(':');

            if (colon < 0)
            {
                bool isMass2 = text[0] == '+' || text[0] == '-';

                return Tuple.Create(getKey(isMass2), ProFormaEvidenceType.None, text, groupName);
            }

            // Let's see if the bit before the colon is a known key
            string keyText = text.Substring(0, colon).ToLower().Trim();
            bool isMass = text[colon + 1] == '+' || text[colon + 1] == '-';

            return keyText switch
            {
                "formula" => Tuple.Create(ProFormaKey.Formula, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName),
                "glycan" => Tuple.Create(ProFormaKey.Glycan, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName),
                "info" => Tuple.Create(ProFormaKey.Info, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName),

                var x when x == "mod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, text, groupName),
                var x when x == "unimod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, text, groupName),
                var x when x == "xlmod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.XlMod, text, groupName),
                var x when x == "gno" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Gno, text, groupName),

                // Special case for RESID id, don't inclue bit with colon
                var x when x == "resid" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, text.Substring(colon + 1), groupName),

                // Handle names and masses
                var x when x == "u" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Unimod, text.Substring(colon + 1), groupName),
                var x when x == "m" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.PsiMod, text.Substring(colon + 1), groupName),
                var x when x == "r" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Resid, text.Substring(colon + 1), groupName),
                var x when x == "x" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.XlMod, text.Substring(colon + 1), groupName),
                var x when x == "g" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Gno, text.Substring(colon + 1), groupName),
                var x when x == "obs" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Observed, text.Substring(colon + 1), groupName),

                _ => Tuple.Create(ProFormaKey.Name, ProFormaEvidenceType.None, text, groupName)
            };
        }
    }
}