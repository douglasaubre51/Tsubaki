namespace Tsubaki;

public partial class MainPage : ContentPage
{
    readonly MisatoApiService _misatoApiServ;


    public MainPage(MainPageViewModel viewModel, MisatoApiService misatoApiServ)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _misatoApiServ = misatoApiServ;
    }


    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var context = BindingContext as MainPageViewModel;

        // Initialize Remote EnvKeys!
        //await Dispatcher.DispatchAsync(async () => await InitEnvKeys());

        await Dispatcher.DispatchAsync(() =>
            context!.IsPageLoading = true
        );
    }

    //async Task InitEnvKeys()
    //{
    //    try
    //    {
    //        if (Storage.SecureStorage.IsEnvKeysInitialized() is true) return;

    //        string productKey = await Storage.SecureStorage.GetProductKey();
    //        if (productKey == string.Empty)
    //        {
    //            await Storage.SecureStorage.StoreProductKey();
    //            productKey = await Storage.SecureStorage.GetProductKey();
    //        }

    //        string projectKey = Storage.SecureStorage.GetProjectKey();

    //        ClientActivationStatusEnum status = await _misatoApiServ.CheckClientActivationStatus(projectKey, productKey);
    //        if (status == ClientActivationStatusEnum.FAILURE) return;

    //        if (status == ClientActivationStatusEnum.NOT_CREATED)
    //            await _misatoApiServ.CreateNewClient(productKey);

    //        List<AttributeResponseDto> attributes = await _misatoApiServ.FetchENVKeys(projectKey, productKey);
    //        if (attributes.Count is 0) return;

    //        foreach (var attribute in attributes)
    //            await Storage.SecureStorage.SetENVKeys(attribute.Key, attribute.Value);

    //        Storage.SecureStorage.SetIsEnvKeysInitialized(true);
    //    }
    //    catch (Exception ex)
    //    {
    //        Debug.WriteLine("Env Keys init error: " + ex.Message);
    //    }
    //}
}
