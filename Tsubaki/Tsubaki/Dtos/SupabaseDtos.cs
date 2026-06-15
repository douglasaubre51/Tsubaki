using System.Text.Json.Serialization;

namespace Tsubaki.Dtos;

public class SupabaseDtos
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("ref")]
    public string Ref { get; set; } = string.Empty;

    [JsonPropertyName("organization_id")]
    public string OrganizationId { get; set; } = string.Empty;

    [JsonPropertyName("organization_slug")]
    public string OrganizationSlug { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("database")]
    public Database? Database { get; set; }
}

public class Database
{
    [JsonPropertyName("host")]
    public string Host { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("postgres_engine")]
    public string PostgresEngine { get; set; } = string.Empty;

    [JsonPropertyName("release_channel")]
    public string ReleaseChannel { get; set; } = string.Empty;
}
