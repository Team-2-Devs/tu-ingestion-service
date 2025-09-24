var builder = WebApplication.CreateBuilder(args);

//
// Services
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();


//
// Pipeline
//
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapControllers();
app.MapGet("/", () => Results.Ok(new { service = "Ingestion.Api", status = "ok" }));

app.Run();

public partial class Program { }