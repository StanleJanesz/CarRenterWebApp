using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace CarRenterWebApp.Classes
{
	internal class UserAuth
	{
        
		public static async Task<User> FillUserInfo(User user, ClaimsPrincipal claimsPrincipal) 
		{
            user.Email = claimsPrincipal.FindFirst("email")?.Value ?? "UNKNOWN";
            try
            {
                user = await APICall.GetUserData(user.Email);
            }
            catch (Exception)
            { 
                user = new User();
                user.Email = claimsPrincipal.FindFirst("email")?.Value ?? "UNKNOWN";
                return user;
            }
            return user;
        }
		public static async Task SetRole(ClaimsPrincipal claimsPrincipal)
		{
            string Email = claimsPrincipal.FindFirst("email")?.Value ?? "UNKNOWN";
            string role = await APICall.IsEmployee(Email);
            claimsPrincipal.AddIdentity(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Role, role) }));
        }
		public static async Task<string?> CheckRole(ClaimsPrincipal claimsPrincipal, NavigationManager navigationManager)
        {
            var role = claimsPrincipal.FindFirst(ClaimTypes.Role);
            if (role == null)
            {
                LogoutUser( navigationManager);
                return null;
            }
            return role.Value;
        }
        public static void LogoutUser(NavigationManager navigationManager)
        {
            navigationManager.NavigateToLogout("authentication/logout");
        }
        public static async Task<bool> isRegistred(ClaimsPrincipal claimsPrincipal, NavigationManager navigationManager)
        {
            if (claimsPrincipal.Identity?.IsAuthenticated == true)
            {
                User user;
                try
                {
                    user = await FillUserInfo(new User(), claimsPrincipal);
                }
                catch (Exception)
                {
                    navigationManager.NavigateTo("/RegistrationPage");
                    return false;
                }
                if (user == null || user.Id == 0)
                {
                    navigationManager.NavigateTo("/RegistrationPage");
                    return false;
                }
                return true;
            }
           
            return true;
        }
		public static readonly string workerRole = "worker";
		public static readonly string readerRole = "reader";
	}
    
}
