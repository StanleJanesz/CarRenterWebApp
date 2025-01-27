using Microsoft.VisualStudio.TestTools.UnitTesting;
using CarRenterWebApp.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRenterWebApp.Classes.Tests
{
	[TestClass()]
	public class APICallTests
	{
		[TestMethod()]
		public async Task IsEmployeeTest()
		{
			var x = await APICall.IsEmployee("john.doe@example.com");
			Assert.IsTrue(x == "worker");
		}

		[TestMethod()]
		public async Task RentalStatusTest()
		{
			var x = await APICall.RentalStatus(1, 1);
			Assert.IsTrue(x == Rental.RentalStatus.Active);
		}

		[TestMethod()]
		public async Task GetCarImageTest()
		{
			var x = await APICall.GetCarImage(3);
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task EmailTest()
		{
			var x = await APICall.Email("xxmarcin007@gmail.com");
			Assert.IsTrue(x);
		}

		[TestMethod()]
		public async Task GetRentalsTest()
		{
			var x = await APICall.GetUsersRentals("carrentapp80@gmail.com");
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task GetCarsTest()
		{
			var x = await APICall.GetCars();
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task GetActiveRentalsTest()
		{
			var x = await APICall.GetActiveRentals();
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task AddUserTest()
		{
			var user = new User
			{
				Email = "costam2",
				BirthDate = DateTime.Now,
				LicenseDate = DateTime.Now,
				Location = "costam",
				IsEmployee = false
			};
			var x = await APICall.AddUser(user);

			Assert.IsTrue(x);
		}

		[TestMethod()]
		public async Task GetUserDataTest()
		{
			var x = await APICall.GetUserData("costam");
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task GenerateOfferTest()
		{
			var x = new RentalRequest
			{
				CarId = 2,
				UserEmail = "costam",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(2),
				Insurance = RentalRequest.InsuranceTypes.Basic,
				IncludeAdditionalServices = false,
				IsFinished = false,
				Pending = false
			};
			//x.Car = new Car
			//{
			//	Id = 2,
			//	Brand = "costam",
			//	Model = "costam",
			//	Location = "costam",
			//	Price = 100,
			//	ProductionYear = 2020,
			//	IsAvailable = true
			//};

			var y = await APICall.GenerateOffer(x);
			Assert.IsNotNull(y);
		}

		[TestMethod()]
		public async Task RentTest()
		{
			var x = new RentalRequest
			{
				CarId = 2,
				UserEmail = "costam",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(2),
				Insurance = RentalRequest.InsuranceTypes.Basic,
				IncludeAdditionalServices = false,
				IsFinished = false,
				Pending = false
			};

			var y = await APICall.GenerateOffer(x);
			var a = new RentalSelectionRequest
			{
				OfferId = y.Id,
				UserEmail = y.UserEmail
			};
			var b = await APICall.Rent(a);
			Assert.IsNotNull(b);
		}

		[TestMethod()]
		public async Task StartReturnTest()
		{
			var x = new CarReturnRequest
			{
				RentalId = 7,
				UserEmail = "costam"
			};

			var y = await APICall.StartReturn(x);

			Assert.IsTrue(y);
		}

		[TestMethod()]
		public async Task ConfirmReturnTest()
		{
			var image = File.ReadAllBytes("C:\\Users\\xx-ma\\prog\\dotnet\\CarRenter\\CarRenterAPI\\car_images\\toyota.jpg");
			var x = await APICall.ConfirmReturn(7, 1, "blabla", image);
			Assert.IsTrue(x);
		}

		[TestMethod()]
		public async Task EditUserTest()
		{
			var user = new User
			{
				Id = 7,
				Email = "costam2",
				BirthDate = DateTime.Now,
				LicenseDate = DateTime.Now + TimeSpan.FromDays(10),
				Location = "nigdzie",
				IsEmployee = false
			};
			var x = await APICall.EditUser(user);
			Assert.IsTrue(x);
		}

		[TestMethod()]
		public async Task GetCarsMidTest()
		{
			var x = await APICall.GetCarsMid();
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task GenerateOfferMidTest()
		{
			var x = new RentalRequest
			{
				CarId = 3,
				UserEmail = "costam",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(2),
				Insurance = RentalRequest.InsuranceTypes.Basic,
				IncludeAdditionalServices = false,
				ProviderId = 1,
				IsFinished = false,
				Pending = false
			};

			var y = await APICall.GenerateOfferMid(x);
			Assert.IsNotNull(y);
		}

		[TestMethod()]
		public async Task RentMidTest()
		{
			var x = new RentalRequest
			{
				CarId = 4,
				UserEmail = "xxmarcin007@gmail.com",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(2),
				Insurance = RentalRequest.InsuranceTypes.Basic,
				IncludeAdditionalServices = false,
				ProviderId = 1,
				IsFinished = false,
				Pending = false
			};

			var y = await APICall.GenerateOfferMid(x);
			var a = new RentalSelectionRequest
			{
				OfferId = y.Id,
				UserEmail = y.UserEmail,
				ProviderId = y.ProviderId
			};
			var b = await APICall.RentMid(a);
			Assert.IsNotNull(b);
		}

		[TestMethod()]
		public async Task GetUsersRentalsMidTest()
		{
			var x = await APICall.GetUsersRentalsMid("xxmarcin007@gmail.com");
			Assert.IsNotNull(x);
		}

		[TestMethod()]
		public async Task StartReturnMidTest()
		{
			var x = new CarReturnRequest
			{
				RentalId = 3,
				UserEmail = "xxmarcin007@gmail.com",
				ProviderId = 1
			};

			var y = await APICall.StartReturnMid(x);

			Assert.IsTrue(y);
		}
	}
}