using MongoDB.Driver;
using impact.Shared.Models;
using Newtonsoft.Json;

public class TikTokUserResponse
{
    [JsonProperty("data")]
    public TikTokUserData Data { get; set; }

    [JsonProperty("error")]
    public TikTokError Error { get; set; }
}

public class TikTokUserData
{
    [JsonProperty("user")]
    public TikTokUser User { get; set; }
}

public class TikTokUser
{
    [JsonProperty("open_id")]
    public string OpenId { get; set; }

    [JsonProperty("union_id")]
    public string UnionId { get; set; }

    [JsonProperty("avatar_url")]
    public string AvatarUrl { get; set; }

    [JsonProperty("display_name")]
    public string DisplayName { get; set; }

    [JsonProperty("bio_description")]
    public string BioDescription { get; set; }

    [JsonProperty("profile_deep_link")]
    public string ProfileDeepLink { get; set; }

    [JsonProperty("is_verified")]
    public bool IsVerified { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("follower_count")]
    public int FollowerCount { get; set; }

    [JsonProperty("following_count")]
    public int FollowingCount { get; set; }

    [JsonProperty("likes_count")]
    public int LikesCount { get; set; }

    [JsonProperty("video_count")]
    public int VideoCount { get; set; }
}

public class TikTokError
{
    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("log_id")]
    public string LogId { get; set; }
}
