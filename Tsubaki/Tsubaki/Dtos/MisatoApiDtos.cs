using System.Text.Json.Serialization;

namespace Tsubaki.Dtos;

public class MisatoApiDtos
{
    public string ClientId { get; set; } = string.Empty;
    public string ProjectId { get; set; } = string.Empty;
    public string BootTime { get; set; } = string.Empty;
    public string Coordinates { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class ClientResponseDto
{
    [JsonPropertyName("is_client_created")]
    public bool IsClientCreated { get; set; }
}

public class AttributeResponseDto
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public record AttributeResponseWrapper(
    List<AttributeResponseDto> Attributes
);
