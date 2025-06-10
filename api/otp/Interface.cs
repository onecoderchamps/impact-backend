public interface IOtpService
{

    Task<string> SendOtpWAAsync(CreateOtpDto dto);
    Task<Object> ValidateOtpWAAsync(ValidateOtpDto dto);
}