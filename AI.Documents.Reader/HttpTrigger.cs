using AI.Documents.Reader.Domain.Entities;
using AI.Documents.Reader.Domain.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AI.Documents.Reader
{
	public class HttpTrigger(ILoggerFactory loggerFactory, IDocumentReaderService documentReaderService)
	{
        private readonly ILogger _logger = loggerFactory.CreateLogger<HttpTrigger>();

		[Function("readUrl")]
		public async Task<DocumentResponse> ReadUrl([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/readFrom")]
			HttpRequestData req, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting function: {FunctionName}", ToString());

			var response = req.CreateResponse(HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

			return await documentReaderService.ReadFromUrlAsync(req.Query["url"]!, cancellationToken);
		}

		[Function("readFile")]
        public async Task<DocumentResponse> ReadFile([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/readFrom/file")]
			HttpRequestData req, [FromBody] DocumentRequest request, CancellationToken cancellationToken)
        {
			_logger.LogInformation("Starting function: {FunctionName}", ToString());

			var response = req.CreateResponse(HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

			return await documentReaderService.ReadFileAsync(request, cancellationToken);
        }
	}
}