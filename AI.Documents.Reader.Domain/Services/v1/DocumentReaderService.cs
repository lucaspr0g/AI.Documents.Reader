using AI.Documents.Reader.Domain.Interfaces;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure;
using Microsoft.Extensions.Configuration;
using AI.Documents.Reader.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace AI.Documents.Reader.Domain.Services.v1
{
	public class DocumentReaderService : IDocumentReaderService
	{
		private readonly int _ttl;
		private readonly string _modelId;

		private readonly IMemoryCache _cache;
		private readonly DocumentAnalysisClient _client;

		public DocumentReaderService(IConfiguration configuration, IMemoryCache cache)
		{
			_cache = cache;
			_modelId = "prebuilt-document";
			_ttl = Convert.ToInt32(configuration["TTL"]!);

			var credential = new AzureKeyCredential(configuration["AiServiceApiKey"]!);
			_client = new DocumentAnalysisClient(new Uri(configuration["AiServiceEndpoint"]!), credential);
		}

		public async Task<IEnumerable<DocumentResponse>> ReadFromUrlAsync(string url, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(url))
				throw new ArgumentNullException(nameof(url));

			_cache.TryGetValue(url, out IEnumerable<DocumentResponse>? response);

			if (response is not null)
				return response;

			var uri = new Uri(url);

			var operation = await _client.AnalyzeDocumentFromUriAsync(WaitUntil.Completed,
				_modelId, uri, default, cancellationToken);

			ValidateResult(operation);

			response = MapResponse(operation.Value);
			_cache.Set(uri, response, DateTime.Now.AddMinutes(_ttl));

			return response;
		}

		public async Task<IEnumerable<DocumentResponse>> ReadFileAsync(DocumentRequest req, CancellationToken cancellationToken)
		{
			if (!req.IsValid())
				throw new ArgumentNullException(nameof(req));

			_cache.TryGetValue(req.File!, out IEnumerable<DocumentResponse>? response);

			if (response is not null)
				return response;

			var bytes = Convert.FromBase64String(req.File!);
			var stream = new MemoryStream(bytes);

			var operation = await _client.AnalyzeDocumentAsync(WaitUntil.Completed,
				_modelId, stream, default, cancellationToken);

			ValidateResult(operation);

			response = MapResponse(operation.Value);
			_cache.Set(req.File!, response, DateTime.Now.AddMinutes(_ttl));

			return response;
		}

		private static void ValidateResult(AnalyzeDocumentOperation operation)
		{
			if (!operation.HasValue)
				throw new InvalidDataException("Unexpected error.");
		}

		private static IEnumerable<DocumentResponse> MapResponse(AnalyzeResult result)
		{
			foreach (var key in result.KeyValuePairs)
			{
				yield return new DocumentResponse
				{
					Key = key.Key?.Content,
					Value = key.Value?.Content
				};
			}
		}
	}
}