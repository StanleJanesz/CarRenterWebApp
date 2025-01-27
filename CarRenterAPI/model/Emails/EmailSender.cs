using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
namespace CarProviderAPI.model.Emails

{
	public class EmailSender
	{
		private string SendGridApiKey = Environment.GetEnvironmentVariable("SendGrid_API_Key");
		private string apiEndpoint = "https://carprovider.azurewebsites.net/api/Car/";
		public async Task SendRentEmail(string emailAdress)
		{
			var client = new SendGridClient(SendGridApiKey);

			var msg = new SendGridMessage();
			msg.SetFrom(new EmailAddress("carrentapp80@gmail.com", "Car Renter App"));
			msg.AddTo(new EmailAddress(emailAdress, emailAdress.Split("@")[0]));
			msg.PlainTextContent = "you are about to rent a car!";
			msg.Subject = "Car Rent App - Rental Confirmation";

			var response = await client.SendEmailAsync(msg);
		}

		public async Task SendReturnEmail(string emailAdress, double price)
		{
			var client = new SendGridClient(SendGridApiKey);

			var msg = new SendGridMessage();
			msg.SetFrom(new EmailAddress("carrentapp80@gmail.com", "Car Renter App"));
			msg.AddTo(new EmailAddress(emailAdress, emailAdress.Split("@")[0]));
			msg.PlainTextContent = $"we got the car back, you have to pay {price:F2}";
			msg.Subject = "Car Rent App - Rental Confirmation";

			var response = await client.SendEmailAsync(msg);
		}

		public async Task SendRentalActivationEmail(string emailAdress, int rentalId)
		{
			var client = new SendGridClient(SendGridApiKey);
			var from = new EmailAddress("carrentapp80@gmail.com", "Car Renter App");
			var subject = "Activate Your Rental";
			var to = new EmailAddress(emailAdress);
			var plainTextContent = "Please activate your rental by clicking the button below.";
			var htmlContent = $@"<p>Please activate your rental by clicking the button below.</p>" +
			$"<a href=\"{apiEndpoint}rentals/activate/{rentalId}\" " +
				  "style = display: inline-block; padding: 10px 20px; color: white; background-color: #007BFF;" +
				  "text-decoration: none; border-radius: 5px;>" +
				"Activate Rental" +
			"</a>";

			var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
