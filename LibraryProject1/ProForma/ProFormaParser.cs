﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TestLibNamespace.ProForma
{
    /// <summary>
    /// Parser for the ProForma proteoform notation (link here to published manuscript)
    /// </summary>
    public class ProFormaParser
    {

        #region Public Method

        public ProFormaTerm ParseString(string proFormaString)
        {
            if (string.IsNullOrEmpty(proFormaString))
                throw new ArgumentNullException(nameof(proFormaString));

            List<ProFormaTag> tags = null;
            var sequence = new StringBuilder();
            var tag = new StringBuilder();
            bool inTag = false;

            for (int i = 0; i < proFormaString.Length; i++)
            {
                char current = proFormaString[i];

                if (current == '[')
                    inTag = true;
                else if (current == ']')
                {
                    if (tags == null)
                        tags = new List<ProFormaTag>();

                    tags.Add(this.ProcessTag(tag.ToString(), sequence.Length - 1));

                    inTag = false;
                    tag.Clear();
                }
                else if (inTag)
                    tag.Append(current);
                else
                    sequence.Append(current);
            }

            return new ProFormaTerm(sequence.ToString(), tags);
        }

        #endregion Public Method

        #region Private Method

        private ProFormaTag ProcessTag(string tag, int index)
        {
            var descriptors = new List<ProFormaDescriptor>();

            var descriptorText = tag.Split('|');

            for (int i = 0; i < descriptorText.Length; i++)
            {
                int colon = descriptorText[i].IndexOf(':');
                string key = colon < 0 ? "" : descriptorText[i].Substring(0, colon);
                string value = descriptorText[i].Substring(colon + 1); // values may have colons

                if (key.Length > 0)
                    descriptors.Add(new ProFormaDescriptor(key, value));
                else if (value.Length > 0)
                    descriptors.Add(new ProFormaDescriptor(value));
                else
                    throw new ProFormaParseException("Empty descriptor within tag " + tag);
            }

            return new ProFormaTag(index, descriptors);
        }

        #endregion Private Method

    }
}