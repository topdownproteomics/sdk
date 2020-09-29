using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to Measure elements
	/// </summary>
	public class FragmentationMeasure
	{
		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the measure
		/// </summary>
		public CvParam Measure { get; set; }
	}
}
