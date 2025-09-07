

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using CheckId;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using impact.Shared.Models;

namespace RepositoryPattern.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<User> dataUser;
        private readonly IMongoCollection<Transaksi> Transaksi;
        private readonly IMongoCollection<Setting3> Setting;
        private readonly IMongoCollection<OtpModel> _otpCollection;

        private readonly string key;
        private readonly ILogger<AuthService> _logger;
        private readonly IOtpService _otpService;

        public AuthService(IConfiguration configuration, ILogger<AuthService> logger, IOtpService otpService)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            IMongoDatabase database = client.GetDatabase("impact");
            _otpCollection = database.GetCollection<OtpModel>("OTP");
            dataUser = database.GetCollection<User>("User");
            Transaksi = database.GetCollection<Transaksi>("Transaksi");
            Setting = database.GetCollection<Setting3>("Setting");

            this.key = configuration.GetSection("AppSettings")["JwtKey"];
            _logger = logger;
            _otpService = otpService;
        }

        public async Task<object> tiktokExchange(string id, TikTokExchangeRequest request)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync()
                    ?? throw new CustomException(400, "Error", "Account not found");

                using var client = new HttpClient();

                var parameters = new Dictionary<string, string>
                {
                    { "client_key", "sbawgaidkbothlgvz9" },
                    { "client_secret", "RWCb2VfNKzT3FmowyYmrXvwL2Qs1P580" },
                    { "code", request.Code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", request.RedirectUri },
                    { "code_verifier", request.CodeVerifier }
                };

                var content = new FormUrlEncodedContent(parameters);

                // ⚠️ TikTok API terbaru untuk token ada di endpoint ini:
                var response = await client.PostAsync("https://open.tiktokapis.com/v2/oauth/token/", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Tambahkan detail error dari TikTok supaya gampang debug
                    throw new CustomException(
                        400,
                        "TikTok Error",
                        $"Failed to exchange code. TikTok response: {responseString}"
                    );
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var tiktokResponse = JsonSerializer.Deserialize<TikTokTokenResponse>(responseString, options);
                // Console.WriteLine($"TikTok Response: {responseString}");
                if (tiktokResponse == null || string.IsNullOrEmpty(tiktokResponse.AccessToken))
                {
                    throw new CustomException(400, "Error", "Invalid TikTok token response");
                }

                // Simpan token ke user
                roleData.TikTokAccessToken = tiktokResponse.AccessToken;
                roleData.TikTokRefreshToken = tiktokResponse.RefreshToken;
                roleData.TikTokOpenId = tiktokResponse.OpenId;
                // roleData.TikTokScope = tiktokResponse.Scope;
                // roleData.TikTokTokenExpiresAt = DateTime.UtcNow.AddSeconds(tiktokResponse.ExpiresIn);

                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);

                return new
                {
                    code = 200,
                    message = "TikTok linked successfully",
                    openId = tiktokResponse.OpenId,
                    // expiresAt = roleData.TikTokTokenExpiresAt
                };
            }
            catch (CustomException)
            {
                throw; // biarkan custom exception naik
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Error", $"Unexpected error: {ex.Message}");
            }
        }


        public async Task<object> UpdateProfileSosmed(string id, UpdateProfileDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                if (roleData == null)
                {
                    throw new CustomException(400, "Error", "Account not found");
                }

                roleData.Image = item.Image;
                roleData.Bio = item.Bio;
                roleData.Categories = item.Categories;
                roleData.Youtube = item.Youtube;
                roleData.Instagram = item.Instagram;
                roleData.Facebook = item.Facebook;
                roleData.TikTok = item.Tiktok;
                roleData.Linkedin = item.Linkedin;
                roleData.FullName = item.FullName;

                await dataUser.ReplaceOneAsync(x => x.Id == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        // public async Task<object> UpdateUserProfile(string id, UpdateFCMProfileDto item)
        // {
        //     try
        //     {
        //         var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
        //         await dataUser.ReplaceOneAsync(x => x.Phone == id, roleData);
        //         return new { code = 200, Message = "Update Berhasil" };
        //     }
        //     catch (CustomException ex)
        //     {
        //         throw;
        //     }
        // }

        // public async Task<object> SendNotif(PayloadNotifSend item)
        // {
        //     try
        //     {
        //         string ServerKey = "AIzaSyDUKDzEbHSUxeI_d0Y8pAkEi8SydSz-TvQ";
        //         using var client = new HttpClient();
        //         client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={ServerKey}");
        //         client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        //         var payload = new
        //         {
        //             to = item.FCM,
        //             notification = new
        //             {
        //                 title = item.Title,
        //                 body = item.Body
        //             },
        //             data = new
        //             {
        //                 forceOpen = "true" // Bisa digunakan untuk membuka app otomatis di Android
        //             }
        //         };
        //         string json = JsonSerializer.Serialize(payload);
        //         var content = new StringContent(json, Encoding.UTF8, "application/json");
        //         HttpResponseMessage response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
        //         string responseString = await response.Content.ReadAsStringAsync();
        //         return new { code = 200, Message = "Notification sent successfully", Response = responseString };
        //     }
        //     catch (CustomException ex)
        //     {
        //         throw;
        //     }
        // }

        public async Task<object> Aktifasi(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data not found");
                var user = new ModelViewUser
                {
                    Phone = roleData.Phone,
                    FullName = roleData.FullName,
                    Balance = roleData.Balance,
                    Image = roleData.Image,
                    Email = roleData.Email,
                    Role = roleData.IdRole,
                    TikTokAccessToken = roleData.TikTokAccessToken,
                    TikTokRefreshToken = roleData.TikTokRefreshToken,
                    TikTokOpenId = roleData.TikTokOpenId,
                };
                return new { code = 200, Id = roleData.Id, Data = user };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> Register(RegisterDto dto)
        {

            var users = await dataUser.Find(o => o.Phone == dto.PhoneNumber).FirstOrDefaultAsync();
            if (users != null)
                throw new CustomException(400, "Error", "Pengguna sudah terdaftar");
            var uuid = Guid.NewGuid().ToString();
            if (users == null)
            {
                var userModel = new User
                {
                    Id = uuid,
                    FullName = dto.FullName,
                    Phone = dto.PhoneNumber,
                    IdRole = dto.IdRole,
                    Email = dto.Email,
                    Image = "https://apiimpact.coderchamps.co.id/api/v1/file/review/6856e21db01459e31d283331",
                    Balance = 0,
                    Address = "",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                };
                var phone = new CreateOtpDto
                {
                    Phonenumber = dto.PhoneNumber
                };
                await _otpService.SendOtpWAAsync(phone);
                await dataUser.InsertOneAsync(userModel);
                return new { code = 200, message = "Pendaftaran Berhasil" };
            }
            else
            {
                throw new CustomException(400, "Error", "Pengguna sudah terdaftar");
            }
        }

        public async Task<object> Login(LoginDto dto)
        {
            try
            {
                var users = await dataUser.Find(o => o.Phone == dto.PhoneNumber).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Pengguna belum terdaftar silahkan register");
                var phone = new CreateOtpDto
                {
                    Phonenumber = dto.PhoneNumber
                };
                await _otpService.SendOtpWAAsync(phone);
                return new { code = 200, message = "Pendaftaran Berhasil" };
            }
            catch (CustomException ex)
            {

                throw;
            }
        }
    }

    public class ModelViewUser
    {
        public string? Id { get; set; }
        public string? Phone { get; set; }
        public string? FullName { get; set; }
        public float? Balance { get; set; }
        public float? Point { get; set; }
        public string? Fcm { get; set; }
        public string? Image { get; set; }
        public string? Email { get; set; }
        public bool? IsMember { get; set; }
        public string? Role { get; set; }
        public string? TikTokAccessToken {get; set;}
        public string? TikTokRefreshToken {get; set;}
        public string? TikTokOpenId {get; set;}


    }
}