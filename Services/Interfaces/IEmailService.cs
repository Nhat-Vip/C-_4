public interface IEmailService
{
    Task SendOtpEmail(string email, string otp);
}