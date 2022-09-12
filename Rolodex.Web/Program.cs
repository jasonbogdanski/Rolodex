using FluentValidation;
using FluentValidation.AspNetCore;
using HtmlTags;
using MediatR;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using NServiceBus;
using Rolodex.Messages;
using Rolodex.Web.DataStore;
using Rolodex.Web.Infrastructure;
using Rolodex.Web.Infrastructure.Tags;

var builder = WebApplication.CreateBuilder(args);

RegisterServices(builder);

var app = builder.Build();

ConfigureApplication(app);

app.Run();

void RegisterServices(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Host.UseNServiceBus(context =>
    {
        var endpointConfiguration = new EndpointConfiguration("Rolodex.Web");

        var transport = endpointConfiguration.UseTransport<LearningTransport>();

        var routing = transport.Routing();
        routing.RouteToEndpoint(typeof(SendEmail), "Rolodex.Email");

        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");
        endpointConfiguration.SendHeartbeatTo("Particular.ServiceControl");

        var metrics = endpointConfiguration.EnableMetrics();
        metrics.SendMetricDataToServiceControl("Particular.Monitoring", TimeSpan.FromMilliseconds(500));

        return endpointConfiguration;
    });

    webApplicationBuilder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApp(webApplicationBuilder.Configuration.GetSection("AzureAd"));

    webApplicationBuilder.Services.AddAuthorization(options =>
    {
        // By default, all incoming requests will be authorized according to the default policy.
        options.FallbackPolicy = options.DefaultPolicy;
    });

    webApplicationBuilder.Services.AddDbContext<RolodexContext>(options =>
        options.UseSqlServer(webApplicationBuilder.Configuration.GetConnectionString("DefaultConnection")));

    webApplicationBuilder.Services.AddAutoMapper(typeof(Program));

    webApplicationBuilder.Services.AddMediatR(typeof(Program));

    webApplicationBuilder.Services.AddHtmlTags(new TagConventions());

    webApplicationBuilder.Services.AddRazorPages(opt =>
        {
            opt.Conventions.ConfigureFilter(new DbContextTransactionPageFilter());
            opt.Conventions.ConfigureFilter(new ValidatorPageFilter());
        })
        .AddMicrosoftIdentityUI();

    webApplicationBuilder.Services.AddMvc(opt => opt.ModelBinderProviders.Insert(0, new EntityModelBinderProvider()));

    webApplicationBuilder.Services.AddFluentValidationAutoValidation(config =>
    {
        config.DisableDataAnnotationsValidation = true;
    });

    webApplicationBuilder.Services.AddValidatorsFromAssemblyContaining<Program>();

    webApplicationBuilder.Services.AddSingleton<IGuidGenerator, GuidGenerator>();
}

void ConfigureApplication(WebApplication webApplication)
{
// Configure the HTTP request pipeline.
    if (!webApplication.Environment.IsDevelopment())
    {
        webApplication.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();
    webApplication.UseStaticFiles();

    webApplication.UseRouting();

    webApplication.UseAuthentication();
    webApplication.UseAuthorization();

    webApplication.MapRazorPages();
    webApplication.MapControllers();
}


