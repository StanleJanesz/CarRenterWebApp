using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace CarRenterAPI.Controllers
{
	public class UserKeyAttribute : ServiceFilterAttribute
	{
		public UserKeyAttribute()
			: base(typeof(UserKeyAuthorizationFilter))
		{
		}
	}

	public class UserKeyAuthorizationFilter : IAuthorizationFilter
	{
		private const string ApiKeyHeaderName = "Rental-API-Key";

		private readonly IUserKeyValidator _apiKeyValidator;

		public UserKeyAuthorizationFilter(IUserKeyValidator apiKeyValidator)
		{
			_apiKeyValidator = apiKeyValidator;
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
			string apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

			if (!_apiKeyValidator.IsValid(apiKey))
			{
				context.Result = new UnauthorizedResult();
			}
		}
	}

	public class UserKeyValidator : IUserKeyValidator
	{
		public bool IsValid(string apiKey)
		{
			return (apiKey == Environment.GetEnvironmentVariable("Rental_API_Key") || apiKey == Environment.GetEnvironmentVariable("Rental_API_Key_2"));
		}
	}

	public interface IUserKeyValidator
	{
		bool IsValid(string apiKey);
	}
}
