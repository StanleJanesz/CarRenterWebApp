namespace CarProviderAPI.model.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime LicenseDate { get; set; }
        public string Location { get; set; }
        public bool IsEmployee { get; set; }

    }
}
