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

public class TikTokVideoListResponse
{
    [JsonProperty("data")]
    public TikTokVideoData Data { get; set; }

    [JsonProperty("error")]
    public TikTokError Error { get; set; }
}

public class TikTokVideoData
{
    [JsonProperty("cursor")]
    public long Cursor { get; set; }

    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("videos")]
    public List<TikTokVideo> Videos { get; set; }
}

public class TikTokVideo
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("video_description")]
    public string VideoDescription { get; set; }

    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("embed_link")]
    public string EmbedLink { get; set; }

    [JsonProperty("cover_image_url")]
    public string CoverImageUrl { get; set; }

    [JsonProperty("like_count")]
    public int LikeCount { get; set; }

    [JsonProperty("comment_count")]
    public int CommentCount { get; set; }

    [JsonProperty("share_count")]
    public int ShareCount { get; set; }

    [JsonProperty("view_count")]
    public int ViewCount { get; set; }
}
