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
		public async Task<HttpResponseData> ReadUrl([HttpTrigger(AuthorizationLevel.Function, "get", Route = "v1/readFrom")]
			HttpRequestData req, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting function: {FunctionName}", ToString());

			try
			{
				var result = await documentReaderService.ReadFromUrlAsync(req.Query["url"]!, cancellationToken);
				return await GenerateOkResponse(req, result, cancellationToken);
			}
			catch (ArgumentException ex)
			{
				return await GenerateBadRequestResponse(req, ex.Message, cancellationToken);
			}
			catch (InvalidDataException)
			{
				return GenerateNoContentRequestResponse(req);
			}
		}

		[Function("readFile")]
		public async Task<HttpResponseData> ReadFile([HttpTrigger(AuthorizationLevel.Function, "post", Route = "v1/readFrom/file")]
			HttpRequestData req, [FromBody] DocumentRequest request, CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting function: {FunctionName}", ToString());

			try
			{
				var result = await documentReaderService.ReadFileAsync(request, cancellationToken);
				return await GenerateOkResponse(req, result, cancellationToken);
			}
			catch (ArgumentException ex)
			{
				return await GenerateBadRequestResponse(req, ex.Message, cancellationToken);
			}
			catch (InvalidDataException)
			{
				return GenerateNoContentRequestResponse(req);
			}
		}

		private static async Task<HttpResponseData> GenerateOkResponse(HttpRequestData req, object obj, CancellationToken cancellationToken)
		{
			var response = req.CreateResponse(HttpStatusCode.OK);
			await response.WriteAsJsonAsync(obj, cancellationToken);

			return response;
		}

		private static async Task<HttpResponseData> GenerateBadRequestResponse(HttpRequestData req, string message, CancellationToken cancellationToken)
		{
			var response = req.CreateResponse(HttpStatusCode.BadRequest);
			response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
			await response.WriteStringAsync(message, cancellationToken);

			return response;
		}

		private static HttpResponseData GenerateNoContentRequestResponse(HttpRequestData req)
		{
			var response = req.CreateResponse(HttpStatusCode.NoContent);
			return response;
		}
	}
}