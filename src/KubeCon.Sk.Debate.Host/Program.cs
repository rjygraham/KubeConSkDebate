using KubeCon.Sk.Debate.Host.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Aspire service defaults include service discovery, resilience, health checks, and OpenTelemetry.
builder.AddServiceDefaults();

// Add the application components including Azure services, Orleans, and OpenAPI.
builder.AddApplicationComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();
app.MapDebateControllerEndpoints();
app.MapOpenApiDuringDevelopment();

// app.UseHttpsRedirection();

app.Run();
