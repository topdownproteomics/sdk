using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the ProteinDetectionList element
	/// </summary>
	public class ProteinDetectionList
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the protein ambiguity groups
		/// </summary>
		public List<ProteinAmbiguityGroup> ProteinAmbiguityGroups { get; set; }
	}
}
