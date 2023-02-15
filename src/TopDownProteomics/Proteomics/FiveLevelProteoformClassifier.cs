using System;
using System.Collections.Generic;
using System.Linq;
using TopDownProteomics.ProForma;

namespace TopDownProteomics.Proteomics
{
    /// <summary>
    /// This static class contains methods for classifying proteoform identifications as defined in the following publication:
    /// Smith, L.M., Thomas, P.M., Shortreed, M.R. et al. A five-level classification system for proteoform identifications. 
    /// Nat Methods 16, 939–940 (2019). https://doi.org/10.1038/s41592-019-0573-x
    /// </summary>
    public static class FiveLevelProteoformClassifier
    {
        /// <summary>
        /// Determine 5-level proteoform classification from ProForma
        /// </summary>
        /// <param name="parsedProteoform">ProForma proteoform </param>
        /// <param name="genes">List of genes for this proForma </param>
        /// <returns></returns>
        public static string ClassifyProForma(ProFormaTerm parsedProteoform, List<string> genes)
        {
            bool ptmLocalized = ProFormaHasLocalizedPTMs(parsedProteoform);
            bool ptmIdentified = ProFormaHasIdentifiedPTMs(parsedProteoform);
            bool sequenceIdentified = ProFormaHasSequenceIdentified(parsedProteoform);
            bool geneIdentified = genes.Count == 1;
            return GetProteoformClassification(ptmLocalized, ptmIdentified, sequenceIdentified, geneIdentified);
        }

        /// <summary>
        /// Determine if proteoform has all of its PTMs localized
        /// </summary>
        /// <param name="proteoform"></param>
        /// <returns></returns>
        private static bool ProFormaHasLocalizedPTMs(ProFormaTerm proteoform)
        {
            //check unlocalized tags 
            if (proteoform.UnlocalizedTags == null || proteoform.UnlocalizedTags.Count == 0)
            {
                //check tag groups
                if (proteoform.TagGroups != null && proteoform.TagGroups.Count != 0)
                {
                    foreach (ProFormaTagGroup group in proteoform.TagGroups)
                    {
                        if (group.Members != null && group.Members.Count > 1)
                        {
                            return false;
                        }
                    }
                }
                //check tags
                if (proteoform.Tags != null)
                {
                    foreach (ProFormaTag tag in proteoform.Tags)
                    {
                        if (tag.ZeroBasedStartIndex != tag.ZeroBasedEndIndex && tag.Descriptors.Count > 0)
                        {
                            return false;
                        }
                    }
                }
                //check labile (inherently unlocalized)
                if (proteoform.LabileDescriptors != null && proteoform.LabileDescriptors.Count != 0)
                {
                    return false;
                }
                //don't need to check N- or C-term, those are localized
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determine if proteoform has all of its PTMs identified
        /// </summary>
        /// <param name="proteoform"></param>
        /// <returns></returns>
        private static bool ProFormaHasIdentifiedPTMs(ProFormaTerm proteoform)
        {
            //if we observed some tags, check that they have names and/or formulas (identified) instead of just a mass shift (not identified)
            if (proteoform.Tags != null)
            {
                foreach (var tag in proteoform.Tags)
                {
                    if (AmbiguousPtmFromDescriptor(tag.Descriptors))
                    {
                        return false;
                    }
                }
            }
            if (proteoform.UnlocalizedTags != null)
            {
                foreach (var tag in proteoform.UnlocalizedTags)
                {
                    if (AmbiguousPtmFromDescriptor(tag.Descriptors))
                    {
                        return false;
                    }
                }
            }
            if (proteoform.TagGroups != null)
            {
                foreach (var tag in proteoform.TagGroups)
                {
                    if (AmbiguousPtmFromKey(tag.Key))
                    {
                        return false;
                    }
                }
            }
            if (proteoform.NTerminalDescriptors != null && AmbiguousPtmFromDescriptor(proteoform.NTerminalDescriptors))
            {
                return false;
            }
            if (proteoform.CTerminalDescriptors != null && AmbiguousPtmFromDescriptor(proteoform.CTerminalDescriptors))
            {
                return false;
            }
            if (proteoform.LabileDescriptors != null && AmbiguousPtmFromDescriptor(proteoform.LabileDescriptors))
            {
                return false;
            }
            //All PTMs had an ID (or there were no PTMs)
            return true;
        }

        /// <summary>
        /// Given a PTM descriptor, is the PTM unknown (i.e. not have an ID)?
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private static bool AmbiguousPtmFromDescriptor(IList<ProFormaDescriptor>? descriptor)
        {
            if (descriptor is null || descriptor.Count == 0)
            {
                return false;
            }
            else if (descriptor.Count == 1)
            {
                return AmbiguousPtmFromKey(descriptor[0].Key);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Given a PTM key, is the PTM unknown (i.e. not have an ID)?
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static bool AmbiguousPtmFromKey(ProFormaKey key)
        {
            return (key.Equals(ProFormaKey.Mass) || key.Equals(ProFormaKey.None));
        }

        /// <summary>
        /// Determine if proteoform has its entire sequence identified
        /// </summary>
        /// <param name="proteoform"></param>
        /// <returns></returns>
        private static bool ProFormaHasSequenceIdentified(ProFormaTerm proteoform)
        {
            //easier to check if the sequence is ambiguous and then reverse the bool to find if the sequence is not ambiguous.
            bool isAmbiguous = proteoform.Tags?.Any(x => x.HasAmbiguousSequence) ?? false;

            bool containsAmbiguousCharacter = proteoform.Sequence.Contains('X') || proteoform.Sequence.Contains('J') ||
                proteoform.Sequence.Contains('B') || proteoform.Sequence.Contains('Z');

            return !(isAmbiguous || containsAmbiguousCharacter);
        }

        /// <summary>
        /// Determine proteoform level between 1 (know everything) and 5 (only know the mass)
        /// as defined in the publication:
        /// Smith, L.M., Thomas, P.M., Shortreed, M.R. et al. A five-level classification system for proteoform identifications. 
        /// Nat Methods 16, 939–940 (2019). https://doi.org/10.1038/s41592-019-0573-x
        /// </summary>
        /// <param name="ptmLocalized">Is the PTM localized?</param>
        /// <param name="ptmIdentified">Do we know what the PTM is, or is it ambiguous (or an unknown mass shift?)</param>
        /// <param name="sequenceIdentified">Do we know the proteoform sequence, or is it ambiguous?</param>
        /// <param name="geneIdentified">Do we know which gene produced this proteoform?</param>
        /// <returns></returns>
        public static string GetProteoformClassification(bool ptmLocalized, bool ptmIdentified, bool sequenceIdentified, bool geneIdentified)
        {
            int sum = Convert.ToInt16(ptmLocalized) + Convert.ToInt16(ptmIdentified) + Convert.ToInt16(sequenceIdentified) + Convert.ToInt16(geneIdentified);
            if (sum == 3) //level 2, but is it A, B, C, or D?
            {
                if (!ptmLocalized)
                {
                    return "2A";
                }
                else if (!ptmIdentified)
                {
                    return "2B";
                }
                else if (!sequenceIdentified)
                {
                    return "2C";
                }
                else //if (!geneIdentified)
                {
                    return "2D";
                }
            }
            else
            {
                return (5 - sum).ToString();
            }
        }
    }
}