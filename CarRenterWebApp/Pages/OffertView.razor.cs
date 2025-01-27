using Microsoft.AspNetCore.Components.Web;

namespace CarRenterWebApp.Pages
{
    using CarRenterWebApp.Classes;
    using Microsoft.AspNetCore.Components;
    using static CarRenterWebApp.Classes.RentalRequest;

    public partial class OffertView
    {
        List<Offer> offers = new List<Offer>();
        private int currentPage = 0;
        private int maxPages = 0;
        private bool loggedin = false;
        private List<byte[]> currentCarsPhotos = new();
        private static int pageAmount = GlobalConstants.pageAmount;
        private bool isLoading = true;
        string email = "";
        bool warn = false;
        User user = new();
        InsuranceTypes insurance = InsuranceTypes.Standard;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            isLoading = true;
            if (firstRender)
            {

                bool success = await GetSelectedCar();
                if (!success) return;

                await GetEmail();

                success = await GetOffers();
                if (!success) return;

                maxPages = CalculatePages();

                currentPage = 0;

                await GetPhotos();

                StateHasChanged();

            }
            isLoading = false;
        }
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            bool success = await GetSelectedCar();
            if (!success) return;

            await GetEmail();

            success = await GetOffers();
            if (!success) return;

            maxPages = CalculatePages();

            currentPage = 0;

            await GetPhotos();

            isLoading = false;
        }
        int CalculatePages() => offers.Count / pageAmount + Math.Min(offers.Count % pageAmount, 1);
        int PageLimit() => Math.Min(currentPage * pageAmount + pageAmount, offers.Count);
        private async Task<bool> GetOffers()
        {
            DateTime start = await localStorage.GetItemAsync<DateTime>("start");
            DateTime end = await localStorage.GetItemAsync<DateTime>("end");
            offers = await Offer.GetOffersFor(CarService.SelectedCar.Model, CarService.SelectedCar.Brand, CarService.CarsDictionary, email, insurance, start, end);
            if (offers == null)
            {
                NavManager.NavigateTo("/");
                return false;
            }
            return true;
        }
        private async Task GetPhotos()
        {
            foreach (var offer in offers)
            {
                var photo = await APICall.GetCarImage(offer.CarId);
                currentCarsPhotos.Add(photo);
            }
        }
        private async Task GetEmail()
        {
            email = "";
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var logUser = authState.User;
            if (logUser.Identity.IsAuthenticated)
            {
                user = await UserAuth.FillUserInfo(user, logUser);
                email = user.Email;
            }
        }
        private async Task<bool> GetSelectedCar()
        {
            CarService.SelectedCar = await LocalStorage.GetItemAsync<Car>("selectedCar");
            if (CarService.SelectedCar == null)
            {
                NavManager.NavigateTo("/");
                return false;
            }
            return true;
        }
        private async void OnClickHandler(int i)
        {
            Offer offer = offers[i];

            await LocalStorage.SetItemAsync<Offer>("offer", offer);

            NavManager.NavigateTo("/OffertDetails");
        }

        private async void Change(ChangeEventArgs e)
        {
            switch (e.Value)
            {
                case "Standard":
                    insurance = InsuranceTypes.Standard;
                    break;
                case "Premium":
                    insurance = InsuranceTypes.Premium;
                    break;
                case "Basic":
                    insurance = InsuranceTypes.Basic;
                    break;
            }
            var success = await GetOffers();
            if (!success) return;

            maxPages = CalculatePages();
            currentPage = 0;
            await GetPhotos();

            StateHasChanged();
        }
        private void OnPageChange(int i)
        {
            currentPage = i;
        }
        private void SortChange(IEnumerable<object> o)
        {
            offers = o.Cast<Offer>().ToList();
        }
    }
}
