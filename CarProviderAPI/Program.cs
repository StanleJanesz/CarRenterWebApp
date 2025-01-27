using CarRenterAPI.Controllers;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowBlazorClient", policy =>
	{
		policy.SetIsOriginAllowed(_ => true)
			  .AllowAnyHeader()
			  .AllowAnyMethod()
			  .AllowCredentials();
	});
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<UserKeyAuthorizationFilter>();
builder.Services.AddSingleton<IUserKeyValidator, UserKeyValidator>();

builder.Services.AddSwaggerGen(c =>
{
	c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
	{
		Description = "API Key required. Add it to the request headers using the field below.",
		Type = SecuritySchemeType.ApiKey,
		Name = "Rental-API-Key",
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

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors("AllowBlazorClient");
app.MapControllers();

app.Run();
