namespace CarRenterAPI.model.Cars
{
	public class RentalRequest
	{
		public int CarId { get; set; }
		public string UserEmail { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public InsuranceTypes Insurance { get; set; }
		public enum InsuranceTypes { Basic, Standard, Premium }
		public bool IncludeAdditionalServices { get; set; }
		public int ProviderId { get; set; }
	}

	public class RentalSelectionRequest
	{
		public int OfferId { get; set; }
		public string UserEmail { get; set; }
		public int ProviderId { get; set; }
	}

	public class RentalConfirmation
	{
		public int OfferId { get; set; }
		public string UserEmail { get; set; }
		public string ConfirmationMessage { get; set; }
	}

	public class CarReturnRequest
	{
		public int RentalId { get; set; }
		public string UserEmail { get; set; }
		public int ProviderId { get; set; }
	}
}
