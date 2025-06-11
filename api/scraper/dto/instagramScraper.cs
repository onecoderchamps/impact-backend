public class InstagramProfile
{
    public string? FullName { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? Username { get; set; }
    public int PostsCount { get; set; }
    public int FollowersCount { get; set; }
    public int FollowsCount { get; set; }
    public bool Private { get; set; }
    public bool Verified { get; set; }
    public bool IsBusinessAccount { get; set; }
    public string? Biography { get; set; }
}
