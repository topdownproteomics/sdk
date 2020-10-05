using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the Modification element
	/// </summary>
	public class MzIdentMlModification
	{
		/// <summary>
		/// Gets and sets the location
		/// </summary>
		public int Location { get; set; }

		/// <summary>
		/// Gets and sets the monoisotopic mass delta
		/// </summary>
		public double MonoisotopicMassDelta { get; set; }

		/// <summary>
		/// Gets and sets the accession
		/// </summary>
		public string Accession { get; set; }

		/// <summary>
		/// Gets and sets the name
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets and sets the reference
		/// </summary>
		public string Reference { get; set; }
	}
}
