using System.Text.Json.Serialization;
using static CarRenterWebApp.Classes.RentalRequest;

namespace CarRenterWebApp.Classes
{
    public class Offer
    {
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
		public int ProviderId { get; set; }
		public Offer() { }
        public Offer(Car car)
        {
            Id = 0;
            TotalCost = 9999.99f;
        }
        public static async Task<List<Offer>> GetOffersFor(string model, string brand, Dictionary<string, Dictionary<string, List<Car>>> cars,string email,InsuranceTypes insurance,DateTime start,DateTime end)// TODO: moze zamiast foreach jedno zapytanie api dlo wielu 
        {
            List<Offer> offers = new List<Offer>();
            if (cars.ContainsKey(brand))
            {
                if (cars[brand].ContainsKey(model))
                {
                    foreach (var car in cars[brand][model])
                    {
                        Offer offer = offer = await APICall.GenerateOfferMid(new RentalRequest()
                        {
                            CarId = car.Id,
                            EndDate = end,
                            StartDate = start,
                            IncludeAdditionalServices = true,
							Insurance = insurance,
                            UserEmail = email,
                            IsFinished = false,
                            Pending = true,
                            ProviderId = car.ProviderId
                        });
                        offers.Add(offer);
                    }
                }
            }

            return offers;
        }
    }
}
