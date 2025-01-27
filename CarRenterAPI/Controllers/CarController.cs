using CarProviderAPI.model.Cars;
using CarProviderAPI.model.Emails;
using CarProviderAPI.model.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SendGrid.Helpers.Mail;
using System.Net.Http;
using System.Net.Mail;
namespace CarProviderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
	public class CarController : ControllerBase
    {
        private readonly ILogger<CarController> _logger;

        private readonly CarContext _context;
		private readonly IMemoryCache _cache;

		public CarController(ILogger<CarController> logger, CarContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
			_cache = cache;
		}

		[UserKey]
		[HttpGet("get-image/{id}")]
        public async Task<IActionResult> GetImage(int id)
        {
            var img = await _context.CarImages.Where(x => x.CarId == id).FirstOrDefaultAsync();
            if (img == null)
            {
                return NotFound();
            }
            return File(img.Image, "image/jpeg");
        }

		[UserKey]
		[HttpGet("get-cars-db")]
        public IEnumerable<Car> GetFromDB()
        {
            return _context.Cars.Where(x => x.IsAvailable == true);
        }

		[UserKey]
		[HttpPost("send-offer")]
        public async Task<ActionResult<string>> SendEmail([FromBody] string emailAdress)
        {
            EmailSender emailSender = new EmailSender();
            await emailSender.SendRentEmail(emailAdress);

            return CreatedAtAction(null, null);
        }

		[UserKey]
		[HttpPost("generate-offer")]
        public ActionResult<Offer> GenerateOffer([FromBody] RentalRequest request)
        {
            if (request == null || request.CarId <= 0)
            {
                return BadRequest("Invalid input data.");
            }

			var car = _context.Cars.Find(request.CarId);

			if (car == null || car.IsAvailable == false)
			{
				return NotFound("Car is not available");
			}

			var basePricePerDay = car.Price; 
            var insuranceCostPerDay = 0.0;
			switch(request.Insurance)
			{
				case RentalRequest.InsuranceTypes.Basic:
					insuranceCostPerDay = Car.BasicInsuranceRate * basePricePerDay;
					break;
				case RentalRequest.InsuranceTypes.Standard:
					insuranceCostPerDay = Car.StandardInsuranceRate * basePricePerDay;
					break;
				case RentalRequest.InsuranceTypes.Premium:
					insuranceCostPerDay = Car.PremiumInsuranceRate * basePricePerDay;
					break;
			}
			var additionalCostPerDay = request.IncludeAdditionalServices ? Car.AdditionalCostsRate * basePricePerDay : 0.0;
            var totalCost = (basePricePerDay + insuranceCostPerDay + additionalCostPerDay) * ((request.EndDate - request.StartDate).Days + 1);

			
			var offer = Offer.GetOffer(request, totalCost, basePricePerDay, insuranceCostPerDay, additionalCostPerDay, car);
			var cacheKey = $"offer_{offer.Id}";
			_cache.Set(cacheKey, offer, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10)).SetAbsoluteExpiration(TimeSpan.FromMinutes(60)));
			
			_cache.TryGetValue(cacheKey, out Offer cachedOffer);
			return Ok(offer);
        }


		[UserKey]
		[HttpPost("select-offer")]
        public async Task<ActionResult<RentalConfirmation>> SelectOffer([FromBody] RentalSelectionRequest request)
        {
            if (request == null || request.OfferId < 0)
            {
                return BadRequest("Invalid input data.");
            }

			if ((!_cache.TryGetValue<Offer>($"offer_{request.OfferId}", out Offer offer)) || offer == null)
			{
				return NotFound("Offer not found.");
			}

			var rentedcar = await _context.Cars.FindAsync(offer.CarId);
            if (rentedcar == null || rentedcar.IsAvailable == false)
            {
                return NotFound("Car is not available");
            }

            else
            {
                rentedcar.IsAvailable = false;
            }

			var rental = _context.Rentals.Add(Rental.GetRental(request, offer));
			await _context.SaveChangesAsync();

			EmailSender emailSender = new EmailSender();
			await emailSender.SendRentalActivationEmail(request.UserEmail, rental.Entity.Id);

			var confirmation = new RentalConfirmation
            {
                OfferId = request.OfferId,
                UserEmail = request.UserEmail,
                ConfirmationMessage = "The offer has been successfully selected and your rental is confirmed."
            };

            return Ok(confirmation);
        }

		[HttpGet("rentals/activate/{rentalId}")]
		public async Task<IActionResult> ActivateRental(int rentalId)
		{
			var rental = await _context.Rentals.FindAsync(rentalId);
			if (rental == null) return NotFound();
			if (rental.Status != Rental.RentalStatus.Pending) return BadRequest("Rental is not pending.");

			rental.Status = Rental.RentalStatus.Active;
			await _context.SaveChangesAsync();

			return Ok();
		}

		[UserKey]
		[HttpPost("start-return")]
		public async Task<ActionResult> StartCarReturn([FromBody] CarReturnRequest request)
		{
			if (request == null)
			{
				return BadRequest(new { Message = "Invalid request data." });
			}

			var rental = await _context.Rentals.FindAsync(request.RentalId);

			if (rental == null || rental.Status != Rental.RentalStatus.Active)
			{
				return NotFound("Rental doesnt exist");
			}
			else
			{
				rental.Status = Rental.RentalStatus.ReadyToReturn;
			}
			await _context.SaveChangesAsync();

			return Ok(new { Message = "The car return process has been successfully started."});
		}

		[UserKey]
		[HttpGet("user-rentals")]
        public async Task<IEnumerable<Rental>> GetUserRentals([FromQuery] string userEmail)
        {			
			var usersrentals = await _context.Rentals.Where(x => x.UserEmail == userEmail).ToListAsync();
            usersrentals.ForEach(x => { 
                var car = _context.Cars.Find(x.CarId); 
                x.Car = car; });
            
			return usersrentals;
        }

		[EmployeeKey]
		[HttpGet("active-rentals")]
		public async Task<IEnumerable<Rental>> GetActiveRentals()
		{
			var activerentals = await _context.Rentals.Where(x => true).ToListAsync();
			activerentals.ForEach(x => {
				var car = _context.Cars.Find(x.CarId);
				x.Car = car;
			});

			return activerentals;
		}

		[UserKey]
		[HttpGet("rental-status")]
		public async Task<ActionResult<Rental.RentalStatus>> GetStatus([FromQuery] int rentalId, [FromQuery] int userId)
		{
			if (rentalId <= 0 || userId <= 0)
			{
				return BadRequest(new { Message = "Invalid request parameters." });
			}

			var rental = await _context.Rentals.FindAsync(rentalId);
			if (rental == null)
			{
				return NotFound("Rental doesnt exist");
			}

            return Ok(rental.Status);
		}

		[EmployeeKey]
		[HttpPost("confirm-return")]
		public async Task<IActionResult> ConfirmCarReturn([FromForm] CarReturnConfirmationRequest request)
		{
			if (request == null || request.RentalId <= 0 || request.EmployeeId <= 0 || request.Attachments == null || request.Attachments.Length == 0)
			{
				return BadRequest(new { Message = "Invalid request data." });
			}


			var rental = await _context.Rentals.FindAsync(request.RentalId);
			if (rental == null || rental.Status != Rental.RentalStatus.ReadyToReturn)
			{
				return NotFound(new { Message = "Invalid rental ID or rental status." });
			}
			rental.Status = Rental.RentalStatus.Completed;

			var car = await _context.Cars.FindAsync(rental.CarId);
			if (car == null)
			{
				return NotFound(new { Message = "Car not found." });
			}
			car.IsAvailable = true;

			await _context.SaveChangesAsync();

			EmailSender emailSender = new EmailSender();
			await emailSender.SendReturnEmail(rental.UserEmail, rental.TotalCost);

			foreach (var attachment in request.Attachments)
			{
				var image = new FinishedRentalImages
				{
					Image = await FinishedRentalImages.ConvertToByteArrayAsync(attachment),
					RentalId = request.RentalId
				};
				await _context.FinishedRentalImages.AddAsync(image);
			}
			await _context.SaveChangesAsync();

			return Ok(new { Message = "The car return has been successfully confirmed.", RentalId = request.RentalId });
		}


		[EmployeeKey]
		[HttpGet("is-employee")]
		public async Task<ActionResult<bool>> IsEmployee([FromQuery] string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return BadRequest(new { Message = "Email is required." });
			}

			var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
			if (user != null)
			{
				return Ok(user.IsEmployee);
			}

			return NotFound(new { Message = "User not found for the provided email." });
		}

		[UserKey]
		[HttpPost("user")]
		public async Task<IActionResult> AddUser([FromBody] User newUser)
		{
			if (newUser == null || string.IsNullOrEmpty(newUser.Email))
			{
				return BadRequest("Invalid user data.");
			}

			var existingUser = await _context.Users.Where(x => x.Email == newUser.Email).FirstOrDefaultAsync();
			if (existingUser != null)
			{
				return BadRequest("User with this email already exists.");
			}

			newUser.Id = 0;
			newUser.IsEmployee = false;
			await _context.Users.AddAsync(newUser);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "User added successfully." });
		}

		[UserKey]
		[HttpGet("user")]
		public async Task<ActionResult<User>> GetUserByEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return BadRequest(new { Message = "Email is required." });
			}

			var user = await _context.Users.Where(x => x.Email == email).FirstOrDefaultAsync();
			if (user == null)
			{
				return NotFound(new { Message = "User not found for the provided email." });
			}

			return Ok(user);
		}

		[UserKey]
		[HttpPatch("user")]
		public async Task<IActionResult> UpdateUser([FromBody] User updatedUser)
		{
			if (updatedUser == null || updatedUser.Id <= 0)
			{
				return BadRequest("Invalid user data.");
			}

			var existingUser = await _context.Users.FindAsync(updatedUser.Id);
			if (existingUser == null)
			{
				return NotFound("User not found.");
			}

			existingUser.BirthDate = updatedUser.BirthDate;
			existingUser.LicenseDate = updatedUser.LicenseDate;
			existingUser.Location = updatedUser.Location;

			_context.Users.Update(existingUser);
			await _context.SaveChangesAsync();

			return Ok(new { Message = "User updated successfully." });
		}

	}
}
