using Blazored.LocalStorage;
using CarRenter.Client.Classes;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddSingleton<CarService>();
builder.Services.AddBlazoredLocalStorage();
await builder.Build().RunAsync();
