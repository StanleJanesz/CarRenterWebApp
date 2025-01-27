using System.Drawing;

namespace CarProviderAPI.model.Cars
{
    public class CarImages
    {
        public int Id { get; set; }
        public byte[] Image { get; set; }
        public int CarId { get; set; }
    }
}
