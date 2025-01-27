using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;
using System.Text.Json;
using CarRenterWebApp.Classes;
using System;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Logging;

namespace CarRenterWebApp.Pages;
public partial class OffertsList
{
    private List<string> models = new();
    private List<string> brands = new();
    private string selectedBrand;
    private string? selectedModel;
    private int currentPage;
    private int maxPages;
    private bool isLoading = true;
    private List<Car> currentCars = new();
    private List<byte[]> currentCarsPhotos = new();
    private List<String> localizations = new();
    string selectedLocalization = "";
    private String imagePath;
    DateTime startTime = DateTime.Now;
    DateTime endTime = DateTime.Now;
    string start = DateTime.Now.ToString("yyyy-MM-dd");
    string end = DateTime.Now.ToString("yyyy-MM-dd");
    private static int pageAmount = GlobalConstants.pageAmount;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        isLoading = true;
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var logUser = authState.User;
        var rolew = logUser.FindFirst(ClaimTypes.Role);

        if (logUser.Identity.IsAuthenticated && rolew == null)
        {
            Console.WriteLine("Setting role");

            await UserAuth.FillUserInfo(new User(), authState.User);
            await InvokeAsync(StateHasChanged);
        }
        if (await CheckReload())
            DoReload();
        isLoading = false;
    }
    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        var logUser = authState.User;
        await UserAuth.SetRole(logUser);
        await UserAuth.isRegistred(AuthenticationStateProvider.GetAuthenticationStateAsync().Result.User, NavManager);

        await CheckReload();

        DoReload();
        isLoading = false;

    }
    void AddBrands(List<Car> cars)
    {
        brands = CarService.CarsDictionary.Keys.ToList();
    }
    async void AddPhotos(List<Car> cars)
    {
        currentCarsPhotos.Clear();
        foreach (var car in cars)
        {
            var photo = await APICall.GetCarImage(car.Id);
            currentCarsPhotos.Add(photo);
        }
    }
    void AddLocalizations(List<Car> cars)
    {
        localizations.Clear();
        localizations = currentCars.Select(o => o.Location).ToList();

    }
    int CalculatePages() => currentCars.Count / pageAmount + Math.Min(currentCars.Count % pageAmount, 1);
    int PageLimit() => Math.Min((currentPage + 1) * pageAmount, @currentCars.Count);
    private async Task<bool> CheckReload()
    {
        var storedDictionary = await localStorage.GetItemAsync<Dictionary<string, Dictionary<string, List<Car>>>>("carsDictionary");
        var lastUpdatedTimestamp = await localStorage.GetItemAsync<DateTime?>("carsDictionaryLastUpdated");
        bool reset = await localStorage.GetItemAsync<bool>("reset");

        bool shouldReload = lastUpdatedTimestamp == null || DateTime.UtcNow - lastUpdatedTimestamp > TimeSpan.FromMinutes(30) || reset;
        if (storedDictionary == null || shouldReload)
        {
            CarService.CarsDictionary = await APICall.GetCarsMid();
            await localStorage.SetItemAsync("carsDictionary", CarService.CarsDictionary);
            await localStorage.SetItemAsync("carsDictionaryLastUpdated", DateTime.UtcNow);
        }
        else
        {

            CarService.CarsDictionary = storedDictionary;
        }
        return shouldReload;

    }

    public void DoReload()
    {
        currentCars = CarService.CarsDictionary.Values
            .SelectMany(dict => dict.Values)
            .SelectMany(list => list)
            .ToList();

        AddLocalizations(currentCars);

        AddPhotos(currentCars);

        AddBrands(currentCars);

        maxPages = CalculatePages();

        currentPage = 0;
    }
    private async void NewCarList()
    {

        if (string.IsNullOrEmpty(selectedBrand))
        {
            models.Clear();
            currentCars = CarService.CarsDictionary.Values.SelectMany(dict => dict.Values).SelectMany(list => list).ToList();
        }
        else if (string.IsNullOrEmpty(selectedModel))
        {
            models = CarService.CarsDictionary[selectedBrand].Keys.ToList();
            currentCars = CarService.CarsDictionary[selectedBrand].Values.SelectMany(list => list).ToList();
        }
        else
        {
            currentCars = CarService.CarsDictionary[selectedBrand][selectedModel].ToList();
        }
        if (!string.IsNullOrEmpty(selectedLocalization))
        {
            currentCars.RemoveAll(car => car.Location != selectedLocalization);
        }
        currentPage = 0;


        maxPages = CalculatePages();

        currentCarsPhotos.Clear();

        foreach (var car in currentCars)
        {
            var photo = await APICall.GetCarImage(car.Id);
            currentCarsPhotos.Add(photo);
        }
        await localStorage.SetItemAsync<List<byte[]>>("photos", currentCarsPhotos);

    }

    private async Task OnModelChange(ChangeEventArgs e)
    {
        selectedModel = e.Value?.ToString();


        NewCarList();

    }
    private void OnLocalizationChange(ChangeEventArgs e)
    {
        selectedLocalization = e.Value?.ToString() ?? "";
        NewCarList();

    }
    private async Task OnBrandChange(ChangeEventArgs e)
    {
        selectedBrand = e.Value?.ToString();
        selectedModel = null;
        NewCarList();
    }
    private void OnStartDateChange(ChangeEventArgs e)
    {

        startTime = DateTime.ParseExact(e.Value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);

        if (endTime < startTime)
        {
            end = startTime.ToString("yyyy-MM-dd");
            endTime = startTime;
        }
        start = startTime.ToString("yyyy-MM-dd");
    }
    private void OnEndDateChange(ChangeEventArgs e)
    {
        endTime = DateTime.ParseExact(e.Value.ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        if (endTime < DateTime.Now)
        {
            endTime = DateTime.Now;
        }
        if (endTime > DateTime.Now.AddMonths(3))
        {
            endTime = DateTime.Now.AddMonths(3);
        }
        end = endTime.ToString("yyyy-MM-dd");
    }
    private void OnPageChange(int i)
    {
        currentPage = i;
    }


    private async void OnClickHandler(int i)
    {
        CarService.SetSelectedCar(currentCars[i]);
        await localStorage.SetItemAsync("selectedCar", CarService.SelectedCar);
        await localStorage.SetItemAsync("start", startTime);
        await localStorage.SetItemAsync("end", endTime);
        NavManager.NavigateTo("/OffertView");
    }
    private void ChangeSortBox(IEnumerable<object> objects)
    {
        currentCars = objects.Cast<Car>().ToList();

    }
    private bool IsDateValid()
    {
        var now = DateTime.Now.Date;
        var threeMonthsFromNow = now.AddMonths(3);
        return startTime >= now && endTime <= threeMonthsFromNow && endTime >= startTime;
    }


}
