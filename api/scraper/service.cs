using MongoDB.Driver;
using impact.Shared.Models;
using Newtonsoft.Json;

namespace RepositoryPattern.Services.ScraperService
{
    public class ScraperService : IScraperService
    {
        private readonly IMongoCollection<Scraper> _scraperCollection;
        private readonly IMongoCollection<Setting> _settingCollection;
        private readonly IMongoCollection<User> _userCollection;

        private readonly IHttpClientFactory _httpClientFactory;

        public ScraperService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("ConnectionURI"));
            var database = client.GetDatabase("impact");

            _scraperCollection = database.GetCollection<Scraper>("Scraper");
            _userCollection = database.GetCollection<User>("User");

            _settingCollection = database.GetCollection<Setting>("Setting");
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Object> GetById(string id)
        {
            try
            {
                var items = await _scraperCollection.Find(_ => _.IdUser == id).ToListAsync();
                var user = await _userCollection.Find(_ => _.Id == id).FirstOrDefaultAsync();
                return new { code = 200,user = user , data = items, message = "Data Add Complete" };
            }
            catch (CustomException)
            {
                throw;
            }
        }

        public async Task<object> scraperTiktok(string idUser)
        {
            try
            {
                var roleData = await _userCollection.Find(x => x.Id == idUser).FirstOrDefaultAsync()
                    ?? throw new CustomException(400, "Error", "Account not found");

                if (string.IsNullOrEmpty(roleData.TikTokAccessToken))
                    throw new CustomException(401, "Unauthorized", "TikTok access token not found");

                var client = _httpClientFactory.CreateClient();

                // ======================
                // 1. Ambil User Info
                // ======================
                var userUrl = "https://open.tiktokapis.com/v2/user/info/?" +
                            "fields=open_id,union_id,avatar_url,display_name,bio_description,profile_deep_link,is_verified,username,follower_count,following_count,likes_count,video_count";

                var userRequest = new HttpRequestMessage(HttpMethod.Get, userUrl);
                userRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", roleData.TikTokAccessToken);

                var userResponse = await client.SendAsync(userRequest);
                if (!userResponse.IsSuccessStatusCode)
                {
                    var error = await userResponse.Content.ReadAsStringAsync();
                    throw new CustomException((int)userResponse.StatusCode, "TikTok API Error (User Info)", error);
                }

                var userResult = await userResponse.Content.ReadAsStringAsync();
                var userData = JsonConvert.DeserializeObject<TikTokUserResponse>(userResult)?.Data?.User;

                if (userData == null)
                    throw new CustomException(500, "Error", "Failed to parse TikTok user data");


                // ======================
                // 2. Ambil Video List
                // ======================
                var videoUrl = "https://open.tiktokapis.com/v2/video/list/?" +
                            "fields=cover_image_url,id,title,video_description,duration,embed_link,like_count,comment_count,share_count,view_count";

                var videoRequest = new HttpRequestMessage(HttpMethod.Post, videoUrl);
                videoRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", roleData.TikTokAccessToken);
                videoRequest.Content = new StringContent(JsonConvert.SerializeObject(new { max_count = 5 }));

                var videoResponse = await client.SendAsync(videoRequest);
                if (!videoResponse.IsSuccessStatusCode)
                {
                    var error = await videoResponse.Content.ReadAsStringAsync();
                    throw new CustomException((int)videoResponse.StatusCode, "TikTok API Error (Video List)", error);
                }

                var videoResult = await videoResponse.Content.ReadAsStringAsync();
                var videoData = JsonConvert.DeserializeObject<TikTokVideoListResponse>(videoResult)?.Data?.Videos ?? new List<TikTokVideo>();


                // ======================
                // 3. Simpan ke MongoDB
                // ======================
                var cekStatus = await _scraperCollection.Find(_ => _.IdUser == idUser && _.Type == "TikTok").FirstOrDefaultAsync();
                if (cekStatus != null)
                {
                    cekStatus.Tiktok = userData;
                    cekStatus.Video = videoData;
                    cekStatus.UpdatedAt = DateTime.Now;

                    await _scraperCollection.ReplaceOneAsync(_ => _.IdUser == idUser && _.Type == "TikTok", cekStatus);

                    return new { code = 200, data = new { user = userData, videos = videoData } };
                }
                else
                {
                    var scraperData = new Scraper
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "TikTok",
                        Tiktok = userData,
                        Video = videoData,
                        IdUser = idUser,
                        IsActive = true,
                        IsVerification = false,
                        CreatedAt = DateTime.Now
                    };
                    await _scraperCollection.InsertOneAsync(scraperData);

                    return new { code = 200, data = new { user = userData, videos = videoData } };
                }
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Error", ex.Message);
            }
        }




        public async Task<object> scraperInstagram(InstagramProfileRequest item, string idUser)
        {
            try
            {
                var authConfig = await _settingCollection
                    .Find(d => d.Key == "apifyKey")
                    .FirstOrDefaultAsync();

                if (authConfig == null || string.IsNullOrEmpty(authConfig.Value))
                    throw new CustomException(400, "API Key", "Apify API key not found in settings");

                var payload = new
                {
                    usernames = new List<string> { item.Username }
                };


                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.apify.com/v2/acts/apify~instagram-profile-scraper/run-sync-get-dataset-items?token={authConfig.Value}";

                var response = await client.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new CustomException((int)response.StatusCode, "Scraper Error", error);
                }

                var result = await response.Content.ReadAsStringAsync();
                var videos = JsonConvert.DeserializeObject<List<InstagramProfile>>(result);
                var firstVideo = videos?.FirstOrDefault();

                var cekStatus = await _scraperCollection.Find(_ => _.IdUser == idUser && _.Type == "Instagram").FirstOrDefaultAsync();
                if (cekStatus != null)
                {
                    cekStatus.Instagram = firstVideo;
                    cekStatus.UpdatedAt = DateTime.Now;
                    await _scraperCollection.ReplaceOneAsync(_ => _.IdUser == idUser && _.Type == "Instagram", cekStatus);

                    return new { code = 200, data = firstVideo };
                }
                else
                {
                    var scraperData = new Scraper
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "Instagram",
                        Instagram = firstVideo,
                        IdUser = idUser,
                        IsActive = true,
                        IsVerification = false,
                        CreatedAt = DateTime.Now
                    };
                    await _scraperCollection.InsertOneAsync(scraperData);

                    return new { code = 200, data = firstVideo };
                }
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Error", ex.Message);
            }
        }

        public async Task<object> scraperYoutube(YoutubeProfileRequest item, string idUser)
        {
            try
            {
                var authConfig = await _settingCollection
                    .Find(d => d.Key == "apifyKey")
                    .FirstOrDefaultAsync();

                if (authConfig == null || string.IsNullOrEmpty(authConfig.Value))
                    throw new CustomException(400, "API Key", "Apify API key not found in settings");
                var payload = new
                {
                    maxResultStreams = 0,
                    maxResults = 1,
                    maxResultsShorts = 1,
                    startUrls = new[]
                    {
                        new
                        {
                            url = "https://www.youtube.com/@" + item.Username,
                            method = "GET"
                        }
                    }
                };


                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.apify.com/v2/acts/streamers~youtube-channel-scraper/run-sync-get-dataset-items?token={authConfig.Value}";

                var response = await client.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new CustomException((int)response.StatusCode, "Scraper Error", error);
                }

                var result = await response.Content.ReadAsStringAsync();
                var videos = JsonConvert.DeserializeObject<List<YoutubeProfile>>(result);
                var firstVideo = videos?.FirstOrDefault();

                var cekStatus = await _scraperCollection.Find(_ => _.IdUser == idUser && _.Type == "Youtube").FirstOrDefaultAsync();
                if (cekStatus != null)
                {
                    cekStatus.Youtube = firstVideo;
                    cekStatus.UpdatedAt = DateTime.Now;
                    await _scraperCollection.ReplaceOneAsync(_ => _.IdUser == idUser && _.Type == "Youtube", cekStatus);

                    return new { code = 200, data = firstVideo };
                }
                else
                {
                    var scraperData = new Scraper
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "Youtube",
                        Youtube = firstVideo,
                        IdUser = idUser,
                        IsActive = true,
                        IsVerification = false,
                        CreatedAt = DateTime.Now
                    };
                    await _scraperCollection.InsertOneAsync(scraperData);

                    return new { code = 200, data = firstVideo };
                }
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Error", ex.Message);
            }
        }

        public async Task<object> scraperLinkin(LinkinProfileRequest item, string idUser)
        {
            try
            {
                var authConfig = await _settingCollection
                    .Find(d => d.Key == "apifyKey")
                    .FirstOrDefaultAsync();

                if (authConfig == null || string.IsNullOrEmpty(authConfig.Value))
                    throw new CustomException(400, "API Key", "Apify API key not found in settings");
                var payload = new
                {
                    ProfileUrls = new List<string>
                    {
                        item.Username
                    }
                };


                var client = _httpClientFactory.CreateClient();
                var url = $"https://api.apify.com/v2/acts/dev_fusion~linkedin-profile-scraper/run-sync-get-dataset-items?token={authConfig.Value}";

                var response = await client.PostAsJsonAsync(url, payload);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new CustomException((int)response.StatusCode, "Scraper Error", error);
                }

                var result = await response.Content.ReadAsStringAsync();
                var videos = JsonConvert.DeserializeObject<List<LinkedInProfile>>(result);
                var firstVideo = videos?.FirstOrDefault();

                var cekStatus = await _scraperCollection.Find(_ => _.IdUser == idUser && _.Type == "Linkedin").FirstOrDefaultAsync();
                if (cekStatus != null)
                {
                    cekStatus.Linkedin = firstVideo;
                    cekStatus.UpdatedAt = DateTime.Now;
                    await _scraperCollection.ReplaceOneAsync(_ => _.IdUser == idUser && _.Type == "Linkedin", cekStatus);

                    return new { code = 200, data = firstVideo };
                }
                else
                {
                    var scraperData = new Scraper
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "Linkedin",
                        Linkedin = firstVideo,
                        IdUser = idUser,
                        IsActive = true,
                        IsVerification = false,
                        CreatedAt = DateTime.Now
                    };
                    await _scraperCollection.InsertOneAsync(scraperData);

                    return new { code = 200, data = firstVideo };
                }
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new CustomException(500, "Error", ex.Message);
            }
        }
    }
}
