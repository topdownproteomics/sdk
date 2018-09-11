﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Parser for the ProForma proteoform notation (link here to published manuscript)
    /// </summary>
    public class ProFormaParser
    {
        /// <summary>
        /// Parses the ProForma string.
        /// </summary>
        /// <param name="proFormaString">The pro forma string.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">proFormaString</exception>
        /// <exception cref="ProFormaParseException">
        /// X is not allowed.
        /// </exception>
        public ProFormaTerm ParseString(string proFormaString)
        {
            if (string.IsNullOrEmpty(proFormaString))
                throw new ArgumentNullException(nameof(proFormaString));

            List<ProFormaTag> tags = null;
            IList<ProFormaDescriptor> nTerminalDescriptors = null;
            IList<ProFormaDescriptor> cTerminalDescriptors = null;

            var sequence = new StringBuilder();
            var tag = new StringBuilder();
            bool inTag = false;
            bool inCTerminalTag = false;
            string prefixTag = null;

            for (int i = 0; i < proFormaString.Length; i++)
            {
                char current = proFormaString[i];

                if (current == '[')
                    inTag = true;
                else if (current == ']')
                {
                    // Handle terminal modifications and prefix tags
                    if (inCTerminalTag)
                    {
                        cTerminalDescriptors = this.ProcessTag(tag.ToString(), prefixTag);
                    }
                    else if (sequence.Length == 0 && proFormaString[i + 1] == '-')
                    {
                        nTerminalDescriptors = this.ProcessTag(tag.ToString(), prefixTag);
                        i++; // Skip the - character
                    }
                    else if (sequence.Length == 0 && proFormaString[i + 1] == '+')
                    {
                        // Make sure the prefix came before the N-terminal modification
                        if (nTerminalDescriptors != null)
                            throw new ProFormaParseException($"Prefix tag must come before an N-terminal modification.");

                        prefixTag = tag.ToString();
                        i++; // Skip the + character
                    }
                    else
                    {
                        if (tags == null)
                            tags = new List<ProFormaTag>();

                        tags.Add(this.ProcessTag(tag.ToString(), sequence.Length - 1, prefixTag));
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
                    else if (current == 'X')
                        throw new ProFormaParseException("X is not allowed.");

                    sequence.Append(current);
                }
            }

            return new ProFormaTerm(sequence.ToString(), nTerminalDescriptors, cTerminalDescriptors, tags);
        }

        private ProFormaTag ProcessTag(string tag, int index, string prefixTag)
        {
            var descriptors = this.ProcessTag(tag, prefixTag);

            return new ProFormaTag(index, descriptors);
        }

        private IList<ProFormaDescriptor> ProcessTag(string tag, string prefixTag)
        {
            var descriptors = new List<ProFormaDescriptor>();
            var descriptorText = tag.Split('|');

            for (int i = 0; i < descriptorText.Length; i++)
            {
                int colon = descriptorText[i].IndexOf(':');
                string key = colon < 0 ? "" : descriptorText[i].Substring(0, colon).TrimStart();
                string value = descriptorText[i].Substring(colon + 1); // values may have colons

                // Prefix tag
                if (!string.IsNullOrEmpty(prefixTag))
                {
                    if (key.Length > 0)
                        throw new ProFormaParseException("Cannot use key-value pairs with a prefix key");

                    descriptors.Add(new ProFormaDescriptor(prefixTag, value));
                }

                // typical descriptor
                else if (key.Length > 0)
                {
                    descriptors.Add(new ProFormaDescriptor(key, value));
                }

                // ambiguity descriptors
                else if (value.Contains(ProFormaAmbiguityAffix.Unlocalized))
                {
                    if (value.Length > ProFormaAmbiguityAffix.Unlocalized.Length)
                        throw new ProFormaParseException("Cannot use group string with an unlocalized affix, " + ProFormaAmbiguityAffix.Unlocalized + ".");

                    descriptors.Add(new ProFormaAmbiguityDescriptor(value));
                }
                else if (value.StartsWith(ProFormaAmbiguityAffix.PossibleSite))
                {
                    string group = value.Substring(ProFormaAmbiguityAffix.PossibleSite.Length);
                    if (group.Length == 0)
                        throw new ProFormaParseException("Cannot use empty group name following the possible site ambiguity prefix, " + ProFormaAmbiguityAffix.PossibleSite + ".");

                    descriptors.Add(new ProFormaAmbiguityDescriptor(ProFormaAmbiguityAffix.PossibleSite, group));
                }
                else if (value.StartsWith(ProFormaAmbiguityAffix.RightBoundary))
                {
                    string group = value.Substring(ProFormaAmbiguityAffix.RightBoundary.Length);
                    if (group.Length == 0)
                        throw new ProFormaParseException("Cannot use empty group name following the range prefix, " + ProFormaAmbiguityAffix.RightBoundary + ".");

                    descriptors.Add(new ProFormaAmbiguityDescriptor(ProFormaAmbiguityAffix.RightBoundary, group));
                }
                else if (value.EndsWith(ProFormaAmbiguityAffix.LeftBoundary))
                {
                    string group = value.Substring(0, value.Length - ProFormaAmbiguityAffix.LeftBoundary.Length + 1);
                    if (group.Length == 0)
                        throw new ProFormaParseException("Cannot use empty group name before the range suffix, " + ProFormaAmbiguityAffix.LeftBoundary + ".");

                    descriptors.Add(new ProFormaAmbiguityDescriptor(ProFormaAmbiguityAffix.LeftBoundary, group));
                }

                // keyless descriptor (UniMod annotation)
                else if (value.Length > 0)
                {
                    descriptors.Add(new ProFormaDescriptor(value));
                }
                else
                {
                    throw new ProFormaParseException("Empty descriptor within tag " + tag);
                }
            }

            return descriptors;
        }
    }
}