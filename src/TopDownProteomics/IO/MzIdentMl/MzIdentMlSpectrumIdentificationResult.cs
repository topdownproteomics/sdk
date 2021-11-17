using System.Collections.Generic;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// Corresponds to the SpectrumIdentificationResult element
	/// </summary>
	public class MzIdentMlSpectrumIdentificationResult
	{
		/// <summary>
		/// Instantiates with required parameters
		/// </summary>
		/// <param name="id"></param>
		/// <param name="spectrumId"></param>
		/// <param name="spectrumScanNumber"></param>
		/// <param name="spectralData"></param>
		/// <param name="inputSpectraDataId"></param>
		/// <param name="spectrumIdentificationItems"></param>
		public MzIdentMlSpectrumIdentificationResult(string id, string spectrumId, int? spectrumScanNumber, MzIdentMlSpectraData spectralData, string inputSpectraDataId, List<MzIdentMlSpectrumIdentificationItem> spectrumIdentificationItems)
		{
			Id = id;
			SpectrumId = spectrumId;
			SpectrumScanNumber = spectrumScanNumber;
			SpectralData = spectralData;
			InputSpectraDataId = inputSpectraDataId;
			SpectrumIdentificationItems = spectrumIdentificationItems;
		}
		/// <summary>
		/// Gets the id
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the spectrum id
		/// </summary>
		public string SpectrumId { get; }

		/// <summary>
		/// Gets  the spectrum id
		/// </summary>
		public int? SpectrumScanNumber { get; }

		/// <summary>
		/// Gets the spectral data
		/// </summary>
		public MzIdentMlSpectraData SpectralData { get; }

		/// <summary>
		/// Gets the input spectra data id
		/// </summary>
		public string InputSpectraDataId { get; }

		/// <summary>
		/// Gets the spectrum identification items
		/// </summary>
		public List<MzIdentMlSpectrumIdentificationItem> SpectrumIdentificationItems { get; }

		/// <summary>
		/// Gets and sets the cvParams
		/// </summary>
		public List<MzIdentMlCvParam>? CvParams { get; set; }

		/// <summary>
		/// Gets and sets the userParams
		/// </summary>
		public List<MzIdentMlUserParam>? UserParams { get; set; }
	}
}
