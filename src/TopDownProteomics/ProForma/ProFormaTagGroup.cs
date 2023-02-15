using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>A tag that is spread across multiple distinct sites.</summary>
    public class ProFormaTagGroup : IProFormaDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, string value, IList<ProFormaMembershipDescriptor> members)
            : this(name, key, ProFormaEvidenceType.None, value, members) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="evidenceType">Type of the evidence.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, ProFormaEvidenceType evidenceType, string value,
            IList<ProFormaMembershipDescriptor> members)
        {
            this.Name = name;
            this.Key = key;
            this.EvidenceType = evidenceType;
            this.Value = value;
            this.Members = members;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        /// <param name="preferredLocation">The zero-based member index for which member is the preferred location.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, string value, IList<ProFormaMembershipDescriptor> members, int preferredLocation)
            : this(name, key, ProFormaEvidenceType.None, value, members, preferredLocation) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="evidenceType">Type of the evidence.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        /// <param name="preferredLocation">The zero-based member index for which member is the preferred location.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, ProFormaEvidenceType evidenceType, string value,
            IList<ProFormaMembershipDescriptor> members, int preferredLocation)
        {
            this.Name = name;
            this.Key = key;
            this.EvidenceType = evidenceType;
            this.Value = value;
            this.Members = members;
            this.PreferredLocation = preferredLocation;
        }

        /// <summary>The name of the group.</summary>
        public string Name { get; }

        /// <summary>The key.</summary>
        public ProFormaKey Key { get; internal set; }

        /// <summary>The type of the evidence.</summary>
        public ProFormaEvidenceType EvidenceType { get; internal set; }

        /// <summary>The value.</summary>
        public string Value { get; internal set; }

        /// <summary>The members of the group.</summary>
        public IList<ProFormaMembershipDescriptor> Members { get; }

        /// <summary>The preferred location for the modification description.</summary>
        public int PreferredLocation { get; internal set; }
    }
}