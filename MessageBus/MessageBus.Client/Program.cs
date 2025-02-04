using MessageBusSample.Client.Services;
using MessageBusSample.Client.ViewModels;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddSingleton<CounterViewModel>();
builder.Services.AddSingleton<EntityService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();
