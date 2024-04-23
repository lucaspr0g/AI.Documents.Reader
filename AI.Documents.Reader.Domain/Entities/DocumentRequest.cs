namespace AI.Documents.Reader.Domain.Entities
{
	public class DocumentRequest
	{
        public string? File { get; set; }

		public bool IsValid()
		{
			return !string.IsNullOrWhiteSpace(File);
		}
    }
}