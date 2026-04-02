namespace Tsubaki.Views;

public partial class Deploys : ContentPage
{
    public Deploys(DeploysViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var context = BindingContext as DeploysViewModel;
        context!.IsPageLoading = true;
    }

    protected override void OnDisappearing()
    {
        var context = BindingContext as DeploysViewModel;
        context!.IsPageExiting = true;

        base.OnDisappearing();
    }
}