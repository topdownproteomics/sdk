﻿using System.Collections.Generic;

namespace TestLibNamespace.ProForma
{
    /// <summary>
    /// The specified way of writing a localized modification. Everything between ‘[‘ and ‘]’ (inclusive). A collection of descriptors.
    /// </summary>
    public class ProFormaTag
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProFormaTag"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="descriptors">The descriptors.</param>
        public ProFormaTag(int index, ICollection<ProFormaDescriptor> descriptors)
        {
            Index = index;
            Descriptors = descriptors;
        }

        #endregion Constructor

        #region Public Properties

        /// <summary>
        /// Gets the zero-based index in the sequence.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Gets the descriptors.
        /// </summary>
        public ICollection<ProFormaDescriptor> Descriptors { get; }

        #endregion Public Properties

    }
}