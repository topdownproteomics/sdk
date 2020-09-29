using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to Inputs element
	/// </summary>
	public class Inputs
	{
		/// <summary>
		/// Gets and sets the sourcefile
		/// </summary>
		public SourceFile SourceFile { get; set; }

		/// <summary>
		/// Gets and sets the search database
		/// </summary>
		public SearchDatabase SearchDatabase { get; set; }

		/// <summary>
		/// Gets and sets the spectradata
		/// </summary>
		public SpectraData SpectraData { get; set; }
	}
}
