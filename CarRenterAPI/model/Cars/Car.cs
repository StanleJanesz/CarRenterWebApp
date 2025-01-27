using System.Data.SqlTypes;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace CarProviderAPI.model.Cars
{
    public class Car
    {
        
        public string Model { get; set; }
        public string Brand { get; set; }
        public int Id { get; set; }
        public int ProductionYear { get; set; }
        public bool IsAvailable { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }

        public static double BasicInsuranceRate = 0.1;
		public static double StandardInsuranceRate = 0.2;
		public static double PremiumInsuranceRate = 0.3;
		public static double AdditionalCostsRate = 0.15;
        public Car ()
        {
        }
    }
}
