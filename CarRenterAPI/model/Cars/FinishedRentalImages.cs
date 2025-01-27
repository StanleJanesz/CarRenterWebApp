namespace CarProviderAPI.model.Cars
{
	public class FinishedRentalImages
	{
		public int Id { get; set; }
		public byte[] Image { get; set; }
		public int RentalId { get; set; }

		public static async Task<byte[]> ConvertToByteArrayAsync(IFormFile file)
		{
			if (file == null || file.Length == 0)
			{
				return null;
			}

			using (var memoryStream = new MemoryStream())
			{
				await file.CopyToAsync(memoryStream);
				return memoryStream.ToArray();
			}
		}
	}
}
