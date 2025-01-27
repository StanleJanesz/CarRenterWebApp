namespace CarRenterAPI.model.Cars
{
	using System;
	using System.Collections.Generic;
	using System.Text.Json;
	using System.Text.Json.Serialization;

	public class Brand
	{
		public int BrandId { get; set; }
		public string BrandName { get; set; }
	}

	public class Model
	{
		public int ModelId { get; set; }
		public string ModelName { get; set; }
		public int SeatCount { get; set; }
		public int DoorCount { get; set; }
		public string EngineType { get; set; }
		public int Horsepower { get; set; }
		public int Torque { get; set; }
		public int Length { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
	}

	public class ForeginCar
	{
		public int CarId { get; set; }
		public int BrandId { get; set; }
		public Brand Brand { get; set; }
		public int ModelId { get; set; }
		public Model Model { get; set; }
		public int Year { get; set; }
		public string Color { get; set; }
		public Fuel FuelType { get; set; }
		public Gearbox GearboxType { get; set; }
		public decimal Mileage { get; set; }
		public bool IsRented { get; set; }
		public string Location { get; set; }
		public decimal RentalCost { get; set; }
		public decimal InsuranceCost { get; set; }

		public enum Fuel { Petrol, Diesel, LPG, Electric }
		public enum Gearbox { Manual, Automatic }

		public Car Convert()
		{
			return new Car
			{
				Brand = Brand.BrandName,
				Model = Model.ModelName,
				ProductionYear = Year,
				Price = (double)RentalCost,
				Location = Location,
				IsAvailable = !IsRented,
				Id = CarId
			};
		}
	}
	public class CarService
	{
		public int ServiceId { get; set; }
		public string ServiceName { get; set; }
		public decimal ServiceDayCost { get; set; }
		public decimal ServiceCost { get; set; }
	}

	public class OfferDto
	{
		public int OfferId { get; set; }
		public decimal DailyPrice { get; set; }
		public async Task<Offer> Convert(int carId, RentalRequest request)
		{
			List<ForeginCar> carslist2 = new List<ForeginCar>();
			using HttpClient client = new HttpClient();
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(15000);
			string response = string.Empty;

			var apiUrl = Environment.GetEnvironmentVariable("Provider_API_Base_Url") + "cars";
			client.DefaultRequestHeaders.Add("Provider-Api-Key", Environment.GetEnvironmentVariable("Provider_API_Key"));
			
			JsonSerializerOptions jsonOptions = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true,
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
			};

			response = string.Empty;
			response = await client.GetStringAsync(apiUrl);
			if (response != string.Empty)
			{
				carslist2 = JsonSerializer.Deserialize<List<ForeginCar>>(response, jsonOptions);
			}
			Car car = carslist2.Where(car2 => car2.CarId == carId).FirstOrDefault().Convert();

			return new Offer
			{
				Id = OfferId,
				CarId = carId,
				Car = car,
				UserEmail = request.UserEmail,
				TotalCost = 999.99f,
				DailyCarRate = (double)DailyPrice,
				DailyInsuranceRate = Car.BasicInsuranceRate * (double)DailyPrice,
				DailyAdditionalRate = Car.AdditionalCostsRate * (double)DailyPrice,
				CompanyInfo = new CompanyDetails
				{
					Name = "ForeignRenter",
					Address = "555 Main St, Warsaw, PL 62701",
					Contact = "666-666-666"
				},
				StartDate = DateTime.Now,
				EndDate = DateTime.Now + TimeSpan.FromDays(3),
				Terms = "The car must be returned in the same condition. Fuel must be refilled.",
				ProviderId = 1

			};
		}

		public class RentalDataDto
		{
			public int RentalId { get; set; }
			public int CarId { get; set; }
			public ForeginCar Car { get; set; }
			public int ComparisionApiId { get; set; }
			public string UserEmail { get; set; }
			public DateTime RentalDate { get; set; }
			public string RentalPlace { get; set; }
			public DateTime? ReturnDate { get; set; }
			public decimal AmmountDue { get; set; }
			public List<CarService> CarServices { get; set; }
			public string PublicFileUrl { get; set; }
			public string Description { get; set; }
			public int? EmployeeId { get; set; }
			public State CarState { get; set; }
			public enum State { Created, Expired, Accepted, ReturnStart, ReturnEnd }
			public Rental Convert()
			{
				var enddate = ReturnDate ?? DateTime.Now + TimeSpan.FromDays(3);
				var status = Rental.RentalStatus.Active;
				switch (CarState)
				{
					case State.Created:
						status = Rental.RentalStatus.Pending;
						break;
					case State.Expired:
						status = Rental.RentalStatus.Cancelled;
						break;
					case State.Accepted:
						status = Rental.RentalStatus.Active;
						break;
					case State.ReturnStart:
						status = Rental.RentalStatus.ReadyToReturn;
						break;
					case State.ReturnEnd:
						status = Rental.RentalStatus.Completed;
						break;
				}
				return new Rental
				{
					Id = RentalId,
					CarId = CarId,
					Car = Car.Convert(),
					UserEmail = UserEmail,
					RentalStartDate = RentalDate,
					RentalEndDate = enddate,
					TotalCost = (double)AmmountDue,
					Notes = Description ?? "brak notatek",
					Status = status
				};
			}
		}

		public class RentalDto
		{
			public int OfferId { get; set; }
			public DateTime StartDate { get; set; }
			public string UserEmail { get; set; }
		}
	}
}
