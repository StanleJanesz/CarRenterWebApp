namespace CarRenterAPI.model.Cars
{
	public class Rental
	{

		public int Id { get; set; } // Unikalne ID wynajmu
		public string UserEmail { get; set; } // ID użytkownika
		public int CarId { get; set; } // ID samochodu
		public Car Car { get; set; }
		public DateTime RentalStartDate { get; set; } // Data rozpoczęcia wynajmu
		public DateTime RentalEndDate { get; set; } // Data zakończenia wynajmu
		public double TotalCost { get; set; } // Całkowity koszt wynajmu
		public RentalStatus Status { get; set; } // Status wynajmu
		public string Notes { get; set; } // Dodatkowe uwagi do wynajmu
		public int ProviderId { get; set; }
		public Rental()
		{
		}
		public enum RentalStatus
		{
			Active,     // Wynajem w trakcie
			ReadyToReturn, // Samochód gotowy do zwrotu
			Completed,  // Wynajem zakończony
			Cancelled,   // Wynajem anulowany
			Pending
		}

		public static Rental GetRental(RentalSelectionRequest request, Offer offer)
		{
			Rental rental = new Rental
			{
				UserEmail = request.UserEmail,
				CarId = offer.CarId,
				RentalStartDate = offer.StartDate,
				RentalEndDate = offer.EndDate,
				TotalCost = offer.TotalCost,
				Notes = "nothing important",
				Status = Rental.RentalStatus.Active
			};

			return rental;

		}

	}
}
