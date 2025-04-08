# CarRenterApp
CarRenterApp is a car rental management system built with ASP.NET Core (Backend) and Blazor (Frontend). 
This application allows users to browse and rent available cars, while employees can manage the fleet, track rentals, and process returns.
## Features
User (Customer) Features
- 🔍 Browse available cars with filters (make, model, price range)
- 📅 Check car availability and its details
- 🚗 Rent a car
- 📝 View rental history
- 🔄 Cancel rentals

Employee (Admin) Features
- 📊 Rental Oversight: View all active/pending/completed rentals
- ✅ Process Returns: Mark cars as returned and handle the returns

## Technologies
The backend is a Web API created with ASP.NET Core, while the fronted was made using Blazor WebAssembly. SQL Server with Entity Framework are used to manage the database.
Additionally, app includes authentication with Google Sign-In and sending emails with SendGrid
