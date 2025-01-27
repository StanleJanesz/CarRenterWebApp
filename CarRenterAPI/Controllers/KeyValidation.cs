using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CarProviderAPI.Controllers
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
			return (apiKey == Environment.GetEnvironmentVariable("User_Rental_API_Key") || apiKey == Environment.GetEnvironmentVariable("Employee_Rental_API_Key"));
		}
	}

	public interface IUserKeyValidator
	{
		bool IsValid(string apiKey);
	}

	public class EmployeeKeyAttribute : ServiceFilterAttribute
	{
		public EmployeeKeyAttribute()
			: base(typeof(EmployeeKeyAuthorizationFilter))
		{
		}
	}

	public class EmployeeKeyAuthorizationFilter : IAuthorizationFilter
	{
		private const string ApiKeyHeaderName = "Rental-API-Key";

		private readonly IEmployeeKeyValidator _apiKeyValidator;

		public EmployeeKeyAuthorizationFilter(IEmployeeKeyValidator apiKeyValidator)
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

	public class EmployeeKeyValidator : IEmployeeKeyValidator
	{
		public bool IsValid(string apiKey)
		{
			return apiKey == Environment.GetEnvironmentVariable("Employee_Rental_API_Key");
		}
	}

	public interface IEmployeeKeyValidator
	{
		bool IsValid(string apiKey);
	}


}
