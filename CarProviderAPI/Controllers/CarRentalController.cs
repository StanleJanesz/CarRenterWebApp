using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarRenterAPI.Controllers;
using CarRenterAPI.model.Cars;
using CarRenterAPI.model.ProviderAPIs;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Caching.Memory;
using static System.Net.WebRequestMethods;
using static CarRenterAPI.model.Cars.OfferDto;

namespace CarProviderAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CarRentalController : ControllerBase
	{
		private readonly ILogger<CarRentalController> _logger;

		private string RentalApiBaseUrl = Environment.GetEnvironmentVariable("Rental_API_Base_Url");
		private string ProviderApiBaseUrl = Environment.GetEnvironmentVariable("Provider_API_Base_Url");

		private string RentalApiKey = Environment.GetEnvironmentVariable("Rental_API_Key");
		private string ProviderApiKey = Environment.GetEnvironmentVariable("Provider_API_Key");

		private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		public CarRentalController(ILogger<CarRentalController> logger)
		{
			_logger = logger;
		}

		[UserKey]
		[HttpGet("cars")]
		public async Task<IEnumerable<Car>> GetCars()
		{
			List<Car> carslist = new List<Car>();
			List<ForeginCar> carslist2 = new List<ForeginCar>();

			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", RentalApiKey);
			var apiUrl = RentalApiBaseUrl + "get-cars-db";
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);

			string response = string.Empty;
			response = await client.GetStringAsync(apiUrl, cancellationTokenSource.Token);
			if (response != string.Empty)
			{
				carslist = JsonSerializer.Deserialize<List<Car>>(response, jsonOptions);
			}
			carslist.ForEach(x => x.ProviderId = 0);

			apiUrl = ProviderApiBaseUrl + "cars";
			client.DefaultRequestHeaders.Add("Provider-Api-Key", ProviderApiKey);

			response = string.Empty;
			response = await client.GetStringAsync(apiUrl);

			if (response != string.Empty)
			{
				carslist2 = JsonSerializer.Deserialize<List<ForeginCar>>(response, jsonOptions);
			}

			foreach (var foreigncar in carslist2)
			{
				Car car = foreigncar.Convert();
				car.ProviderId = 1;
				carslist.Add(car);
			}

			return carslist;
		}

		[UserKey]
		[HttpPost("generate-offer")]
		public async Task<ActionResult<Offer>> GenerateOffer([FromBody] RentalRequest request)
		{
			if (request == null || request.CarId <= 0)
			{
				return BadRequest("Invalid input data.");
			}

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Rental-API-Key", RentalApiKey);
			httpClient.DefaultRequestHeaders.Add("Provider-Api-Key", ProviderApiKey);

			if (request.ProviderId == 0)
			{
				var content = JsonContent.Create(request);
				var apiUrl = RentalApiBaseUrl + "generate-offer";
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
				HttpResponseMessage response = await httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token);

				string jsonResponse = string.Empty;
				Offer offer = null;
				if (response.IsSuccessStatusCode)
				{
					jsonResponse = await response.Content.ReadAsStringAsync();
					offer = JsonSerializer.Deserialize<Offer>(jsonResponse, jsonOptions);
					offer.ProviderId = 0;
				}

				return Ok(offer);
			}
			else
			{
				var apiUrl = ProviderApiBaseUrl + $"cars/{request.CarId}/price";
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);

				var response = await httpClient.GetStringAsync(apiUrl, cancellationTokenSource.Token);
				OfferDto offerDto = new OfferDto();
				if (response != string.Empty)
				{
					offerDto = JsonSerializer.Deserialize<OfferDto>(response, jsonOptions);
				}

				Offer offer = await offerDto.Convert(request.CarId, request);
				return Ok(offer);
			}

		}

		[UserKey]
		[HttpPost("select-offer")]
		public async Task<ActionResult<RentalConfirmation>> SelectOffer([FromBody] RentalSelectionRequest request)
		{
			if (request == null || request.OfferId < 0)
			{
				return BadRequest("Invalid input data.");
			}
			var apiUrl = RentalApiBaseUrl + "select-offer";

			using var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("Rental-API-Key", RentalApiKey);
			httpClient.DefaultRequestHeaders.Add("Provider-Api-Key", ProviderApiKey);

			if (request.ProviderId == 0)
			{
				var content = JsonContent.Create(request);

				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
				var response = await httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token);

				string jsonResponse = string.Empty;
				RentalConfirmation confirmation = null;
				if (response.IsSuccessStatusCode)
				{
					jsonResponse = await response.Content.ReadAsStringAsync();
					confirmation = JsonSerializer.Deserialize<RentalConfirmation>(jsonResponse, jsonOptions);
				}

				return Ok(confirmation);
			}
			else
			{
				RentalDto dto = new RentalDto
				{
					OfferId = request.OfferId,
					UserEmail = request.UserEmail,
					StartDate = DateTime.Now
				};
				apiUrl = ProviderApiBaseUrl + "rentals";
				var content = JsonContent.Create(request);


				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
				var response = await httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token);
				string jsonResponse = string.Empty;
				int rentalId = 0;

				if (response.IsSuccessStatusCode)
				{
					jsonResponse = await response.Content.ReadAsStringAsync();
					rentalId = JsonSerializer.Deserialize<int>(jsonResponse, jsonOptions);
				}
				var confirmation = new RentalConfirmation
				{
					OfferId = request.OfferId,
					UserEmail = request.UserEmail,
					ConfirmationMessage = "The offer has been successfully selected and your rental is confirmed."
				};
				return Ok(confirmation);
			}
		}

		[UserKey]
		[HttpGet("user-rentals")]
		public async Task<IEnumerable<Rental>> GetUserRentals([FromQuery] string userEmail)
		{
			List<Rental> rentalslist = new List<Rental>();
			List<RentalDataDto> foreignrentals = new List<RentalDataDto>();

			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("Rental-API-Key", RentalApiKey);

			string url = RentalApiBaseUrl + "user-rentals?userEmail=" + userEmail;
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			var response = await client.GetStringAsync(url, cancellationTokenSource.Token);

			if (response != string.Empty)
			{
				rentalslist = JsonSerializer.Deserialize<List<Rental>>(response, jsonOptions);
			}
			rentalslist.ForEach(x => x.ProviderId = 0);

			url = ProviderApiBaseUrl + $"rentals/email/{userEmail}";
			client.DefaultRequestHeaders.Add("Provider-Api-Key", ProviderApiKey);

			response = string.Empty;
			response = await client.GetStringAsync(url);

			if (response != string.Empty)
			{
				foreignrentals = JsonSerializer.Deserialize<List<RentalDataDto>>(response, jsonOptions);
			}

			foreach (var foreignrental in foreignrentals)
			{
				if (foreignrental.CarState != RentalDataDto.State.Expired && foreignrental.CarState != RentalDataDto.State.Created)
				{
					Rental rental = foreignrental.Convert();
					rental.ProviderId = 1;
					rentalslist.Add(rental);
				}
			}

			return rentalslist;
		}

		[UserKey]
		[HttpPost("start-return")]
		public async Task<ActionResult> StartCarReturn([FromBody] CarReturnRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Message = "Invalid request data." });
			}

			if (request.ProviderId == 0)
			{
				var apiUrl = RentalApiBaseUrl + "start-return";

				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Rental-API-Key", RentalApiKey);

				var content = JsonContent.Create(request);

				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
				var response = await httpClient.PostAsync(apiUrl, content, cancellationTokenSource.Token);

				string jsonResponse = string.Empty;
				if (response.IsSuccessStatusCode)
				{
					jsonResponse = await response.Content.ReadAsStringAsync();
					return Ok();
				}
			}
			else
			{
				var apiUrl = ProviderApiBaseUrl + $"rentals/{request.RentalId}/return";
				using var httpClient = new HttpClient();
				httpClient.DefaultRequestHeaders.Add("Provider-Api-Key", ProviderApiKey);

				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
				var response = await httpClient.PostAsync(apiUrl, null, cancellationTokenSource.Token);

				if (response.IsSuccessStatusCode)
				{
					return Ok();
				}
			}
			return Ok();
		}
	}
}
