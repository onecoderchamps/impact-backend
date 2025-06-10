

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

        public async Task<object> UpdateProfile(string id, UpdateProfileDto item)
        {
            try
            {
                var Bank = await Setting.Find(d => d.Key == "IuranTahunan").FirstOrDefaultAsync() ?? throw new CustomException(400, "Data", "Data not found");
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                if (roleData.Balance < Bank.Value)
                {
                    throw new CustomException(400, "Error", "Saldo tidak cukup untuk melakukan pembayaran.");
                }
                // Cek apakah transaksi koperasi tahunan tahun ini sudah ada
                var now = DateTime.Now;
                var startOfYear = new DateTime(now.Year, 1, 1);
                var endOfYear = new DateTime(now.Year, 12, 31, 23, 59, 59);

                var filter = Builders<Transaksi>.Filter.And(
                    Builders<Transaksi>.Filter.Eq(_ => _.Type, "KoperasiTahunan"),
                    Builders<Transaksi>.Filter.Eq(_ => _.IdUser, roleData.Phone),
                    Builders<Transaksi>.Filter.Gte(_ => _.CreatedAt, startOfYear),
                    Builders<Transaksi>.Filter.Lte(_ => _.CreatedAt, endOfYear)
                );

                var existingTransaction = await Transaksi.Find(filter).FirstOrDefaultAsync();
                if (existingTransaction != null)
                {
                    throw new CustomException(400, "Error", "Transaksi koperasi tahunan tahun ini sudah ada.");
                }

                var transaksi = new Transaksi
                {
                    Id = Guid.NewGuid().ToString(),
                    IdUser = roleData.Phone,
                    IdTransaksi = Guid.NewGuid().ToString(),
                    Type = "KoperasiTahunan",
                    Nominal = Bank.Value,
                    Ket = "Iuran Tahunan Koperasi",
                    Status = "Expense",
                    CreatedAt = DateTime.Now
                };
                await Transaksi.InsertOneAsync(transaksi);

                roleData.Balance -= Bank.Value;
                roleData.FullName = item.FullName;
                roleData.Email = item.Email;
                roleData.Address = item.Address;
                await dataUser.ReplaceOneAsync(x => x.Phone == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> UpdateUserProfile(string id, UpdateFCMProfileDto item)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data tidak ada");
                await dataUser.ReplaceOneAsync(x => x.Phone == id, roleData);
                return new { code = 200, Message = "Update Berhasil" };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> SendNotif(PayloadNotifSend item)
        {
            try
            {
                string ServerKey = "AIzaSyDUKDzEbHSUxeI_d0Y8pAkEi8SydSz-TvQ";
                using var client = new HttpClient();
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={ServerKey}");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                var payload = new
                {
                    to = item.FCM,
                    notification = new
                    {
                        title = item.Title,
                        body = item.Body
                    },
                    data = new
                    {
                        forceOpen = "true" // Bisa digunakan untuk membuka app otomatis di Android
                    }
                };
                string json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("https://fcm.googleapis.com/fcm/send", content);
                string responseString = await response.Content.ReadAsStringAsync();
                return new { code = 200, Message = "Notification sent successfully", Response = responseString };
            }
            catch (CustomException ex)
            {
                throw;
            }
        }

        public async Task<object> Aktifasi(string id)
        {
            try
            {
                var roleData = await dataUser.Find(x => x.Phone == id).FirstOrDefaultAsync() ?? throw new CustomException(400, "Error", "Data not found");
                // Cek apakah transaksi koperasi tahunan tahun ini sudah ada
                var now = DateTime.Now;
                var startOfYear = new DateTime(now.Year, 1, 1);
                var endOfYear = new DateTime(now.Year, 12, 31, 23, 59, 59);

                var filter = Builders<Transaksi>.Filter.And(
                    Builders<Transaksi>.Filter.Eq(_ => _.Type, "KoperasiTahunan"),
                    Builders<Transaksi>.Filter.Eq(_ => _.IdUser, roleData.Phone),
                    Builders<Transaksi>.Filter.Gte(_ => _.CreatedAt, startOfYear),
                    Builders<Transaksi>.Filter.Lte(_ => _.CreatedAt, endOfYear)
                );

                var existingTransaction = await Transaksi.Find(filter).FirstOrDefaultAsync();
                var user = new ModelViewUser
                {
                    Phone = roleData.Phone,
                    FullName = roleData.FullName,
                    Balance = roleData.Balance,
                    Image = roleData.Image,
                    Email = roleData.Email,
                    IsMember = existingTransaction != null,
                    Role = roleData.IdRole,
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
                    Image = "https://beres-backend-609517395039.asia-southeast2.run.app/api/v1/file/review/68333845226d836a9b5eb15c",
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


    }
}