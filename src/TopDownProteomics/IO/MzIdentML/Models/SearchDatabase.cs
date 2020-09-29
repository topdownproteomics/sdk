using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the SearchDatabase element
	/// </summary>
	public class SearchDatabase
	{
		/// <summary>
		/// Gets and sets the Id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public string Location { get; set; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the number of sequences
		/// </summary>
		public int NumberOfSequences { get; set; }

		/// <summary>
		/// Gets and sets the number of residues
		/// </summary>
		public int NumberOfResidues { get; set; }

		/// <summary>
		/// Gets and sets the version info
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Gets and sets the file format
		/// </summary>
		public CvParam FileFormat { get; set; }

		/// <summary>
		/// Gets and sets the database name
		/// </summary>
		public UserParam DatabaseName { get; set; }

		/// <summary>
		/// Gets and sets database params
		/// </summary>
		public List<CvParam> DatabaseParams { get; set; } = new List<CvParam>();
	}
}
