namespace CarRenter.Client.Classes
{
    public class Car
    {
        public string model { get; set; }
        public string brand { get; set; }
        public int id { get; set; }
        public int productionYear { get; set; }
        public Car(int id, string model, string brand, int productionYear)
        {
            this.id = id;
            this.model = model;
            this.brand = brand;
            this.productionYear = productionYear;
        }
        public Car()
        {

        }
        public static Dictionary<string, Dictionary<string, List<Car>>> LoadCarsFrom()
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
        static Dictionary<string, Dictionary<string, List<Car>>> AddCarToDic(Dictionary<string, Dictionary<string, List<Car>>> cars, Car car)
        {
            if (!cars.ContainsKey(car.brand)) cars.Add(car.brand, new Dictionary<string, List<Car>>());
            if (!cars[car.brand].ContainsKey(car.model)) cars[car.brand].Add(car.model, new List<Car>());
            cars[car.brand][car.model].Add(car);
            return cars;
        }
    }
}
