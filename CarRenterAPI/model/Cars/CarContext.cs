using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarProviderAPI.model.Users;
namespace CarProviderAPI.model.Cars
{
    public class CarContext : DbContext
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarImages> CarImages { get; set; }
        public DbSet<Rental> Rentals { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<FinishedRentalImages> FinishedRentalImages { get; set; }
		public string ConnString { get; }
        public CarContext() 
        {
        }
        public CarContext(DbContextOptions<CarContext> options) : base(options)
        {
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder options)
         => options.UseSqlServer(Environment.GetEnvironmentVariable("DB_Conn_String"));
	}
}

