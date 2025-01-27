
namespace CarProviderAPI.model.Cars
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
    }

    public class RentalSelectionRequest
    {
        public int OfferId { get; set; }
        public string UserEmail { get; set; }
    }

    public class RentalConfirmation
    {
        public Rental Rental { get; set; }
        public int OfferId { get; set; }
        public string UserEmail { get; set; }
        public string ConfirmationMessage { get; set; }
        public static RentalConfirmation GetRentalConfirmation(RentalSelectionRequest request, Rental rental)
		{
			RentalConfirmation confirmation = new RentalConfirmation
			{
				OfferId = request.OfferId,
				UserEmail = request.UserEmail,
				ConfirmationMessage = "The offer has been successfully selected and your rental is confirmed."
			};

			return confirmation;
		}
	}

	public class CarReturnRequest
	{
		public int RentalId { get; set; }
		public string UserEmail { get; set; }
	}

	public class CarReturnConfirmationRequest
	{
		public int RentalId { get; set; }
		public int EmployeeId { get; set; }
		public string Notes { get; set; }
		public IFormFile[] Attachments { get; set; }
	}
}
