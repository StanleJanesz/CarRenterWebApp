using Microsoft.EntityFrameworkCore;
using CarProviderAPI.model.Cars;
using System.Configuration;
using CarProviderAPI.Controllers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "AllowBlazorClient", policy =>
	{
		policy.SetIsOriginAllowed(_ => true)
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials(); 
	});
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddDbContext<CarContext>(opt =>
	opt.UseSqlServer());
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
	{
		Description = "API Key required. Add it to the request headers using the field below.",
		Type = SecuritySchemeType.ApiKey,
		Name = "Rental-API-Key", // The header name where the key is sent
		In = ParameterLocation.Header,
		Scheme = "ApiKeyScheme"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "ApiKey"
				},
				Scheme = "ApiKeyScheme",
				Name = "ApiKey",
				In = ParameterLocation.Header
			},
			new List<string>()
		}
	});
});

builder.Services.AddSingleton<UserKeyAuthorizationFilter>();
builder.Services.AddSingleton<IUserKeyValidator, UserKeyValidator>();

builder.Services.AddSingleton<EmployeeKeyAuthorizationFilter>();
builder.Services.AddSingleton<IEmployeeKeyValidator, EmployeeKeyValidator>();

var app = builder.Build();

app.UseCors("AllowBlazorClient");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
