using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentML.Models
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationResult element
	/// </summary>
	public class SpectrumIdentificationResult
	{
		/// <summary>
		/// Gets and sets the id
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// Gets and sets the spectrum id
		/// </summary>
		public string SpectrumId { get; set; }

		/// <summary>
		/// Gets and sets the input spectra data id
		/// </summary>
		public string InputSpectraDataId { get; set; }

		/// <summary>
		/// Gets and sets the spectrum identification items
		/// </summary>
		public List<SpectrumIdentificationItem> SpectrumIdentificationItems { get; set; } = new List<SpectrumIdentificationItem>();

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<CvParam> CvParams { get; set; } = new List<CvParam>();
	}
}
