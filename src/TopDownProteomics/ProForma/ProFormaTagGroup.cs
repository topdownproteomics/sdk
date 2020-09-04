using System.Collections.Generic;

namespace TopDownProteomics.ProForma
{
    /// <summary>A tag that is spread across multiple distinct sites.</summary>
    public class ProFormaTagGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTagGroup"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="members">The members.</param>
        public ProFormaTagGroup(string name, ProFormaKey key, string value, IList<ProFormaMembershipDescriptor> members)
        {
            Name = name;
            Key = key;
            Value = value;
            Members = members;
        }

        /// <summary>The name of the group.</summary>
        public string Name { get; }

        /// <summary>The key.</summary>
        public ProFormaKey Key { get; }

        /// <summary>The value.</summary>
        public string Value { get; }

        /// <summary>The members of the group.</summary>
        public IList<ProFormaMembershipDescriptor> Members { get; }
    }
}