namespace TopDownProteomics.IO.MzIdentMl
{
	internal class MzIdentMlProteinDetection
	{
		public MzIdentMlProteinDetection(string id, string proteinDetectionProtocolId, string proteinDetectionListId, string spectrumIdentificationListId)
		{
			Id = id;
			ProteinDetectionProtocolId = proteinDetectionProtocolId;
			ProteinDetectionListId = proteinDetectionListId;
			SpectrumIdentificationListId = spectrumIdentificationListId;
		}

		public string Id { get; }
		public string ProteinDetectionProtocolId { get; }
		public string ProteinDetectionListId { get; }
		public string SpectrumIdentificationListId { get; }
	}
}
