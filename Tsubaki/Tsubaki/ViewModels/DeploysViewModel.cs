namespace Tsubaki.ViewModels;

[QueryProperty(nameof(CurrentService), "SelectedService")]
public partial class DeploysViewModel(RenderClients renderClient) : Tsubaki.ViewModels.BaseViewModel
{
    [ObservableProperty]
    private Service? currentService;

    [ObservableProperty]
    private bool isCollectionRefreshing;

    [ObservableProperty]
    private bool didInfiniteScrollRequested;

    private readonly RenderClients _renderClients = renderClient;

    [ObservableProperty]
    private ObservableRangeCollection<DeployDtos> deployCardCollection = [];

    [ObservableProperty]
    private int keepAliveRetryCounter = 0;

    private CancellationTokenSource? _keepAliveCtsSource = null;

    private ServiceStateModel? _serviceStateModel = null;

    [ObservableProperty]
    private string keepAliveStatusMessage = "In active";
    [ObservableProperty]
    private bool isPageExiting;
    [ObservableProperty]
    private bool isPageLoading;

    [RelayCommand]
    async Task StopKeepAlive()
    {
        _keepAliveCtsSource?.Cancel();
        Debug.WriteLine("Stopping keepalive!");
        KeepAliveStatusMessage = "InActive";
        KeepAliveRetryCounter = 0;
    }

    [RelayCommand]
    async Task TriggerKeepAlive()
    {
        _keepAliveCtsSource = new CancellationTokenSource();
        CancellationToken keepAliveCts = _keepAliveCtsSource.Token;

        try
        {
            while (keepAliveCts.IsCancellationRequested is false && KeepAliveRetryCounter < 3)
            {
                KeepAliveStatusMessage = "IsActive";
                bool result = await _renderClients.KeepAlive(CurrentService!.ServiceDetails!.Url);
                if (result is false)
                {
                    Debug.WriteLine("Service is down!");
                    KeepAliveRetryCounter += 1;
                    await Task.Delay(1000, keepAliveCts);

                    continue;
                }

                await Task.Delay(300000, keepAliveCts);
                //await Task.Delay(1000, keepAliveCts);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Keep Alive error: " + ex.Message);
        }
        finally
        {
            KeepAliveRetryCounter = 0;
            KeepAliveStatusMessage = "InActive";
            Debug.WriteLine("Automatically stopping keepalive!");
        }
    }

    [RelayCommand]
    async Task LaunchService()
    {
        try
        {
            Console.WriteLine("swiper no swiping!!!");
            Console.WriteLine(CurrentService.ServiceDetails.Url);
            await Browser.Default.OpenAsync(new Uri(CurrentService!.ServiceDetails!.Url), BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [RelayCommand]
    async Task GetMoreDeploys()
    {
        if (IsCollectionRefreshing is true) return;
        if (DidInfiniteScrollRequested is true) return;
        try
        {
            DidInfiniteScrollRequested = true;
            Console.WriteLine("Infinite scroll requested!");

            List<DeployDtos>? deployDtos = await _renderClients.GetDeploysFromCursorId(
                CurrentService!.Id,
                DeployCardCollection.Last().Cursor);

            if (deployDtos is null || deployDtos.Count == 0) return;

            foreach (var deploy in deployDtos)
            {
                if (deploy.Deploy!.Status == "canceled")
                    deploy.IsCanceled = true;

                if (deploy.Deploy.Status == "deactivated")
                    deploy.IsDeactivated = true;

                if (deploy.Deploy.Status == "build_failed")
                    deploy.DidBuildFail = true;
            }

            DeployCardCollection.AddRange(deployDtos);
            Console.WriteLine($"Deploy card collection: {DeployCardCollection.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            DidInfiniteScrollRequested = false;
        }
    }

    [RelayCommand]
    async Task RefreshDeployCardCollection()
    {
        try
        {
            IsCollectionRefreshing = true;
            List<DeployDtos>? deployDtos = [];

            deployDtos = await _renderClients.GetAllDeploys(CurrentService!.Id);

            Console.WriteLine($"refresh data count: {deployDtos!.Count}");
            Console.WriteLine($"Deploy card collection: {DeployCardCollection.Count}");

            if (deployDtos is null || deployDtos.Count == 0) return;

            foreach (var deploy in deployDtos)
            {
                if (deploy.Deploy!.Status == "canceled")
                    deploy.IsCanceled = true;

                if (deploy.Deploy.Status == "deactivated")
                    deploy.IsDeactivated = true;

                if (deploy.Deploy.Status == "build_failed")
                    deploy.DidBuildFail = true;
            }

            DeployCardCollection.Clear();
            DeployCardCollection.AddRange(deployDtos);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsCollectionRefreshing = false;
        }
    }

    async partial void OnIsPageExitingChanged(bool oldValue, bool newValue)
    {
        if (newValue is false) return;

        try
        {
            if (_serviceStateModel is not null)
            {
                _serviceStateModel.CtsSource = _keepAliveCtsSource;
                _serviceStateModel.RetryCount = KeepAliveRetryCounter;
                _serviceStateModel.IsRunning = KeepAliveStatusMessage.Equals("IsActive") ? true : false;
                return;
            }

            ServiceStateStore.Services.Add(
                CurrentService!.Id,
                new ServiceStateModel
                {
                    ServiceId = CurrentService.Id,
                    RetryCount = KeepAliveRetryCounter,
                    CtsSource = _keepAliveCtsSource,
                    IsRunning = KeepAliveStatusMessage.Equals("IsActive") ? true : false
                });

        }
        catch (Exception ex)
        {
            Debug.WriteLine("Deploys page exiting!" + ex.Message);
        }
        finally
        {
            IsPageExiting = false;
        }
    }
    async partial void OnIsPageLoadingChanged(bool value)
    {
        if (value is false) return;
        try
        {
            DeployCardCollection.Clear();
            IsCollectionRefreshing = true;

            bool doesServiceExist = ServiceStateStore.Services.ContainsKey(CurrentService!.Id);
            if (doesServiceExist is true)
            {
                ServiceStateStore.Services.TryGetValue(CurrentService.Id, out _serviceStateModel);
                if (_serviceStateModel is null) return;

                KeepAliveRetryCounter = _serviceStateModel.RetryCount;
                _keepAliveCtsSource = _serviceStateModel.CtsSource;
                if (_serviceStateModel.IsRunning is true)
                    KeepAliveStatusMessage = "IsActive";
            }

            List<DeployDtos>? deployDtos = await _renderClients.GetAllDeploys(CurrentService!.Id);
            if (deployDtos is null || deployDtos.Count == 0) return;

            foreach (var deploy in deployDtos)
            {
                if (deploy.Deploy!.Status == "canceled")
                    deploy.IsCanceled = true;

                if (deploy.Deploy.Status == "deactivated")
                    deploy.IsDeactivated = true;

                if (deploy.Deploy.Status == "build_failed")
                    deploy.DidBuildFail = true;
            }

            DeployCardCollection.AddRange(deployDtos);
            Console.WriteLine($"Deploy card collection: {DeployCardCollection.Count}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsCollectionRefreshing = false;
            IsPageLoading = false;
        }
    }
}
