using AI.Documents.Reader.Domain.Interfaces;
using AI.Documents.Reader.Domain.Services.v1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables()
	.Build();

var host = new HostBuilder()
	.ConfigureFunctionsWorkerDefaults()
	.ConfigureServices(s =>
	{
		s.AddScoped<IDocumentReaderService, DocumentReaderService>();
		s.AddSingleton(configuration);
		s.AddMemoryCache();
	})
	.Build();

host.Run();
