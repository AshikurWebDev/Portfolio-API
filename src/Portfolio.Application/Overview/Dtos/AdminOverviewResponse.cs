namespace Portfolio.Application.Overview.Dtos;

/// <summary>Counts for the admin dashboard's home page.</summary>
/// <param name="PublishedPosts">Posts live on the blog.</param>
/// <param name="DraftPosts">Posts not yet published.</param>
/// <param name="DesignItems">Design gallery items (visible and hidden).</param>
/// <param name="UnreadMessages">Contact messages not yet marked read.</param>
/// <param name="SkillGroups">Cards in the Skills section.</param>
/// <param name="Skills">Skills across all cards.</param>
/// <param name="HasCv">True when a CV PDF is uploaded.</param>
/// <param name="CvUploadedAt">When the CV was uploaded, if there is one.</param>
public sealed record AdminOverviewResponse(
    int PublishedPosts,
    int DraftPosts,
    int DesignItems,
    int UnreadMessages,
    int SkillGroups,
    int Skills,
    bool HasCv,
    DateTimeOffset? CvUploadedAt);
