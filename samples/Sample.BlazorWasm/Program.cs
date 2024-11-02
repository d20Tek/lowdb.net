//---------------------------------------------------------------------------------------------------------------------
// Copyright (c) d20Tek.  All rights reserved.
//---------------------------------------------------------------------------------------------------------------------
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Sample.BlazorWasm;
using Sample.BlazorWasm.Pages;
using D20Tek.LowDb.Browser;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// swap between different LowDbs (local, session) by picking the one to register.
builder.Services.AddLocalLowDb<TasksDocument>("d20tek-tasks");
//builder.Services.AddSessionLowDb<TasksDocument>("d20tek-tasks-session");

builder.Services.AddScoped<TaskRepository>();

await builder.Build().RunAsync();
