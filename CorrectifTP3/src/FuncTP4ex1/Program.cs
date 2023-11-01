using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Register your dependencies here
        services.AddBlobServiceClient(configuration => configuration.GetConnectionString("AzureWebJobsStorage"));
        // Add any other necessary services
    })
    .Build();

host.Run();
//nathan test8 pipeline