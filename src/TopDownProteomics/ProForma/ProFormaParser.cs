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

#if !NETSTANDARD2_1
        /// <summary>
        /// Parses the ProForma string.
        /// </summary>
        /// <param name="proFormaString">The pro forma string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">proFormaString</exception>
        /// <exception cref="ProFormaParseException">
        /// X is not allowed.
        /// </exception>
        public ProFormaTerm ParseString(string proFormaString) => this.ParseString(proFormaString.AsSpan());
#endif

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
            IList<ProFormaGlobalModification>? globalModifications = null;

            var sequence = new StringBuilder();
            var tag = new StringBuilder();
            bool inTag = false;
            bool inGlobalTag = false;
            bool inCTerminalTag = false;
            int openLeftBrackets = 0;
            int openLeftBraces = 0;
            int? startRange = null;
            int? endRange = null;

            // Don't love doing a global index of performance wise, but would need to restructure things to handle multiple unlocalized tags
            int unlocalizedIndex = proFormaString.IndexOf('?');

            for (int i = 0; i < proFormaString.Length; i++)
            {
                if (unlocalizedIndex == i) continue; // Skip unlocalized separator

                char current = proFormaString[i];

                if (current == '<')
                {
                    inGlobalTag = true;
                }
                else if (current == '>')
                {
                    string tagText = tag.ToString();

                    // Make sure nothing happen before this global mod
                    if (sequence.Length > 0 || unlocalizedTags?.Count > 0 || nTerminalDescriptors?.Count > 0 || tagGroups?.Count > 0)
                        throw new ProFormaParseException("Global modifications must be the first element in ProForma string.");

                    this.HandleGlobalModification(ref tagGroups, ref globalModifications, sequence, startRange, endRange, tagText);

                    inGlobalTag = false;
                    tag.Clear();
                }
                else if (current == '(' && !inTag)
                {
                    if (startRange.HasValue)
                        throw new ProFormaParseException("Overlapping ranges are not allowed.");

                    startRange = sequence.Length;
                }
                else if (current == ')' && !inTag)
                {
                    endRange = sequence.Length;

                    // Ensure a tag comes next
                    if (proFormaString[i + 1] != '[')
                        throw new ProFormaParseException("Ranges must end next to a tag.");
                }
                else if (current == '{' && openLeftBraces++ == 0)
                {
                    inTag = true;
                }
                else if (current == '}' && --openLeftBraces == 0)
                {
                    string tagText = tag.ToString();

                    labileDescriptors = this.ProcessTag(tagText, endRange.HasValue ? startRange : null, sequence.Length - 1, ref tagGroups);

                    inTag = false;
                    tag.Clear();
                }
                else if (!inGlobalTag && current == '[' && openLeftBrackets++ == 0)
                {
                    inTag = true;
                }
                else if (!inGlobalTag && current == ']' && --openLeftBrackets == 0)
                {
                    // Don't allow 2 tags right next to eachother in the sequence
                    if (sequence.Length > 0 && proFormaString.Length > i + 1 && proFormaString[i + 1] == '[')
                        throw new ProFormaParseException("Two tags next to eachother are not allowed.");

                    string tagText = tag.ToString();

                    // Handle terminal modifications and prefix tags
                    if (inCTerminalTag)
                    {
                        cTerminalDescriptors = this.ProcessTag(tagText, -1, -1, ref tagGroups);
                    }
                    else if (sequence.Length == 0 && proFormaString[i + 1] == '-')
                    {
                        nTerminalDescriptors = this.ProcessTag(tagText, -1, -1, ref tagGroups);
                        i++; // Skip the - character
                    }
                    else if (unlocalizedIndex >= i)
                    {
                        // Make sure the prefix came before the N-terminal modification
                        if (nTerminalDescriptors != null)
                            throw new ProFormaParseException($"Unlocalized modification must come before an N-terminal modification.");

                        var descriptors = this.ProcessTag(tagText, -1, -1, ref tagGroups);

                        if (descriptors != null)
                        {
                            int count = 1;

                            // Check for higher count
                            if (proFormaString[i + 1] == '^')
                            {
                                int j = i + 2;
                                while (char.IsDigit(proFormaString[j]))
                                    j++;

#if NETSTANDARD2_1
                                if (!int.TryParse(proFormaString.Slice(i + 2, j - i - 2), out count))
#else
                                if (!int.TryParse(proFormaString.Slice(i + 2, j - i - 2).ToString(), out count))
#endif
                                    throw new ProFormaParseException("Can't process number after '^' character.");

                                i = j - 1; // Point i at the last digit
                            }

                            if (unlocalizedTags == null)
                                unlocalizedTags = new List<ProFormaUnlocalizedTag>();

                            unlocalizedTags.Add(new ProFormaUnlocalizedTag(count, descriptors));
                        }

                        //i++; // skip the ? character
                    }
                    else if (sequence.Length == 0)
                    {
                        throw new ProFormaParseException($"Invalid n terminal descriptor.");
                    }
                    else
                    {
                        this.ProcessTag(tagText, endRange.HasValue ? startRange : null, sequence.Length - 1, ref tags, ref tagGroups);
                    }

                    inTag = false;
                    tag.Clear();

                    // Reset the range if we have processed the tag on the end of it
                    if (endRange.HasValue)
                    {
                        startRange = null;
                        endRange = null;
                    }
                }
                else if (inTag || inGlobalTag)
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
                unlocalizedTags, tagGroups?.Values, globalModifications);
        }

        private void HandleGlobalModification(ref IDictionary<string, ProFormaTagGroup>? tagGroups,
            ref IList<ProFormaGlobalModification>? globalModifications, StringBuilder sequence,
            int? startRange, int? endRange, string tagText)
        {
            // Check for '@' to specify targets
            int atSymbolIndex = tagText.LastIndexOf('@');
            string innerTagText;
            ICollection<char>? targets = null;

            if (atSymbolIndex > 0)
            {
                // Handle fixed modification with targets   
                innerTagText = tagText.Substring(1, atSymbolIndex - 2);

                targets = new List<char>();
                for (int k = atSymbolIndex + 1; k < tagText.Length; k++)
                {
                    if (char.IsUpper(tagText[k]))
                        targets.Add(tagText[k]);
                    else if (tagText[k] != ',')
                        throw new ProFormaParseException($"Unexpected character {tagText[k]} in global modification target list.");
                }
            }
            else
            {
                // No targets, global isotope ... assume whole thing should be read
                innerTagText = tagText;
            }

            var descriptors = this.ProcessTag(innerTagText, endRange.HasValue ? startRange : null, sequence.Length - 1, ref tagGroups);

            if (descriptors != null)
            {
                if (globalModifications == null)
                    globalModifications = new List<ProFormaGlobalModification>();

                globalModifications.Add(new ProFormaGlobalModification(descriptors, targets));
            }
        }

        private void ProcessTag(string tag, int? startIndex, int index, ref IList<ProFormaTag>? tags, ref IDictionary<string, ProFormaTagGroup>? tagGroups)
        {
            var descriptors = this.ProcessTag(tag, startIndex, index, ref tagGroups);

            // Only add a tag if descriptors come back
            if (descriptors != null)
            {
                if (tags == null) tags = new List<ProFormaTag>();

                if (startIndex.HasValue)
                    tags.Add(new ProFormaTag(startIndex.Value, index, descriptors));
                else
                    tags.Add(new ProFormaTag(index, descriptors));
            }
        }

        private IList<ProFormaDescriptor>? ProcessTag(string tag, int? startIndex, int index, ref IDictionary<string, ProFormaTagGroup>? tagGroups)
        {
            IList<ProFormaDescriptor>? descriptors = null;
            var descriptorText = tag.Split('|');

            for (int i = 0; i < descriptorText.Length; i++)
            {
                var (key, evidence, value, group, weight) = this.ParseDescriptor(descriptorText[i].TrimStart());

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

                    // If the group was defined before the sequence, don't include it in the membership
                    if (index >= 0)
                    {
                        if (startIndex.HasValue)
                            tagGroups[group].Members.Add(new ProFormaMembershipDescriptor(startIndex.Value, index, weight));
                        else
                            tagGroups[group].Members.Add(new ProFormaMembershipDescriptor(index, weight));
                    }
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

        private Tuple<ProFormaKey, ProFormaEvidenceType, string, string?, double> ParseDescriptor(string text)
        {
            if (text.Length == 0)
                throw new ProFormaParseException("Cannot have an empty descriptor.");

            // Let's look for a group
            int groupIndex = text.IndexOf('#');
            string? groupName = null;
            double weight = 0.0;

            if (groupIndex >= 0)
            {
                // Check for weight
                int weightIndex = text.IndexOf('(');

                if (weightIndex > groupIndex)
                {
                    // Make sure descriptor ends in ')' to close out weight
                    if (text[text.Length - 1] != ')')
                        throw new ProFormaParseException("Descriptor with weight must end in ')'.");

                    int length = text.Length - weightIndex - 2;

#if NETSTANDARD2_1
                    if (!double.TryParse(text.AsSpan().Slice(weightIndex + 1, length), out weight))
#else
                    if (!double.TryParse(text.AsSpan().Slice(weightIndex + 1, length).ToString(), out weight))
#endif
                        throw new ProFormaParseException($"Could not parse weight: {text.Substring(weightIndex + 1, text.Length - weightIndex - 2)}");

                    groupName = text.Substring(groupIndex + 1, weightIndex - groupIndex - 1);
                }
                else
                {
                    groupName = text.Substring(groupIndex + 1);
                }

                text = text.Substring(0, groupIndex);

                if (string.IsNullOrEmpty(groupName))
                    throw new ProFormaParseException("Group name cannot be empty.");
            }

            // Check for naked group tag
            if (string.IsNullOrEmpty(text))
                return Tuple.Create(ProFormaKey.None, ProFormaEvidenceType.None, text, groupName, weight);

            static ProFormaKey getKey(bool isMass) => isMass ? ProFormaKey.Mass : ProFormaKey.Name;

            // Let's look for a colon
            int colon = text.IndexOf(':');

            if (colon < 0)
            {
                bool isMass2 = text[0] == '+' || text[0] == '-';

                return Tuple.Create(getKey(isMass2), ProFormaEvidenceType.None, text, groupName, weight);
            }

            // Let's see if the bit before the colon is a known key
            string keyText = text.Substring(0, colon).ToLower().Trim();
            bool isMass = text[colon + 1] == '+' || text[colon + 1] == '-';

            return keyText switch
            {
                "formula" => Tuple.Create(ProFormaKey.Formula, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName, weight),
                "glycan" => Tuple.Create(ProFormaKey.Glycan, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName, weight),
                "info" => Tuple.Create(ProFormaKey.Info, ProFormaEvidenceType.None, text.Substring(colon + 1), groupName, weight),

                "mod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.PsiMod, text, groupName, weight),
                "unimod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Unimod, text.ToUpper(), groupName, weight),
                "xlmod" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.XlMod, text, groupName, weight),
                "gno" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Gno, text, groupName, weight),

                // Special case for RESID id, don't inclue bit with colon
                "resid" => Tuple.Create(ProFormaKey.Identifier, ProFormaEvidenceType.Resid, text.Substring(colon + 1), groupName, weight),

                // Handle names and masses
                "u" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Unimod, text.Substring(colon + 1), groupName, weight),
                "m" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.PsiMod, text.Substring(colon + 1), groupName, weight),
                "r" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Resid, text.Substring(colon + 1), groupName, weight),
                "x" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.XlMod, text.Substring(colon + 1), groupName, weight),
                "g" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Gno, text.Substring(colon + 1), groupName, weight),
                "b" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Brno, text.Substring(colon + 1), groupName, weight),
                "obs" => Tuple.Create(getKey(isMass), ProFormaEvidenceType.Observed, text.Substring(colon + 1), groupName, weight),

                _ => Tuple.Create(ProFormaKey.Name, ProFormaEvidenceType.None, text, groupName, weight)
            };
        }
    }
}