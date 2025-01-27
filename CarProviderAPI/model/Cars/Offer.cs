using CarRenterAPI.model.Users;
namespace CarRenterAPI.model.Cars
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

    }
}
