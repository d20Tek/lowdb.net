﻿//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Sample.WebApi.Endpoints;
using D20Tek.LowDb;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLowDbAsync<TasksDocument>(b =>
    b.UseFileDatabase("tasks.json")
     .WithFolder("data")
     .WithLifetime(ServiceLifetime.Scoped));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTaskEntityEndpoints();

app.Run();
