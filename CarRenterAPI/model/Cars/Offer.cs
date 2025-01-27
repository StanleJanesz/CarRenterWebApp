using CarProviderAPI.model.Users;
namespace CarProviderAPI.model.Cars
{
	public class Offer
	{
		private static int _nextId = 1;
		private static readonly object _lock = new object();

		public int Id { get; set; }
		public int CarId { get; set; }
		public Car Car { get; set; }
		public string UserEmail { get; set; }
		public double TotalCost { get; set; }
		public double DailyCarRate { get; set; }
		public double DailyInsuranceRate { get; set; }
		public double DailyAdditionalRate { get; set; }
		public CompanyDetails CompanyInfo { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public string Terms { get; set; }

		public Offer()
		{
			lock (_lock)
			{
				Id = _nextId++;
			}
		}

		public static Offer GetOffer(RentalRequest request, double totalcost, double basePricePerDay, double insuranceCostPerDay, double additionalCostPerDay, Car car)
		{
			Offer offer = new Offer();
			offer.Car = car;
			offer.CarId = request.CarId;
			offer.UserEmail = request.UserEmail;
			offer.TotalCost = totalcost;
			offer.StartDate = request.StartDate;
			offer.EndDate = request.EndDate;
			offer.Terms = "The car must be returned in the same condition. Fuel must be refilled.";
			offer.CompanyInfo = new CompanyDetails
			{
				Name = "CarRenter",
				Address = "1234 Main St, Springfield, IL 62701",
				Contact = "555-555-5555"
			};
			offer.DailyCarRate = basePricePerDay;
			offer.DailyInsuranceRate = insuranceCostPerDay;
			offer.DailyAdditionalRate = additionalCostPerDay;
			return offer;
		}
	}
}
