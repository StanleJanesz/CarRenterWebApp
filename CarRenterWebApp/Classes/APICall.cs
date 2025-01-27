using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Threading;
using static System.Net.WebRequestMethods;

namespace CarRenterWebApp.Classes
{
	public static class APICall
	{
		public static string PrimaryAPIUrl;
		public static string MidddleAPIUrl;

		private static readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true
		};
		public static string UserKey;
		public static string EmployeeKey;

		public static async Task<bool> StartReturnMid(CarReturnRequest request)
		{
			var apiUrl = MidddleAPIUrl + "start-return"; // Zmień na swój URL

            // Obiekt żądania

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //   var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

		public static async Task<List<Rental>> GetUsersRentalsMid(string email)
		{
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);
			string url = MidddleAPIUrl + "user-rentals?userEmail=" + email;
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);

            var response = await TryAPICall(client.GetStringAsync(url, cancellationTokenSource.Token));
            // var response = await client.GetStringAsync(url);

            List<Rental> rentalslist = JsonSerializer.Deserialize<List<Rental>>(response, jsonOptions);

            return rentalslist;
        }

		public static async Task<RentalConfirmation> RentMid(RentalSelectionRequest request)
		{
			var apiUrl = MidddleAPIUrl + "select-offer"; // Zmień na swój URL
											   // Obiekt żądania

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //  var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            RentalConfirmation confirmation = null;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                confirmation = JsonSerializer.Deserialize<RentalConfirmation>(jsonResponse, jsonOptions);
            }

            return confirmation;
        }

		public static async Task<Offer> GenerateOfferMid(RentalRequest request)
		{
			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

			var apiUrl = MidddleAPIUrl + "generate-offer";

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            HttpResponseMessage response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            Offer offer = null;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                offer = JsonSerializer.Deserialize<Offer>(jsonResponse, jsonOptions);
            }

            return offer;
        }

		public static async Task<Dictionary<string, Dictionary<string, List<Car>>>> GetCarsMid()
		{
			var cars = new Dictionary<string, Dictionary<string, List<Car>>>();
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);
			var apiUrl = MidddleAPIUrl + "cars";
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			string response = string.Empty;
			response = await TryAPICall(client.GetStringAsync(apiUrl, cancellationTokenSource.Token));
			if (response != string.Empty)
			{
				List<Car> carslist = JsonSerializer.Deserialize<List<Car>>(response, jsonOptions);
				foreach (var car in carslist)
				{
					if (car.Location == null)
					{
						car.Location = "no location";
					}
					cars = Car.AddCarToDic(cars, car);
				}
			}
			return cars;
		}

        public static async Task<string> IsEmployee(string email)
        {
            var apiUrl = Url + "is-employee";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Rental-API-Key", EmployeeKey);
            string requestUrl = $"{apiUrl}?email={email}";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(client.GetAsync(requestUrl, cancellationTokenSource.Token));
            // var response = await client.GetAsync(requestUrl);
            bool isEmployee = false;
            if (response.IsSuccessStatusCode)
            {
                // Read and print the response content
                string content = await response.Content.ReadAsStringAsync();
                isEmployee = JsonSerializer.Deserialize<bool>(content, jsonOptions);
            }
            if (isEmployee)
                return UserAuth.workerRole;
            else
                return UserAuth.readerRole;
        }

		public static async Task<bool> ConfirmReturn(int rentalId, int employeeId, string notes, byte[] img)
		{
			var apiUrl = PrimaryAPIUrl + "confirm-return"; // Zmień na swój URL

            // Obiekt żądania

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", EmployeeKey);

            using var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(rentalId.ToString()), "RentalId");

            // Add EmployeeId
            formData.Add(new StringContent(employeeId.ToString()), "EmployeeId");

            // Add Notes
            formData.Add(new StringContent(notes), "Notes");

            var fileContent = new ByteArrayContent(img);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
            formData.Add(fileContent, "Attachments", "image");

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, formData, cancellationTokenSource.Token));
            // var response = await httpClient.PostAsync(apiUrl, formData);
            string jsonResponse = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                return true;
            }

            return false;
        }

		public static async Task<Rental.RentalStatus> RentalStatus(int rentalId, int userId)
		{
			var apiUrl = PrimaryAPIUrl + "rental-status";
			HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);
			string requestUrl = $"{apiUrl}?rentalId={rentalId}&userId={userId}";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			var response = await TryAPICall(client.GetAsync(requestUrl, cancellationTokenSource.Token));
          //  var response = await client.GetAsync(requestUrl);
			Rental.RentalStatus status = Rental.RentalStatus.Active;
			if (response.IsSuccessStatusCode)
			{
				// Read and print the response content
				string content = await response.Content.ReadAsStringAsync();
				status = JsonSerializer.Deserialize<Rental.RentalStatus>(content, jsonOptions);
			}

            return status;
        }

		public static async Task<bool> StartReturn(CarReturnRequest request)
		{
			var apiUrl = PrimaryAPIUrl + "start-return"; // Zmień na swój URL

            // Obiekt żądania

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //   var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

        public static async Task<byte[]> GetCarImage(int carId)
        {
            // Create an instance of HttpClient
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

			// Construct the full URL
			string url = PrimaryAPIUrl + $"get-image/{carId}";

            // Make the asynchronous GET request
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            HttpResponseMessage response = await TryAPICall(client.GetAsync(url, cancellationTokenSource.Token));
            //HttpResponseMessage response = await client.GetAsync(url);
            byte[] imageBytes = null;
            // Ensure the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the image data as a byte array
                imageBytes = await response.Content.ReadAsByteArrayAsync();
            }
            return imageBytes;
        }

        public static async Task<bool> Email(string email)
        {
            // Create an instance of HttpClient
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

			// Construct the full URL for the API
			string url = PrimaryAPIUrl + "send-offer";

            // Serialize the email address to JSON (if required by the API)
            var content = JsonContent.Create(email);

            // Make the asynchronous POST request
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            HttpResponseMessage response = await TryAPICall(client.PostAsync(url, content, cancellationTokenSource.Token));
            // HttpResponseMessage response = await client.PostAsync(url, content);
            string result = string.Empty;
            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Read the response as a string
                result = await response.Content.ReadAsStringAsync();
                return true;
            }
            return false;
        }

		public static async Task<List<Rental>> GetUsersRentals(string email)
		{
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);
			string url = PrimaryAPIUrl + "user-rentals?userEmail=" + email;
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			
            var response = await TryAPICall(client.GetStringAsync(url, cancellationTokenSource.Token));
            // var response = await client.GetStringAsync(url);

            List<Rental> rentalslist = JsonSerializer.Deserialize<List<Rental>>(response, jsonOptions);

            return rentalslist;
        }

		public static async Task<List<Rental>> GetActiveRentals()
		{
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", EmployeeKey);
			string url = PrimaryAPIUrl + "active-rentals";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
            var response = await TryAPICall(client.GetStringAsync(url, cancellationTokenSource.Token));
            //var response = await client.GetStringAsync(url);

            List<Rental> rentalslist = JsonSerializer.Deserialize<List<Rental>>(response, jsonOptions);

            return rentalslist;
        }

		public static async Task<RentalConfirmation> Rent(RentalSelectionRequest request)
		{
			var apiUrl = PrimaryAPIUrl + "select-offer"; // Zmień na swój URL
            // Obiekt żądania

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //  var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            RentalConfirmation confirmation = null;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                confirmation = JsonSerializer.Deserialize<RentalConfirmation>(jsonResponse, jsonOptions);
            }

            return confirmation;
        }

        public static async Task<Offer> GenerateOffer(RentalRequest request)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(request);

			var apiUrl = PrimaryAPIUrl + "generate-offer";

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            HttpResponseMessage response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            Offer offer = null;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                offer = JsonSerializer.Deserialize<Offer>(jsonResponse, jsonOptions);
            }

            return offer;
        }

        private static async Task<string> TryAPICall(Task<string> task)
        {
            string response = string.Empty;
            try
            {
                response = await task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Timeout");
            }
            return response;
        }

        private static async Task<HttpResponseMessage> TryAPICall(Task<HttpResponseMessage> task)
        {
            HttpResponseMessage response = new();
            try
            {
                response = await task;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Timeout");
                response.StatusCode = System.Net.HttpStatusCode.RequestTimeout;
            }
            return response;
        }
        public static async Task<Dictionary<string, Dictionary<string, List<Car>>>> GetCars()
		{
			var cars = new Dictionary<string, Dictionary<string, List<Car>>>();
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);
			var apiUrl = PrimaryAPIUrl + "get-cars-db";
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			string response = string.Empty;
			response = await TryAPICall(client.GetStringAsync(apiUrl, cancellationTokenSource.Token));
			if (response != string.Empty)
			{
				List<Car> carslist = JsonSerializer.Deserialize<List<Car>>(response, jsonOptions);
				foreach (var car in carslist)
				{
					if (car.Location == null)
					{
						car.Location = "no location";
					}
					cars = Car.AddCarToDic(cars, car);
				}
			}
			return cars;
		}

        public static async Task<bool> AddUser(User newUser)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(newUser);

			var apiUrl = PrimaryAPIUrl + "user";

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token));
            //  var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                return true;
            }

            return false;
        }

        public static async Task<User> GetUserData(string email)
        {
            // Create an instance of HttpClient
            using HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

			// Construct the full URL
			string url = PrimaryAPIUrl + $"user?email={email}";

            // Make the asynchronous GET request
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            HttpResponseMessage response = await TryAPICall(client.GetAsync(url, cancellationTokenSource.Token));
            // HttpResponseMessage response = await client.GetAsync(url);
            string jsonResponse = string.Empty;

            // Ensure the request was successful
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
            }
            User user = JsonSerializer.Deserialize<User>(jsonResponse, jsonOptions);
            return user;
        }

        public static async Task<bool> EditUser(User newUser)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Rental-API-Key", UserKey);

            // Serializacja obiektu do JSON
            var content = JsonContent.Create(newUser);

			var apiUrl = PrimaryAPIUrl + "user";

            // Wysłanie żądania POST
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(GlobalConstants.timeout);
            var response = await TryAPICall(httpClient.PatchAsync(apiUrl, content, cancellationTokenSource.Token));
            //  var response = await httpClient.PostAsync(apiUrl, content);
            string jsonResponse = string.Empty;
            if (response.IsSuccessStatusCode)
            {
                jsonResponse = await response.Content.ReadAsStringAsync();
                return true;
            }

            return false;
        }
    }
}
