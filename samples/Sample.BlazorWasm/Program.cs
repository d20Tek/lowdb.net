//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using D20Tek.LowDb;
using Sample.BlazorWasm;
using Sample.BlazorWasm.Pages;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLowDb<TasksDocument>(b =>
    b.UseFileDatabase("tasks.json")
     .WithFolder(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\d20tek-tasks")
     .WithLifetime(ServiceLifetime.Scoped));

await builder.Build().RunAsync();
