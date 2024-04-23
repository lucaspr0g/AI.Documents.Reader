namespace AI.Documents.Reader.Domain.Entities
{
	public class DocumentResponse
	{
		public List<string> Tables { get; set; }

		public List<KeyValuePair> KeyValuePairs { get; set; }

        public DocumentResponse()
        {
			Tables = [];
			KeyValuePairs = [];
        }
    }
}