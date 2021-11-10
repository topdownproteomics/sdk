using System;
using System.Collections.Generic;
using System.Text;

namespace TopDownProteomics.IO.MzIdentMl
{
	internal class MzIdentMlAnalysisCollection
	{
		public MzIdentMlAnalysisCollection(List<MzIdentMlSpectrumIdentification> spectrumIdentifications, List<MzIdentMlProteinDetection> proteinDetections)
		{
			SpectrumIdentifications = spectrumIdentifications;
			ProteinDetections = proteinDetections;
		}

		public List<MzIdentMlSpectrumIdentification> SpectrumIdentifications { get; }
		public List<MzIdentMlProteinDetection> ProteinDetections { get; }
	}
}
