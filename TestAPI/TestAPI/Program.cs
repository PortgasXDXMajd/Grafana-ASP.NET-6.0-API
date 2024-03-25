using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Prometheus;
using Prometheus.HttpMetrics;
using TestAPI.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGenService<User>,GenService<User>>();
builder.Services.AddSingleton<IMetricsService, MetricsService>();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting(); // This is where you add the UseRouting middleware
app.UseHttpMetrics(options =>
{
    // This will preserve only the first digit of the status code.
    // For example: 200, 201, 203 -> 2xx
    options.ReduceStatusCodeCardinality();
});

var usersGauge = Metrics.CreateGauge("app_users_total", "Number of registered users in the application");

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapMetrics(); // This should be inside UseEndpoints
});

RegisterMetricsForEndpoints(app);

app.Run();


void RegisterMetricsForEndpoints(WebApplication app)
{
    var endpointDataSource = app.Services.GetRequiredService<EndpointDataSource>();

    var counter = Metrics.CreateCounter("http_requests_received_total", "Total HTTP requests received",
        new CounterConfiguration
        {
            LabelNames = new[] { "code","method","controller", "action", "endpoint" }
        });

    foreach (var endpoint in endpointDataSource.Endpoints.OfType<RouteEndpoint>())
    {
        var metadata = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();

        if (metadata != null)
        {
            var controller = metadata.ControllerName;
            var action = metadata.ActionName;

            var httpMethodMetadata = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>();
            if (httpMethodMetadata != null)
            {
                foreach (var httpMethod in httpMethodMetadata.HttpMethods)
                {
                    for (int i = 1; i <= 5; i++)
                    {
                        counter.WithLabels($"{i}xx", httpMethod, controller, action, endpoint.RoutePattern.RawText!).Inc(0);
                    }
                }
            }
        }
    }
}