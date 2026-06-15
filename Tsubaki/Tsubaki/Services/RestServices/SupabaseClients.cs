namespace Tsubaki.Services.RestServices;

public class SupabaseClients
{
    readonly string BaseURL = $"{EnvStorage.SupabaseBaseURL}";
    readonly HttpClient _httpClient;

    public SupabaseClients()
    {
        _httpClient = new HttpClient();
        _ = Init();
    }

    public async Task Init()
        => _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + await Storage.SecureStorage.GetSupabasePAT());

    public async Task<List<SupabaseDtos>> GetAllProjects()
    {
        var response = await _httpClient.GetAsync(BaseURL);
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine($"GetAll: Projects: Supabase: error: {response.StatusCode}");
            return [];
        }

        return await response.Content.ReadFromJsonAsync<List<SupabaseDtos>>() ?? [];
    }

}
