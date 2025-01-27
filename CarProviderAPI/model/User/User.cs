namespace CarRenterAPI.model.Users
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public DrivingLicenceInfo LicenceInfo { get; set; }
    }
    public record class DrivingLicenceInfo
    {
        public required string LicenceId {  get; init; }
        public required DateOnly ExpirationDate { get; init; }
    }

}
