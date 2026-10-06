using Sample.WebApi.Endpoints;
using D20Tek.LowDb;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();

builder.Services.AddLowDbAsync<TasksDocument>(b =>
    b.UseFileDatabase("tasks.json")
     .WithFolder("data")
     .WithLifetime(ServiceLifetime.Scoped));
builder.Services.AddScoped<ITasksRepository, TasksRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapTaskEntityEndpoints();
app.MapTaskV2Endpoints();

app.Run();
