using MongoDB.Driver;
using impact.Shared.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RepositoryPattern.Services.ChatService
{
    public class ChatService : IChatService
    {
        private readonly IMongoCollection<ChatModel> _ChatCollection;
        private readonly IMongoCollection<User> _userCollection;


        public ChatService(IConfiguration configuration)
        {
            MongoClient client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("impact");
            _ChatCollection = database.GetCollection<ChatModel>("Chat");
            _userCollection = database.GetCollection<User>("User");
        }

        public async Task<object> SendChatWAAsync(string idUser, CreateChatDto dto)
        {
            try
            {
                var roleData = await _userCollection.Find(x => x.Phone == idUser).FirstOrDefaultAsync();
                var items = new ChatModel
                {
                    Id = Guid.NewGuid().ToString(),
                    IdOrder = dto.IdOrder,
                    IdUser = idUser,
                    Name = roleData.FullName,
                    Sender = "User",
                    CreatedAt = DateTime.UtcNow,
                    Message = dto.Message,
                    Image = dto.Image,
                };
                await _ChatCollection.InsertOneAsync(items);
                return new { code = 200, data = "Berhasil" };
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Internal Server Error", ex.Message);
            }
        }

        public async Task<object> GetChatWAAsync(GetChatDto dto)
        {
            try
            {
                var items = await _ChatCollection
                    .Find(x => x.IdOrder == dto.IdOrder)
                    .SortBy(x => x.CreatedAt) // Mengurutkan dari yang paling lama ke terbaru
                    .ToListAsync();
                if (items == null)
                {
                    return new { code = 400, data = "Data not found" };
                }
                return new { code = 200, data = items };
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Internal Server Error", ex.Message);
            }
        }

        // public async Task<object> SendNotif(PayloadNotifSend item)
        // {
        //     try
        //     {
        //         string response = await FirebaseService.SendPushNotification2(item.Image,item.FCM, item.Title, item.Body, item.IdOrder);
        //         return new { code = 200, Message = "Notification sent successfully", Response = response };
        //     }
        //     catch (CustomException ex)
        //     {
        //         throw;
        //     }
        // }

    }
}
