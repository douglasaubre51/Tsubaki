namespace Tsubaki.ViewModels;

public partial class SettingsViewModel : BaseViewModel
{
    [ObservableProperty]
    bool isPageLoading;

    [ObservableProperty]
    string renderKeyField = string.Empty;
    [ObservableProperty]
    string supabaseKeyField1 = string.Empty;
    [ObservableProperty]
    string supabaseKeyField2 = string.Empty;

    [ObservableProperty]
    bool renderKeyFieldEnabled;
    [ObservableProperty]
    bool supabaseKeyField1Enabled;
    [ObservableProperty]
    bool supabaseKeyField2Enabled;

    [ObservableProperty]
    bool isDirty;


    async partial void OnIsPageLoadingChanged(bool value)
    {
        if (value is false) return;

        try
        {
            // Retrieve all keys!
            RenderKeyField = await Storage.SecureStorage.GetRenderKey();
            SupabaseKeyField1 = await Storage.SecureStorage.GetSupabasePAT();
            SupabaseKeyField2 = await Storage.SecureStorage.GetSupabasePAT2();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Settings page load error: " + ex.Message);
        }
        finally
        {
            IsDirty = false;
            isPageLoading = false;
        }
    }

    [RelayCommand]
    async Task EnableRenderKeyField()
    {
        RenderKeyFieldEnabled = !RenderKeyFieldEnabled;
    }
    [RelayCommand]
    async Task EnableSupabaseKeyField1()
    {
        SupabaseKeyField1Enabled = !SupabaseKeyField1Enabled;
    }
    [RelayCommand]
    async Task EnableSupabaseKeyField2()
    {
        SupabaseKeyField2Enabled = !SupabaseKeyField2Enabled;
    }

    [RelayCommand]
    async Task SaveAllFields()
    {
        try
        {
            await Storage.SecureStorage.SetRenderKey(RenderKeyField);
            await Storage.SecureStorage.SetSupabasePAT(SupabaseKeyField1);
            await Storage.SecureStorage.SetSupabasePAT2(SupabaseKeyField2);

            IsDirty = false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("SettingsView: Auth Keys Save Changes error: " + ex.Message);
        }
    }
}
