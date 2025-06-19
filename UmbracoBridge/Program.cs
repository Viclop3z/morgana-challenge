using Microsoft.Extensions.Configuration;
using UmbracoBridge.API.Extensions;
using UmbracoBridge.API.MiddleWare;
using UmbracoBridge.Application.Extensions;
using UmbracoBridge.Infrastructure.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
 
builder.Services.AddControllers();


builder.Services.AddOpenApi();
builder.Services.AddLogging();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.Configure<ApiServiceSettings>(
    builder.Configuration.GetSection("Services")
);

builder.Services.Configure<ApiServiceSettings>(builder.Configuration.GetSection("ApiServiceSettings"));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
