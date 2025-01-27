using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRenterWebApp.Pages;
using System.Net.Http.Json;
using System.Text.Json;
using CarRenterWebApp.Classes;
using System.Text;
using System.Net.Mail;

namespace CarRenterWebApp.Pages.Tests
{
	[TestClass()]
	public class OffertsListTests
	{
		//[TestMethod()]
		//public async Task CalApiTest()
		//{
		//	var offers = new OffertsList();
		//	var cars = await offers.CalApi();
		//	Assert.IsNotNull(cars);
		//}

		//[TestMethod()]
		//public async Task CalApiOfferTest()
		//{
		//	RentalRequest request = new RentalRequest();
		//	request.StartDate = DateTime.Now;
		//	request.EndDate = DateTime.Now.AddDays(1);
		//	request.IncludeInsurance = true;
		//	request.IncludeAdditionalServices = true;
		//	var offers = new OffertsList();
		//	var cars = await offers.CalApi();
		//	request.Car = cars.Values.SelectMany(dict => dict.Values).SelectMany(list => list).FirstOrDefault();
		//	request.CarId = request.Car.id;
		//	request.UserEmail = "costam";
		//	var offer = await offers.CalApiOffer(request);

		//	Assert.IsTrue(offer.Price != 0);
		//}

		//[TestMethod()]
		//public async Task CalApiRentTest()
		//{
		//	RentalRequest request = new RentalRequest();
		//	request.StartDate = DateTime.Now;
		//	request.EndDate = DateTime.Now.AddDays(1);
		//	request.IncludeInsurance = true;
		//	request.IncludeAdditionalServices = true;
		//	var offers = new OffertsList();
		//	var cars = await offers.CalApi();
		//	request.Car = cars.Values.SelectMany(dict => dict.Values).SelectMany(list => list).FirstOrDefault();
		//	request.CarId = request.Car.id;
		//	request.UserEmail = "costam";
		//	var offer = await offers.CalApiOffer(request);

		//	RentalSelectionRequest selection = new RentalSelectionRequest();
		//	selection.Offer = offer;
		//	selection.OfferId = offer.Id;
		//	selection.UserEmail = "costam";

		//	var rent = await offers.CalApiRent(selection);

		//	Assert.IsNotNull(rent);
		//}

		//[TestMethod()]
		//public async Task CalApiEmailTest()
		//{
		//	//var offers = new OffertsList();
		//	//var res = await offers.CalApiEmail("xxmarcin007@gmail.com");
		//	using HttpClient client = new HttpClient();
		//	string email = "xxmarcin007@gmail.com";
		//	// Construct the full URL for the API
		//	string url = $"https://localhost:7207/api/Car/send-offer";

		//	var content = JsonContent.Create(email);

		//	// Make the asynchronous POST request
		//	HttpResponseMessage response = await client.PostAsync(url, content);
		//	string result = string.Empty;
		//	// Check if the request was successful
		//	bool res = false;
		//	if (response.IsSuccessStatusCode)
		//	{
		//		// Read the response as a string
		//		result = await response.Content.ReadAsStringAsync();
		//		res = true;
		//	}
		//	Assert.IsTrue(res);
		//}

		//[TestMethod()]
		//public async Task CalApiImageTest()
		//{
		//	var offers = new OffertsList();
		//	var res = await offers.CalApiImage(3);

		//	Assert.IsNotNull(res);
		//}

		//[TestMethod()]
		//public async Task CalApiRentalStatusTest()
		//{
		//	var offers = new OffertsList();
		//	var status = await offers.CalApiRentalStatus(1, 1);
		//	Assert.IsTrue(status==Rental.RentalStatus.Active);
		//}
	}
}