// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;

var host = Host.CreateDefaultBuilder(args);

RegisterServices(host);

var built = host.Build();

built.Run();

void RegisterServices(IHostBuilder hostBuilder)
{
    hostBuilder.ConfigureServices((context, services)  =>
    {
        var configuration = context.Configuration;
        services.AddOptions<EmailCommunicationOptions>()
            .Bind(configuration.GetSection("EmailCommunication"));
    });

    hostBuilder.ConfigureAppConfiguration(config =>
    {
        config.AddUserSecrets<Program>();
    });

    hostBuilder.UseNServiceBus(context =>
    {
        var endpointConfiguration = new EndpointConfiguration("Rolodex.Email");

        endpointConfiguration.UseTransport<LearningTransport>();

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

        var metrics = endpointConfiguration.EnableMetrics();
        metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

        return endpointConfiguration;
    });
}

public record EmailCommunicationOptions
{
    public string ConnectionString { get; set; } = null!;
    public string ReplyToEmail { get; set; } = null!;
}
