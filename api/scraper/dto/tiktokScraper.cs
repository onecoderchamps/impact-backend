public class TiktokVideo
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string TextLanguage { get; set; }
    public long CreateTime { get; set; }
    public string CreateTimeISO { get; set; }
    public bool IsAd { get; set; }
    public AuthorMeta AuthorMeta { get; set; }
    public MusicMeta MusicMeta { get; set; }
    public string WebVideoUrl { get; set; }
    public List<string> MediaUrls { get; set; }
    public VideoMeta VideoMeta { get; set; }
    public int DiggCount { get; set; }
    public int ShareCount { get; set; }
    public int PlayCount { get; set; }
    public int CollectCount { get; set; }
    public int CommentCount { get; set; }
    public List<object> Mentions { get; set; }
    public List<object> DetailedMentions { get; set; }
    public List<Hashtag> Hashtags { get; set; }
    public List<object> EffectStickers { get; set; }
    public bool IsSlideshow { get; set; }
    public bool IsPinned { get; set; }
    public bool IsSponsored { get; set; }
    public string Input { get; set; }
    public string FromProfileSection { get; set; }
}

public class AuthorMeta
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ProfileUrl { get; set; }
    public string NickName { get; set; }
    public bool Verified { get; set; }
    public string Signature { get; set; }
    public string BioLink { get; set; }
    public string OriginalAvatarUrl { get; set; }
    public string Avatar { get; set; }
    public CommerceUserInfo CommerceUserInfo { get; set; }
    public bool PrivateAccount { get; set; }
    public string Region { get; set; }
    public string RoomId { get; set; }
    public bool TtSeller { get; set; }
    public int Following { get; set; }
    public int Friends { get; set; }
    public int Fans { get; set; }
    public long Heart { get; set; }
    public int Video { get; set; }
    public int Digg { get; set; }
}

public class CommerceUserInfo
{
    public bool CommerceUser { get; set; }
}

public class MusicMeta
{
    public string MusicName { get; set; }
    public string MusicAuthor { get; set; }
    public bool MusicOriginal { get; set; }
    public string PlayUrl { get; set; }
    public string CoverMediumUrl { get; set; }
    public string OriginalCoverMediumUrl { get; set; }
    public string MusicId { get; set; }
}

public class VideoMeta
{
    public int Height { get; set; }
    public int Width { get; set; }
    public int Duration { get; set; }
    public string CoverUrl { get; set; }
    public string OriginalCoverUrl { get; set; }
    public string Definition { get; set; }
    public string Format { get; set; }
    public List<SubtitleLink> SubtitleLinks { get; set; }
}

public class SubtitleLink
{
    public string Language { get; set; }
    public string DownloadLink { get; set; }
    public string TiktokLink { get; set; }
    public string Source { get; set; }
    public string SourceUnabbreviated { get; set; }
    public string Version { get; set; }
}

public class Hashtag
{
    public string Name { get; set; }
}
