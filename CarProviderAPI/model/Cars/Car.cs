
namespace CarRenterAPI.model.Cars
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
		public int ProviderId { get; set; }

		public static double BasicInsuranceRate = 0.1;
		public static double StandardInsuranceRate = 0.2;
		public static double PremiumInsuranceRate = 0.3;
		public static double AdditionalCostsRate = 0.15;
		public Car (int id,string model, string brand,int productionYear)
        {
            this.Id = id;
            this.Model = model;
            this.Brand = brand;
            this.ProductionYear = productionYear;
        }
        public Car ()
        {

        }
        public static Dictionary<string,Dictionary<string, List<Car> >> LoadCarsFromAPI()
        {
            var cars = new Dictionary<string, Dictionary<string, List<Car> >>();
            // load Car from selected provider API 
            for (int i = 0; i < 123; i++)
            {
                string model = "yaris";
                string brand = "toyota";
                int year = 1994;
                int id = 1;
                AddCarToDic(cars, new Car(id, model, brand, year));
            }
            return cars;
        }

        public static Dictionary<string, Dictionary<string, List<Car>>> LoadCarsFromDB()
        {
            var cars = new Dictionary<string, Dictionary<string, List<Car>>>();
            // load Car from selected provider API 
            for (int i = 0; i < 123; i++)
            {
                string model = "yaris";
                string brand = "toyota";
                int year = 1994;
                int id = 1;
                AddCarToDic(cars, new Car(id, model, brand, year));
            }
            return cars;
        }
        public static Dictionary<string, Dictionary<string, List<Car>>> AddCarToDic(Dictionary<string, Dictionary<string, List<Car>> > cars, Car car )
        {
            if (!cars.ContainsKey(car.Brand)) cars.Add(car.Brand, new Dictionary<string, List<Car>>());
            if (!cars[car.Brand].ContainsKey(car.Model)) cars[car.Brand].Add(car.Model, new List<Car>());
            cars[car.Brand][car.Model].Add(car);
            return cars;
        }
    }
}
