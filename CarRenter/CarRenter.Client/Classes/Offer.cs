namespace CarRenter.Client.Classes
{
    public class Offer
    {
        static int counter = 0;
        public int Id { get; set; }
        public Car Car { get; set; }
        public User? User { get; set; }
        public float price { get; set; }
        public Offer(Car car)
        {
            Car = car;
            Id = counter++;
            price = 9999.99f;
        }
        public static List<Offer> GetOffersFor(string model, string brand)
        {
            List<Offer> offers = new List<Offer>();
            Dictionary<string, Dictionary<string, List<Car>>> cars = Car.LoadCarsFrom();
            if (cars.ContainsKey(brand))
            {
                if (cars[brand].ContainsKey(model))
                {
                    foreach (var car in cars[brand][model])
                    {
                        offers.Add(new Offer(car));
                    }
                }
            }

            return offers;
        }
    }
}
