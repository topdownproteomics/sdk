using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the cvParam element
	/// </summary>
	public class MzIdentMlCvParam
	{
		/// <summary>
		/// Instantiates with the required parameters
		/// </summary>
		/// <param name="accession"></param>
		/// <param name="reference"></param>
		/// <param name="name"></param>
		public MzIdentMlCvParam(string accession, string reference, string name)
		{
			this.Accession = accession;
			this.Reference = reference;
			this.Name = name;
		}

		/// <summary>
		/// Gets the accession
		/// </summary>
		public string Accession { get;  }

		/// <summary>
		/// Gets the name
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the reference
		/// </summary>
		public string Reference { get; }

		/// <summary>
		/// Gets and sets the unit accession
		/// </summary>
		public string? UnitAccession { get; set; }

		/// <summary>
		/// Gets and sets the unit CV reference
		/// </summary>
		public string? UnitCvRef { get; set; }

		/// <summary>
		/// Gets and sets the unit name
		/// </summary>
		public string? UnitName { get; set; }

		/// <summary>
		/// Gets and sets the value
		/// </summary>
		public string? Value { get; set; }
	}
}
