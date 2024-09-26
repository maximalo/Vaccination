namespace Vaccination.Api.Exceptions
{
    public class SecurityTokenException : Exception
    {
        public SecurityTokenException(string message) : base(message)
        { }
    }
}