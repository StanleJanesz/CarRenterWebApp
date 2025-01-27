using CarRenterWebApp;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using CarRenterWebApp.Classes;
using System;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
var configuration = builder.Configuration;
APICall.PrimaryAPIUrl = configuration["PrimaryAPIUrl"];
APICall.MidddleAPIUrl = configuration["MidddleAPIUrl"];
APICall.UserKey = configuration["UserKey"];
APICall.EmployeeKey = configuration["EmployeeKey"];
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<CarService>();
builder.Services.AddOidcAuthentication(options =>
{
	options.ProviderOptions.Authority = "https://accounts.google.com/";
	options.ProviderOptions.ClientId = configuration["ClientID"];
	options.ProviderOptions.ResponseType = "id_token";
	options.ProviderOptions.DefaultScopes.Add("openid");
	options.ProviderOptions.DefaultScopes.Add("profile");
	options.ProviderOptions.DefaultScopes.Add("email");
});
builder.Services.AddAuthorizationCore();


// Add CORS policy		

var app = builder.Build();
await app.RunAsync();
