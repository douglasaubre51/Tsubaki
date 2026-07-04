namespace Tsubaki.Services.RestServices;

public class RenderClients
{
    HttpClient _client;
    public RenderClients()
    {
        _client = new HttpClient();
        string renderKey = Storage.SecureStorage.GetRenderKey().Result;
        _client.DefaultRequestHeaders.Add("authorization", "Bearer " + renderKey);
    }

    public async Task<bool> KeepAlive(string serviceUrl)
    {
        Debug.WriteLine("Sending PING to: " + serviceUrl);
        var response = await _client.GetAsync($"{serviceUrl}/swagger");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("Keep alive error code: " + response.StatusCode);
            return false;
        }

        Debug.WriteLine("Service OK!");
        return true;
    }

    public async Task RestartServiceById(string serviceId)
    {
        var response = await _client.PostAsync($"{EnvStorage.RenderBaseURL}/services/{serviceId}/restart", null);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine("starting service error: " + response.StatusCode);

        Debug.WriteLine("resuming service: " + serviceId);
    }
    public async Task ResumeServiceById(string serviceId)
    {
        var response = await _client.PostAsync($"{EnvStorage.RenderBaseURL}/services/{serviceId}/resume", null);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine("resuming service error: " + response.StatusCode);

        Debug.WriteLine("starting service: " + serviceId);
    }
    public async Task SuspendServiceById(string serviceId)
    {
        var response = await _client.PostAsync($"{EnvStorage.RenderBaseURL}/services/{serviceId}/suspend", null);
        if (response.IsSuccessStatusCode is false)
            Debug.WriteLine("suspending service error: " + response.StatusCode);

        Debug.WriteLine("suspending service: " + serviceId);
    }

    // paginated client
    public async Task<List<DeployDtos>?> GetDeploysFromCursorId(string serviceId, string cursorId)
    {
        var response = await _client.GetAsync($"{EnvStorage.RenderBaseURL}/services/{serviceId}/deploys?cursor={cursorId}&limit=15");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("GetDeploysFromCursorId error: " + response.StatusCode);
            return null!;
        }

        return await response.Content.ReadFromJsonAsync<List<DeployDtos>>();
    }

    public async Task<List<DeployDtos>?> GetAllDeploys(string serviceId)
    {
        var response = await _client.GetAsync($"{EnvStorage.RenderBaseURL}/services/{serviceId}/deploys?limit=8");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("GetAllDeploys error: " + response.StatusCode);
            return null!;
        }

        return await response.Content.ReadFromJsonAsync<List<DeployDtos>>();
    }
    public async Task<List<RenderDtos>?> GetAllServices()
    {
        Debug.WriteLine("fetching all web services!");
        var response = await _client.GetAsync($"{EnvStorage.RenderBaseURL}/services");
        if (response.IsSuccessStatusCode is false)
        {
            Debug.WriteLine("GetAllServices error: " + response.StatusCode);
            return null!;
        }

        return await response.Content.ReadFromJsonAsync<List<RenderDtos>>();
    }
}
