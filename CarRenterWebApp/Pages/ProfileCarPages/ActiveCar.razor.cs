using CarRenterWebApp.Classes;

namespace CarRenterWebApp.Pages.ProfileCarPages
{
    public partial class ActiveCar
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
    }
}
