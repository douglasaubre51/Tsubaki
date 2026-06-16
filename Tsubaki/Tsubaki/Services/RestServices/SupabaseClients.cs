namespace Tsubaki.Services.RestServices;

public class SupabaseClients
{
    readonly string BaseURL = $"{EnvStorage.SupabaseBaseURL}";
    readonly HttpClient _httpClient;

    public SupabaseClients()
        => _httpClient = new HttpClient();

    public async Task Init()
        => _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + await Storage.SecureStorage.GetSupabasePAT());
    public async Task Init2()
        => _httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + await Storage.SecureStorage.GetSupabasePAT2());

    public void ClearHeader() => _httpClient.DefaultRequestHeaders.Remove("Authorization");

    public async Task<List<SupabaseDtos>> GetAllProjects()
    {
        await Init();
        var response = await _httpClient.GetAsync(BaseURL);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine($"GetAll: Projects: SupabasePAT: error: {response.StatusCode}");

        List<SupabaseDtos> dtos = await response.Content.ReadFromJsonAsync<List<SupabaseDtos>>() ?? [];

        ClearHeader();
        await Init2();
        response = await _httpClient.GetAsync(BaseURL);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine($"GetAll: Projects: SupabasePAT2: error: {response.StatusCode}");

        dtos.AddRange(await response.Content.ReadFromJsonAsync<List<SupabaseDtos>>());

        ClearHeader();
        return dtos;
    }

    public async Task ResumeProjectById(string projectId)
    {
        await Init();
        var response = await _httpClient.PostAsync($"{BaseURL}/{projectId}/restore", null);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine($"ResumeProjectById: SupabasePAT: error: {response.StatusCode}");

        ClearHeader();
        await Init2();
        response = await _httpClient.PostAsync($"{BaseURL}/{projectId}/restore", null);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine($"ResumeProjectById: SupabasePAT2: error: {response.StatusCode}");

        ClearHeader();
    }
}
