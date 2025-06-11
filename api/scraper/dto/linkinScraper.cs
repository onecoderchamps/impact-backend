public class LinkedInProfile
{
    public string LinkedinUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Headline { get; set; }
    public int Connections { get; set; }
    public int Followers { get; set; }
    public string Email { get; set; }
    public string MobileNumber { get; set; }
    public string JobTitle { get; set; }
    public string CompanyName { get; set; }
    public string CompanyIndustry { get; set; }
    public string CompanyWebsite { get; set; }
    public string CompanyLinkedin { get; set; }
    public string CompanyFoundedIn { get; set; }
    public string CompanySize { get; set; }
    public string CurrentJobDuration { get; set; }
    public double CurrentJobDurationInYrs { get; set; }
    public string TopSkillsByEndorsements { get; set; }
    public string AddressCountryOnly { get; set; }
    public string AddressWithCountry { get; set; }
    public string AddressWithoutCountry { get; set; }
    public string ProfilePic { get; set; }
    public string ProfilePicHighQuality { get; set; }
    public string About { get; set; }
    public string PublicIdentifier { get; set; }
    public string OpenConnection { get; set; }
    public string Urn { get; set; }

    public List<Experience> Experiences { get; set; }
    public List<Update> Updates { get; set; }
    public List<Skill> Skills { get; set; }
    public List<ProfilePicDimension> ProfilePicAllDimensions { get; set; }
    public List<Education> Educations { get; set; }
    public List<InterestSection> Interests { get; set; }
    public List<object> LicenseAndCertificates { get; set; }
    public List<object> HonorsAndAwards { get; set; }
    public List<object> Languages { get; set; }
    public List<object> VolunteerAndAwards { get; set; }
    public List<object> Verifications { get; set; }
    public List<object> Promos { get; set; }
    public List<object> Highlights { get; set; }
    public List<object> Projects { get; set; }
    public List<object> Publications { get; set; }
    public List<object> Patents { get; set; }
    public List<object> Courses { get; set; }
    public List<object> TestScores { get; set; }
    public List<object> Organizations { get; set; }
    public List<object> VolunteerCauses { get; set; }
    public List<object> Recommendations { get; set; }
}

public class Experience
{
    public string CompanyId { get; set; }
    public string CompanyUrn { get; set; }
    public string CompanyLink1 { get; set; }
    public string Logo { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Caption { get; set; }
    public string Metadata { get; set; }
    public bool Breakdown { get; set; }
    public List<SubComponentWrapper> SubComponents { get; set; }
}

public class Update
{
    public string PostText { get; set; }
    public string Image { get; set; }
    public string PostLink { get; set; }
    public int NumLikes { get; set; }
    public int NumComments { get; set; }
    public List<ReactionTypeCount> ReactionTypeCounts { get; set; }
}

public class ReactionTypeCount
{
    public int Count { get; set; }
    public string ReactionType { get; set; }
}

public class Skill
{
    public string Title { get; set; }
    public List<SubComponentWrapper> SubComponents { get; set; }
}

public class SubComponentWrapper
{
    public List<Description> Description { get; set; }
}

public class Description
{
    public string Type { get; set; }
    public string Text { get; set; }
}

public class ProfilePicDimension
{
    public int Width { get; set; }
    public int Height { get; set; }
    public string Url { get; set; }
}

public class Education
{
    public string CompanyId { get; set; }
    public string CompanyUrn { get; set; }
    public string CompanyLink1 { get; set; }
    public string Logo { get; set; }
    public string Title { get; set; }
    public string Caption { get; set; }
    public bool Breakdown { get; set; }
    public List<SubComponentWrapper> SubComponents { get; set; }
}

public class InterestSection
{
    public string SectionName { get; set; }
    public List<InterestComponent> SectionComponents { get; set; }
}

public class InterestComponent
{
    public string TitleV2 { get; set; }
    public string Caption { get; set; }
    public string Subtitle { get; set; }
    public string Size { get; set; }
    public string TextActionTarget { get; set; }
    public List<SubComponentWrapper> SubComponents { get; set; }
}
