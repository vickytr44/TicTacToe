using TicTacToe.Api.Middleware;
using TicTacToe.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

app.Run();

// Make Program public for WebApplicationFactory in integration tests
public partial class Program { }
