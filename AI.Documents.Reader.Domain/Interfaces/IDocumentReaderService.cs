using AI.Documents.Reader.Domain.Entities;

namespace AI.Documents.Reader.Domain.Interfaces
{
	public interface IDocumentReaderService
	{
		Task<IEnumerable<DocumentResponse>> ReadFromUrlAsync(string url, CancellationToken cancellationToken);
		Task<IEnumerable<DocumentResponse>> ReadFileAsync(DocumentRequest req, CancellationToken cancellationToken);
	}
}