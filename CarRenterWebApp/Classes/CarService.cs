namespace CarRenterWebApp.Classes
{
    using System.Collections.Generic;

    public class CarService
    {
        public Dictionary<string, Dictionary<string, List<Car>>> CarsDictionary { get; set; } = new();
        public Car SelectedCar { get; set; }
        public List<Offer> OfferList { get; set; }
        public void SetCarsDictionary(Dictionary<string, Dictionary<string, List<Car>>> dictionary)
        {
            CarsDictionary = dictionary;
        }

        public void SetSelectedCar(Car car)
        {
            SelectedCar = car;
        }
    }
}
