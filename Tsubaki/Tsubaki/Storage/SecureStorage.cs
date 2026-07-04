namespace Tsubaki.Storage;

public static class SecureStorage
{
    public async static Task<string> GetRenderKey()
        => await Microsoft.Maui.Storage.SecureStorage.Default.GetAsync("RenderKey") ?? string.Empty;
    public async static Task<string> GetSupabasePAT()
        => await Microsoft.Maui.Storage.SecureStorage.Default.GetAsync("SupabaseKey") ?? string.Empty;
    public async static Task<string> GetSupabasePAT2()
        => await Microsoft.Maui.Storage.SecureStorage.Default.GetAsync("SupabaseKey2") ?? string.Empty;

    public async static Task SetRenderKey(string key)
        => await Microsoft.Maui.Storage.SecureStorage.Default.SetAsync("RenderKey", key);
    public async static Task SetSupabasePAT(string key)
        => await Microsoft.Maui.Storage.SecureStorage.Default.SetAsync("SupabaseKey", key);
    public async static Task SetSupabasePAT2(string key)
        => await Microsoft.Maui.Storage.SecureStorage.Default.SetAsync("SupabaseKey2", key);


    public static bool IsEnvKeysInitialized()
        => Preferences.Default.Get<bool>("IsEnvKeysInitialized", false);

    public static void SetIsEnvKeysInitialized(bool value)
        => Preferences.Default.Set<bool>("IsEnvKeysInitialized", value);


    public async static Task<string> GetProductKey()
        => await Microsoft.Maui.Storage.SecureStorage.Default.GetAsync("ProductKey") ?? String.Empty;
    public async static Task StoreProductKey()
        => await Microsoft.Maui.Storage.SecureStorage.Default.SetAsync("ProductKey", Guid.NewGuid().ToString());

    public static string GetProjectKey()
        => "ed7c10db-1bb0-4aa3-a8ec-5e992bee0941";


    public async static Task SetENVKeys(string key, string value)
        => await Microsoft.Maui.Storage.SecureStorage.Default.SetAsync(key, value);
}
