using CarRenterWebApp.Classes;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;

namespace CarRenterWebApp.Pages
{


    public partial class OffertDetails
    {
        Offer? offer = null;
        byte[]? carPhoto = null;
        bool warn = false;
        User user = new();
        private bool isLoading = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            offer = await localStorage.GetItemAsync<Offer>("offer");
            var isUser = await GetUser();
            if (!isUser || offer == null)
            {
                NavManager.NavigateTo("/");
                return;
            }
            carPhoto = await APICall.GetCarImage(offer.CarId);
            if (firstRender)
                StateHasChanged();
            warn = await localStorage.GetItemAsync<bool>("warn");
        }
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            offer = await localStorage.GetItemAsync<Offer>("offer");
            var isUser = await GetUser();
            if (!isUser || offer == null)
            {
                NavManager.NavigateTo("/");
                return;
            }
            carPhoto = await APICall.GetCarImage(offer.CarId);
            warn = await localStorage.GetItemAsync<bool>("warn");
            isLoading = false;
        }
        private async Task onconfirm(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {

            if (warn)
            {
                await localStorage.SetItemAsync<bool>("reset", true);
            }
            await APICall.RentMid(new RentalSelectionRequest()
            {
                OfferId = offer.Id,
                UserEmail = user.Email,
                ProviderId = offer.ProviderId
            });
            NavManager.NavigateTo("/");
        }
        private async Task<bool> GetUser()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var logUser = authState.User;
            if (logUser.Identity.IsAuthenticated)
            {
                user = await UserAuth.FillUserInfo(user, logUser);
                if (user != null)
                {
                    return true;
                }
            }
            return false;
        }
        private void comeback(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {
            NavManager.NavigateTo("/OffertView");
        }


    }
}
