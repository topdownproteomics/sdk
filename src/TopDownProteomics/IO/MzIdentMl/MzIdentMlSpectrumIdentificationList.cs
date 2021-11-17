using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentMl
{
	/// <summary>
	/// A collection of spectrum identifications
	/// </summary>
	public class MzIdentMlSpectrumIdentificationList
	{
		/// <summary>
		/// Creates a spectrum identification list
		/// </summary>
		/// <param name="id"></param>
		/// <param name="measures"></param>
		/// <param name="spectrumIdentificationResults"></param>
		/// <param name="proteinDetectionProtocol"></param>
		/// <param name="spectrumIdentificationProtocol"></param>
		/// <param name="name"></param>
		/// <param name="sequencesSearched"></param>
		public MzIdentMlSpectrumIdentificationList(string id, List<MzIdentMlFragmentationMeasure> measures, List<MzIdentMlSpectrumIdentificationResult> spectrumIdentificationResults, MzIdentMlProteinDetectionProtocol proteinDetectionProtocol, MzIdentMlSpectrumIdentificationProtocol spectrumIdentificationProtocol, string? name = null, int? sequencesSearched = null)
		{
			Id = id;
			Measures = measures;
			SpectrumIdentificationResults = spectrumIdentificationResults;
			ProteinDetectionProtocol = proteinDetectionProtocol;
			SpectrumIdentificationProtocol = spectrumIdentificationProtocol;
			Name = name;
			SequencesSearched = sequencesSearched;
		}

		/// <summary>
		/// Gets the ID
		/// </summary>
		public string Id { get; }

		/// <summary>
		/// Gets the measures
		/// </summary>
		public List<MzIdentMlFragmentationMeasure> Measures { get; }

		/// <summary>
		/// Gets the spectrum identification results
		/// </summary>
		public List<MzIdentMlSpectrumIdentificationResult> SpectrumIdentificationResults { get; }

		/// <summary>
		/// Gets the protein detection protocol
		/// </summary>
		public MzIdentMlProteinDetectionProtocol ProteinDetectionProtocol { get; }

		/// <summary>
		/// Gets the spectrum identification protocol
		/// </summary>
		public MzIdentMlSpectrumIdentificationProtocol SpectrumIdentificationProtocol { get; }

		/// <summary>
		/// Gets the name
		/// </summary>
		public string? Name { get; }

		/// <summary>
		/// Gets the number of sequences searched
		/// </summary>
		public int? SequencesSearched { get; }
	}
}
