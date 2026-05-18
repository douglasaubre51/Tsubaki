namespace Tsubaki.ViewModels;

public partial class MainPageViewModel(RenderClients renderClient) : BaseViewModel
{
    private readonly RenderClients _renderClient = renderClient;

    [ObservableProperty]
    private bool isPageLoading;
    [ObservableProperty]
    private bool isCollectionRefreshing;
    [ObservableProperty]
    private bool isServiceChanging;

    [ObservableProperty]
    private ObservableRangeCollection<RenderDtos> serviceCardCollection = [];


    async partial void OnIsPageLoadingChanged(bool value)
    {
        if (value is false) return;

        try
        {
            if (ServiceCardCollection.Count is not 0) return;

            IsBusy = true;
            IsServiceChanging = true;

            await Task.Run(async () =>
            {
                Debug.WriteLine("Fetching service data...");

                var data = await _renderClient.GetAllServices();
                if (data is null || data.Count == 0)
                {
                    Debug.WriteLine("empty data!");
                    return;
                }

                Debug.WriteLine("Service count: " + data.Count);

                foreach (var s in data)
                {
                    if (s.Service!.Suspended != StatusEnum.suspended.ToString())
                    {
                        s.IsActive = true;
                        s.ServiceCardStatusColor = Color.FromHex(CardColorStore.ActiveCard);
                        s.IsNotActive = false;
                    }
                    else
                    {
                        s.IsActive = false;
                        s.IsNotActive = true;
                        s.ServiceCardStatusColor = Color.FromHex(CardColorStore.SuspendedCard);
                    }
                }

                Debug.WriteLine("Fetching service data complete!");

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Debug.WriteLine("Flushing data to collection!");
                    ServiceCardCollection.AddRange(data);
                });
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsServiceChanging = false;
            IsBusy = false;
            IsPageLoading = false;
        }
    }

    [RelayCommand]
    async Task RefreshServiceCardCollection()
    {
        await Task.Run(async () =>
        {
            var data = await _renderClient.GetAllServices();
            if (data is null || data.Count == 0) return;

            Debug.WriteLine("Refreshing service data!");

            foreach (var s in data)
            {
                if (s.Service!.Suspended != StatusEnum.suspended.ToString())
                {
                    s.IsActive = true;
                    s.ServiceCardStatusColor = Color.FromHex(CardColorStore.ActiveCard);
                    s.IsNotActive = false;
                }
                else
                {
                    s.IsActive = false;
                    s.IsNotActive = true;
                    s.ServiceCardStatusColor = Color.FromHex(CardColorStore.SuspendedCard);
                }
            }

            MainThread.BeginInvokeOnMainThread(() =>
                ServiceCardCollection.ReplaceRange(data)
            );
        });
    }

    [RelayCommand]
    async Task RefreshCollection()
        => await RefreshServiceCardCollection();


    [RelayCommand]
    async Task GoToDeploys(Service selectedService)
    {
        if (IsBusy is true) return;

        IsBusy = true;
        try
        {
            await Shell.Current.GoToAsync(
                "Deploys",
                false,
                new Dictionary<string, object>
                {
                    { "SelectedService" , selectedService  }
                });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("navigating to deploy error: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }


    [RelayCommand]
    async Task StopService(RenderDtos dto)
    {
        if (IsBusy is true) return;

        IsBusy = true;
        IsServiceChanging = true;
        try
        {
            await _renderClient.SuspendServiceById(dto.Service!.Id);
            await RefreshServiceCardCollection();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("error stopping service: " + ex.Message);
            await Shell.Current.DisplayAlertAsync("Error", "error stopping service!", "ok");
        }
        finally
        {
            IsServiceChanging = false;
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task RestartService(RenderDtos dto)
    {
        if (IsBusy is true) return;

        IsBusy = true;
        IsServiceChanging = true;
        try
        {
            await _renderClient.RestartServiceById(dto.Service!.Id);
            await RefreshServiceCardCollection();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("error restarting service: " + ex.Message);
            await Shell.Current.DisplayAlertAsync("Error", "error restarting service!", "ok");
        }
        finally
        {
            IsServiceChanging = false;
            IsBusy = false;
        }
    }
    [RelayCommand]
    async Task StartService(RenderDtos dto)
    {
        if (IsBusy is true) return;

        IsBusy = true;
        IsServiceChanging = true;
        try
        {
            await _renderClient.ResumeServiceById(dto.Service!.Id);
            await RefreshServiceCardCollection();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("error starting service: " + ex.Message);
            await Shell.Current.DisplayAlertAsync("Error", "error starting service!", "ok");
        }
        finally
        {
            IsServiceChanging = false;
            IsBusy = false;
        }
    }


    [RelayCommand]
    async Task OpenProjectSite(string url)
    {
        try
        {
            await Browser.Default.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("error launching site: " + ex.Message);
            await Shell.Current.DisplayAlertAsync("Error", "error launching site!", "ok");
        }
    }
}
