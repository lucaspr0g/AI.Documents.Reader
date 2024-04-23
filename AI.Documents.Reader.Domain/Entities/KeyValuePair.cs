namespace AI.Documents.Reader.Domain.Entities
{
	public class KeyValuePair
	{
		public string? Key { get; set; }

		public string? Value { get; set; }

		public float Confidence { get; set; }
	}
}