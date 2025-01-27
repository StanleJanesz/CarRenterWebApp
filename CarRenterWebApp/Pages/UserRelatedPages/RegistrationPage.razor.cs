using CarRenterWebApp.Classes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.IO;

namespace CarRenterWebApp.Pages.UserRelatedPages
{
    public partial class RegistrationPage
    {
        [SupplyParameterFromForm]
        private User? user { get; set; }
        static User DefoultUser = new User();
        string country;
        string city;
        string street;
        string houseNumber;
        private EditContext editContext = new EditContext(DefoultUser);
        private ValidationMessageStore validationMessageStore;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                user = new User();
                await GetUser();
                editContext = new EditContext(user);
                validationMessageStore = new ValidationMessageStore(editContext);
                editContext.OnFieldChanged += HandleFieldChanged;
                StateHasChanged();
            }
        }
        private void HandleFieldChanged(object? sender, FieldChangedEventArgs e)
        {
            validationMessageStore.Clear();
            ValidateForm();
        }

        private void ValidateForm()
        {
            var age = DateTime.Today.Year - user.BirthDate.Year;
            if (user.BirthDate > DateTime.Today.AddYears(-age)) age--;

            if (age < 18 || age > 120)
            {
                validationMessageStore.Add(() => user.BirthDate, "Age must be between 18 and 120 years.");
            }

            if (user.LicenseDate <= user.BirthDate)
            {
                validationMessageStore.Add(() => user.LicenseDate, "License date must be after birth date.");
            }

            editContext.NotifyValidationStateChanged();
        }
        private async Task GetUser()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var logUser = authState.User;
            user = await UserAuth.FillUserInfo(user, logUser);
        }
        private async Task SaveUser(User user) // tu będzie api call do zapisania usera
        {
            await APICall.AddUser(user);
        }

        private async Task Submit()
        {
            ValidateForm();
            if (editContext.Validate())
            {
                user.Location = $"{country}, {city}, {street}, {houseNumber}";
                await SaveUser(user);
                Thread.Sleep(20);
                NavManager.NavigateTo("/UserProfilePage");
            }
        }
    }

}