namespace Tsubaki.Services.RestServices;

public class MisatoApiService
{
    readonly string BaseURL = $"{EnvStorage.MisatoBaseURL}";

    readonly HttpClient _httpClient;


    public MisatoApiService()
    {
        _httpClient = new HttpClient();
    }


    /// <summary>
    /// Create a new client.
    /// Client will be created based on its activation status.
    /// </summary>
    /// <param name="clientId"></param>
    /// <returns>True if this client is created at the api.</returns>
    public async Task<bool> CreateNewClient(string clientId)
    {
        Location? geo;
        string coordinates = string.Empty;
        Placemark placemark = new Placemark();

#if ANDROID
        geo = await Geolocation.Default.GetLastKnownLocationAsync();
        coordinates = $"lattitude: {geo!.Latitude.ToString()} longitude: {geo.Longitude.ToString()}";

        IEnumerable<Placemark> placemarks = await Geocoding.Default.GetPlacemarksAsync(geo.Latitude, geo.Longitude);
        placemark = placemarks.FirstOrDefault()!;
#endif

        string time = DateTime.UtcNow.ToString();

        var response = await _httpClient.PostAsJsonAsync<MisatoApiDtos>(
            $"{BaseURL}/new_client",
            new MisatoApiDtos
            {
                ClientId = clientId,
                ProjectId = Storage.SecureStorage.GetProjectKey(),
                BootTime = time,
                Location = placemark.Locality ?? string.Empty,
                Coordinates = coordinates
            });
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("CreateNewClient: MisatoApiService: error: " + response.StatusCode);
            return false;
        }

        return true;
    }

    /// <summary>
    ///  Check if the client is already created at the API.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="clientId"></param>
    /// <returns>FAILURE on error response, NOT_CREATED if the client doesnt exist and CREATED if the client already exists.</returns>
    public async Task<ClientActivationStatusEnum> CheckClientActivationStatus(string projectId, string clientId)
    {
        var response = await _httpClient.GetAsync($"{BaseURL}/{projectId}/{clientId}/check_activation_status");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("CheckClientActivationStatus: MisatoApiService: error: " + response.StatusCode);
            return ClientActivationStatusEnum.FAILURE;
        }

        ClientResponseDto result = await response.Content.ReadFromJsonAsync<ClientResponseDto>();
        if (result!.IsClientCreated is false) return ClientActivationStatusEnum.NOT_CREATED;

        return ClientActivationStatusEnum.CREATED;
    }

    /// <summary>
    /// Gets all the environment keys as long as the client is allowed.
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="clientId"></param>
    /// <returns></returns>
    public async Task<List<AttributeResponseDto>> FetchENVKeys(string projectId, string clientId)
    {
        var response = await _httpClient.GetAsync($"{BaseURL}/{projectId}/{clientId}/fetch_env_keys");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("FetchENVKeys: MisatoApiService: error: " + response.StatusCode);
            return [];
        }

        return await response.Content.ReadFromJsonAsync<List<AttributeResponseDto>>() ?? [];
    }
}
