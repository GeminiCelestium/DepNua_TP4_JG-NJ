using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.AddJsonFile("config.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetService<IConfiguration>();

                // Ajoutez la télémétrie Application Insights en utilisant IConfiguration
                services.AddApplicationInsightsTelemetry(configuration["ApplicationInsights:InstrumentationKey"]);
            })
            .Build();

host.Run();

//nathan test8 pipeline