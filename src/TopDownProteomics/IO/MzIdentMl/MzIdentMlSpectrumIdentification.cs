namespace TopDownProteomics.IO.MzIdentMl
{
	internal class MzIdentMlSpectrumIdentification
	{
		public MzIdentMlSpectrumIdentification(string id, string spectrumIdentificationProtocolId, string spectrumIdentificationListId, string inputSpectraId, string searchDatabaseId)
		{
			Id = id;
			SpectrumIdentificationProtocolId = spectrumIdentificationProtocolId;
			SpectrumIdentificationListId = spectrumIdentificationListId;
			InputSpectraId = inputSpectraId;
			SearchDatabaseId = searchDatabaseId;
		}

		public string Id { get; }
		public string SpectrumIdentificationProtocolId { get; }
		public string SpectrumIdentificationListId { get; }
		public string InputSpectraId { get; }
		public string SearchDatabaseId { get; }
	}
}
