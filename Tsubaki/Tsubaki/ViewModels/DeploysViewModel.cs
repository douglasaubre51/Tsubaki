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
    [ObservableProperty]
    private int retryCounterHistory = 0;

    private CancellationTokenSource? _keepAliveCtsSource = null;

    private ServiceStateModel? _serviceStateModel = null;

    [ObservableProperty]
    private string keepAliveStatusMessage = "InActive";
    [ObservableProperty]
    private bool keepAliveNotActive = true;

    [ObservableProperty]
    private bool isPageExiting;
    [ObservableProperty]
    private bool isPageLoading;


    async partial void OnIsPageLoadingChanged(bool value)
    {
        if (value is false) return;
        try
        {
            DeployCardCollection.Clear();
            IsCollectionRefreshing = true;

            var services = ServiceStateStore.Services.Keys;
            foreach (var key in services)
            {
                Debug.WriteLine("Service key: " + key);

                ServiceStateModel service;
                ServiceStateStore.Services.TryGetValue(key, out service);
                if (service.CtsSource is null) return;

                Debug.WriteLine("Did service quit? " + service.CtsSource.IsCancellationRequested);
            }

            bool doesServiceExist = ServiceStateStore.Services.ContainsKey(CurrentService!.Id);
            if (doesServiceExist is true)
            {
                ServiceStateStore.Services.TryGetValue(CurrentService.Id, out _serviceStateModel);
                if (_serviceStateModel is null) return;

                KeepAliveRetryCounter = _serviceStateModel.RetryCount;
                _keepAliveCtsSource = _serviceStateModel.CtsSource;

                if (_serviceStateModel.IsRunning is true)
                {
                    KeepAliveStatusMessage = "Active";
                    KeepAliveNotActive = false;
                }
            }

            await Task.Run(async () =>
            {
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

                MainThread.BeginInvokeOnMainThread(() =>
                    DeployCardCollection.AddRange(deployDtos)
                );
            });
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


    [RelayCommand]
    async Task StopKeepAlive()
    {
        _keepAliveCtsSource?.Cancel();

        KeepAliveRetryCounter = 0;
        KeepAliveNotActive = true;
        KeepAliveStatusMessage = "InActive";
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
                KeepAliveStatusMessage = "Active";
                KeepAliveNotActive = false;

                bool result = await _renderClients.KeepAlive(CurrentService!.ServiceDetails!.Url);
                if (result is false)
                {
                    KeepAliveRetryCounter += 1;
                    RetryCounterHistory = KeepAliveRetryCounter;

                    await Task.Delay(1000, keepAliveCts);

                    continue;
                }

                await Task.Delay(300000, keepAliveCts);
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
            KeepAliveNotActive = true;
        }
    }

    [RelayCommand]
    async Task LaunchService()
    {
        try
        {
            Debug.WriteLine("swiper no swiping!!!");

            await Browser.Default.OpenAsync(new Uri(CurrentService!.ServiceDetails!.Url), BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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

            await Task.Run(async () =>
            {
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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeployCardCollection.AddRange(deployDtos);
                    DidInfiniteScrollRequested = false;
                });
            });
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

            await Task.Run(async () =>
            {
                deployDtos = await _renderClients.GetAllDeploys(CurrentService!.Id);

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

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    DeployCardCollection.Clear();
                    DeployCardCollection.AddRange(deployDtos);
                });
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
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
                _serviceStateModel.IsRunning = KeepAliveStatusMessage.Equals("Active") ? true : false;

                return;
            }

            if (KeepAliveStatusMessage.Equals("InActive")) return;

            ServiceStateStore.Services.Add(
                CurrentService!.Id,
                new ServiceStateModel
                {
                    ServiceId = CurrentService.Id,
                    RetryCount = KeepAliveRetryCounter,
                    CtsSource = _keepAliveCtsSource,
                    IsRunning = KeepAliveStatusMessage.Equals("Active") ? true : false
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
}
