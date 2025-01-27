using CarRenterAPI.model.Cars;
using System.Collections.Specialized;
using System.Diagnostics.Metrics;
using System.IO;
using System.Text.Json;

namespace CarRenterAPI.model.ProviderAPIs
{
    public class CarProvider
    {
        private static int count=0;
        public int Id { get; set; }
        public string name { get; set; }
        public string baseUrl { get; set; }
        public string getCarUrl {  get; set; }
        public CarProvider(string name, string baseUrl,string getCarUrl) 
        {
            this.Id = CarProvider.count++;
            this.name = name;
            this.baseUrl = baseUrl;
            this.getCarUrl = getCarUrl;
        }

        public List<Car> GetCars() 
        {
            HttpClient client = new HttpClient();
            var RESp = client.GetStringAsync(baseUrl + getCarUrl);
            string s = RESp.Result;
            List<Car> cars = JsonSerializer.Deserialize<List<Car>>(s);
            return cars;
        }
    }
}
