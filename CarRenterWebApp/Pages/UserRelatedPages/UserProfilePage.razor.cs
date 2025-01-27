using CarRenterWebApp.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace CarRenterWebApp.Pages.UserRelatedPages
{
    public partial class UserProfilePage
    {
        User? user = new User();
        string userProfilePath = "";
        List<Rental> rentals = new();
        List<Rental> filterrentals = new();
        Dictionary<int, byte[]?> CarPhotos = new();
        TimeSpan LicenseTime;
        int LicenseYears;
        private bool isLoading = true;
        protected override async Task OnAfterRenderAsync(bool firstRender)// TODO: jak bedzi apicall do tego to dane uzytkownika wziazc z api 
        {
            if (!await CheckUser()) return;
            user = await APICall.GetUserData(user.Email);
            rentals = await GetRentals(user.IsEmployee);
            await GetPhotos();

            LicenseTime = (DateTime.Now - user.LicenseDate);
            LicenseYears = LicenseTime.Days / 365;



            if (firstRender)
                StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            if (!await CheckUser()) return;
            user = await APICall.GetUserData(user.Email);
            rentals = await GetRentals(user.IsEmployee);
            await GetPhotos();

            LicenseTime = (DateTime.Now - user.LicenseDate);
            LicenseYears = LicenseTime.Days / 365;
            isLoading = false;

        }
        private async Task<bool> CheckUser()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var logUser = authState.User;
            if (logUser.Identity.IsAuthenticated)
                await UserAuth.SetRole(logUser);
            var role = await UserAuth.CheckRole(logUser, Navigation);
            var email = logUser.FindFirst("email")?.Value;
            if (email == null)
            {
                UserAuth.LogoutUser(Navigation);
                return false;
            }
            if (role == null) return false;
            user = await UserAuth.FillUserInfo(user, logUser);
            return true;
        }
        private async Task GetPhotos()
        {
            foreach (var rental in rentals)
            {
                var photo = await APICall.GetCarImage(rental.CarId);
                if (CarPhotos.ContainsKey(rental.Id))
                    CarPhotos[rental.Id] = photo;
                else
                    CarPhotos.Add(rental.Id, photo);
            }
        }
        private byte[] GetCarPhoto(int rentId)
        {
            if (CarPhotos.ContainsKey(rentId))
            {
                return CarPhotos[rentId];
            }
            return null;
        }
        private async Task<List<Rental>> GetRentals(bool role)
        {
            List<Rental> rentals = new List<Rental>();

            if (role) rentals = await APICall.GetActiveRentals();
            else rentals = await APICall.GetUsersRentalsMid(user.Email);

            if (rentals == null) rentals = new List<Rental>();

            rentals.OrderByDescending(o => o.RentalEndDate).ToList();

            filterrentals = rentals.ToList();

            return rentals;
        }




        public void BeginLogOut()
        {
            Navigation.NavigateToLogout("authentication/logout");
        }
        public async void OnClickReturnCarHandler(Rental rental)
        {
            await localStorage.SetItemAsync<List<Rental>>("rentals", rentals);
            await localStorage.SetItemAsync<Rental>("rental", rental);
            Thread.Sleep(10);
            NavManager.NavigateTo("/ReturnPage");
        }
        public async void OnClickHistoryHandler(Rental rental)
        {
            await localStorage.SetItemAsync<List<Rental>>("rentals", rentals);
            await localStorage.SetItemAsync<Rental>("rental", rental);
            Thread.Sleep(10);
            NavManager.NavigateTo("/RentalHistory");
        }
        public async void OnClickPendingCarHandler(Rental rental)
        {
            await localStorage.SetItemAsync<List<Rental>>("rentals", rentals);
            await localStorage.SetItemAsync<Rental>("rental", rental);
            Thread.Sleep(10);
            NavManager.NavigateTo("/RentalHistory");
        }
        public void onFilterChange(ChangeEventArgs e)
        {
            switch (e.Value)
            {
                case "":
                    filterrentals = rentals.ToList();
                    break;
                case "toapprove":
                    filterrentals = rentals.Where(item => item.Status == Rental.RentalStatus.ReadyToReturn).ToList();
                    break;
                case "returned":
                    filterrentals = rentals.Where(item => item.Status == Rental.RentalStatus.Completed).ToList();
                    break;
                case "active":
                    filterrentals = rentals.Where(item => item.Status == Rental.RentalStatus.Active).ToList();
                    break;
                default:
                    break;
            }
        }
        public async void OnClickGetCarBackHandler(Rental rental)
        {
            await localStorage.SetItemAsync<List<Rental>>("rentals", rentals);
            await localStorage.SetItemAsync<Rental>("rental", rental);
            Thread.Sleep(10);
            NavManager.NavigateTo("/RecieveCar");
        }
        public async void OnClickActiveCarHandler(Rental rental)
        {
            await localStorage.SetItemAsync<List<Rental>>("rentals", rentals);
            await localStorage.SetItemAsync<Rental>("rental", rental);
            Thread.Sleep(10);
            NavManager.NavigateTo("/ActiveCar");
        }
    }
}
