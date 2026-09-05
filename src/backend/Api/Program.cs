using Microsoft.EntityFrameworkCore;
using TicTacToe.Api.Endpoints;
using TicTacToe.Api.Middleware;
using TicTacToe.Application;
using TicTacToe.Infrastructure;
using TicTacToe.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddApplication();
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

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TicTacToeDbContext>();
    db.Database.EnsureCreated();
}

// Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));
app.MapGameEndpoints();

app.Run();

// Make Program public for WebApplicationFactory in integration tests
public partial class Program { }
