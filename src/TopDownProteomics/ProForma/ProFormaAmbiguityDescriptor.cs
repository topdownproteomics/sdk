using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.ProForma
{
    /// <summary>
    /// Member of a tag annotating ambiguous localization of a modification along a proteoform sequence.
    /// </summary>
    public class ProFormaAmbiguityDescriptor
        : ProFormaDescriptor
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ProFormaAmbiguityDescriptor"/> class.
        /// </summary>
        /// <param name="affix"></param>
        /// <param name="group"></param>
        public ProFormaAmbiguityDescriptor(string affix, string group)
            : base(affix, group)
        {
            Affix = affix;
            Group = group;
        }

        /// <summary>
        /// Initializes an instance of <see cref="ProFormaAmbiguityDescriptor"/> with the affix only.
        /// </summary>
        /// <param name="affix"></param>
        public ProFormaAmbiguityDescriptor(string affix)
            : this(affix, string.Empty)
        {
            Affix = affix;
        }

        /// <summary>
        /// String identifying type of ambiguous modification assignment
        /// </summary>
        public string Affix { get; }

        /// <summary>
        /// String uniquely identifying a group of ambiguity descriptors
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Gets string representation of <see cref="ProFormaAmbiguityDescriptor"/>
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return
                Affix == ProFormaAmbiguityAffix.Unlocalized ? Affix :
                Affix == ProFormaAmbiguityAffix.LeftBoundary ? Group + Affix :
                Affix + Group;
        }
    }
}
