namespace Tsubaki.Views;

public partial class SupabaseView : ContentPage
{
    public SupabaseView(SupabaseViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        var context = BindingContext as SupabaseViewModel;
        Dispatcher.Dispatch(() => context!.IsPageLoading = true);
    }
}