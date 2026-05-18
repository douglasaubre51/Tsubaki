namespace Tsubaki;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();

        var context = BindingContext as MainPageViewModel;

        await Dispatcher.DispatchAsync(() =>
            context!.IsPageLoading = true
        );
    }
}
