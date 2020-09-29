using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to SourceFile elements
	/// </summary>
	public class SourceFile
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public CvParam FileFormat { get; set; }
	}
}
