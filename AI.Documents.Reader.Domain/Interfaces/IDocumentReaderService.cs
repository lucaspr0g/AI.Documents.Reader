using AI.Documents.Reader.Domain.Entities;

namespace AI.Documents.Reader.Domain.Interfaces
{
	public interface IDocumentReaderService
	{
		Task<DocumentResponse> ReadFromUrlAsync(string url, CancellationToken cancellationToken);
		Task<DocumentResponse> ReadFileAsync(DocumentRequest req, CancellationToken cancellationToken);
	}
}