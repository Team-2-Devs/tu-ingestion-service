using Ingestion.Api.DependencyInjection;
using Ingestion.Api.Hosting;
using Ingestion.Application.Abstractions;
using Ingestion.Application.DependencyInjection;
using Ingestion.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

// Compose layers
builder.Services
  .AddApi()
  .AddApplication()
  .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Run database initialization once at startup
using (var scope = app.Services.CreateScope())
{
  var initializer = scope.ServiceProvider
      .GetRequiredService<IDatabaseInitializer>();

  await initializer.InitializeAsync();
}

// Pipeline
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<CorrelationMiddleware>();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { service = "Ingestion.Api", status = "ok" }));

app.Run();

public partial class Program { }
