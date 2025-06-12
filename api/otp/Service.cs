using MongoDB.Driver;
using impact.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RepositoryPattern.Services.OtpService
{
    public class OtpService : IOtpService
    {
        private readonly IMongoCollection<OtpModel> _otpCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Setting> _settingCollection;


        public OtpService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("impact");
            _otpCollection = database.GetCollection<OtpModel>("OTP");
            _userCollection = database.GetCollection<User>("User");
            _settingCollection = database.GetCollection<Setting>("Setting");
        }

        public async Task<string> SendOtpWAAsync(CreateOtpDto dto)
        {
            // Hapus OTP yang sudah ada sebelumnya
            var existingOtps = await _otpCollection.Find(otp => otp.Phone == dto.Phonenumber).ToListAsync();
            foreach (var otps in existingOtps)
            {
                await _otpCollection.DeleteOneAsync(o => o.Id == otps.Id);
            }
            var authConfig = await _settingCollection.Find(d => d.Key == "authKey").FirstOrDefaultAsync() ?? throw new CustomException(400, "Data", "Data not found");
            var appConfig = await _settingCollection.Find(d => d.Key == "appKey").FirstOrDefaultAsync() ?? throw new CustomException(400, "Data", "Data not found");

            // Generate OTP baru
            var otpCode = new Random().Next(1000, 9999).ToString();
            var otp = new OtpModel
            {
                Phone = dto.Phonenumber,
                CodeOtp = otpCode,
                TypeOtp = dto.Phonenumber,
                CreatedAt = DateTime.UtcNow
            };

            // Simpan OTP ke database
            await _otpCollection.InsertOneAsync(otp);
            var emailBody = $"Your OTP code is: {otpCode}";
            // Kirim email
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var form = new MultipartFormDataContent();
                    form.Add(new StringContent(appConfig.Value ?? string.Empty), "appkey");
                    form.Add(new StringContent(authConfig.Value ?? string.Empty), "authkey");
                    form.Add(new StringContent(dto.Phonenumber), "to");
                    form.Add(new StringContent($"Your code code is {otpCode}"), "message");

                    var response = await httpClient.PostAsync("https://app.saungwa.com/api/create-message", form);
                    var result = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        return $"OTP sent to your WA";
                    }
                    else
                    {
                        return $"Failed to send OTP. Response: {result}";
                    }
                }
            }
            catch (Exception)
            {
                throw new CustomException(400, "Message", "Failed to send OTP email");
            }
        }

        ///role
        ///0 admin
        ///1 user
        ///2 userHigh

        public async Task<object> ValidateOtpWAAsync(ValidateOtpDto dto)
        {
            // Cari OTP berdasarkan email
            var otp = await _otpCollection.Find(o => o.Phone == dto.phonenumber).FirstOrDefaultAsync();

            if (otp == null)
                throw new CustomException(400, "Message", "OTP not found");

            if (otp.CodeOtp != dto.Code)
                throw new CustomException(400, "Message", "OTP invalid");

            var users = await _userCollection.Find(o => o.Phone == dto.phonenumber).FirstOrDefaultAsync();
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            var jwtService = new JwtService(configuration);
            // Hapus OTP setelah validasi
            string token = jwtService.GenerateJwtToken(users.Id, users.Id);
            await _otpCollection.DeleteOneAsync(o => o.Id == otp.Id);
            return new { code = 200, accessToken = token, IdRole = users.IdRole, id = users.Id };
        }

        public class sendForm
        {
            public string? Id { get; set; }
            public string? Phone { get; set; }
            public string? Subject { get; set; }
            public string? Message { get; set; }
            public string? Otp { get; set; }
        }


    }
}
