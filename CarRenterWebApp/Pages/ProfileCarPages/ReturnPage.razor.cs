namespace CarRenterWebApp.Pages.ProfileCarPages

{
    using CarRenterWebApp.Classes;

    public partial class ReturnPage
    {
        Rental rental;
        byte[]? carPhoto;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            rental = await localStorage.GetItemAsync<Rental>("rental");
            if (rental == null)
            {
                Navigation.NavigateTo("/UserProfilePage");
                return;
            }
            if (firstRender)
            {
                StateHasChanged();
            }
        }
        private async Task GetPhoto()
        {
            carPhoto = await APICall.GetCarImage(rental.CarId);
        }
        private async void OnClick(Microsoft.AspNetCore.Components.Web.MouseEventArgs e)
        {
            await APICall.StartReturnMid(new CarReturnRequest() { RentalId = rental.Id, UserEmail = rental.UserEmail, ProviderId = rental.ProviderId });
            Navigation.NavigateTo("UserProfilePage");
        }
    }
}

