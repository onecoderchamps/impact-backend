
public interface IAuthService
{
    // Task<Object> RegisterGoogleAsync(string model,RegisterGoogleDto login);
    Task<Object> Aktifasi(string id);
    Task<Object> tiktokExchange(string id, TikTokExchangeRequest request);

    Task<Object> UpdateProfileSosmed(string id, UpdateProfileDto item);
    // Task<Object> UpdateUserProfile(string id, UpdateFCMProfileDto item);
    Task<Object> Register(RegisterDto registerDto);
    Task<Object> Login(LoginDto registerDto);
    // Task<Object> SendNotif(PayloadNotifSend payloadNotifSend);
}