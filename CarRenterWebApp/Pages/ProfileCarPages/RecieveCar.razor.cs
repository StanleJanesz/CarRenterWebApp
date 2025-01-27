namespace CarRenterWebApp.Pages.ProfileCarPages
{
    using CarRenterWebApp.Classes;
    using Microsoft.AspNetCore.Components.Forms;

    public partial class RecieveCar
    {
        Rental rental;
        Byte[]? carPhoto = null;
        String notes = "notes";
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            rental = await localStorage.GetItemAsync<Rental>("rental");

            if (firstRender)
            {
                StateHasChanged();
            }
        }

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            var file = e.File;
            using (var ms = new MemoryStream())
            {
                await file.OpenReadStream().CopyToAsync(ms);
                carPhoto = ms.ToArray();
            }
        }


        private async void OnClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            User user = new();
            user = await UserAuth.FillUserInfo(user, authState.User);
            await APICall.ConfirmReturn(rental.Id, GlobalConstants.register, notes, carPhoto); ;
            Navigation.NavigateTo("UserProfilePage");
        }
    }
}

