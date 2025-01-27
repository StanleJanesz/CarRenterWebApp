namespace CarRenterWebApp.Pages.ProfileCarPages
{
    using CarRenterWebApp.Classes;
    public partial class RentalHistory
    {
        Rental rental;
        byte[]? carPhoto = null;

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

    }
}
