namespace Tsubaki.Models.SupabaseModels;

public class SupabaseProjectModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public string OrganizationId { get; set; } = string.Empty;
    public string OrganizationSlug { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    public string DBHost { get; set; } = string.Empty;

}
