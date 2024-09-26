namespace Vaccination.Application.Dtos.User
{
    public class UserDetailsResponse
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? SocialSecurityNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? City { get; set; }
        public string? Nationality { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? PhoneNumber { get; set; }
    }
}