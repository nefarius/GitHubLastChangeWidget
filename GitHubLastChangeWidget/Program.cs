using FastEndpoints;
using FastEndpoints.Swagger;

using GitHubLastChangeWidget.Services;

using Nefarius.Utilities.AspNetCore;

var builder = WebApplication.CreateBuilder(args).Setup();

builder.Services.AddFastEndpoints().SwaggerDocument(o => o.RemoveEmptyRequestSchema = true);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<GitHubApiService>();

var app = builder.Build().Setup();

app.UseFastEndpoints().UseSwaggerGen();

app.Run();
