namespace Prototype_Backend.Records
{
    public class Records
    {
        public record UserRegistrationQuery(string FullName, string Username, string Email, string Profile, string PhoneNumber, string Password);
        public record UserLoginRequest(string Username, string Password);

        public record UserRegistrationResponse(string Username, string Message);
    }
}
