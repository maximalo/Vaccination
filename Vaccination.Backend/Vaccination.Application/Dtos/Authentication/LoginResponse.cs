namespace Vaccination.Application.Dtos.Authentication
{
    public class LoginResponse
    {
        public DateTime? Expiration { get; set; }
        public bool IsSucceed { get; set; }
        public string Message { get; set; }
        public string? RefreshToken { get; set; }
        public string? Token { get; set; }
    }
}