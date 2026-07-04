namespace Tsubaki.Views;

public partial class SettingsView : ContentPage
{
    public SettingsView(SettingsViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        var viewModel = BindingContext as SettingsViewModel;
        Dispatcher.Dispatch(() => viewModel!.IsPageLoading = true);
    }

    private void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        var viewModel = BindingContext as SettingsViewModel;
        viewModel!.IsDirty = true;
    }

    private void Entry_TextChanged_1(object sender, TextChangedEventArgs e)
    {
        var viewModel = BindingContext as SettingsViewModel;
        viewModel!.IsDirty = true;
    }

    private void Entry_TextChanged_2(object sender, TextChangedEventArgs e)
    {
        var viewModel = BindingContext as SettingsViewModel;
        viewModel!.IsDirty = true;
    }
}