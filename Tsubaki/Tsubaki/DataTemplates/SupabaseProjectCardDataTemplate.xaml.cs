using Tsubaki.Models.SupabaseModels;

namespace Tsubaki.DataTemplates;

public partial class SupabaseProjectCardDataTemplate : DataTemplate
{
    readonly SupabaseClients _supabaseClient;


    public SupabaseProjectCardDataTemplate()
    {
        InitializeComponent();
        _supabaseClient = Servicer.Provider!.GetService<SupabaseClients>()!;
    }


    private async void RefreshBtnClicked(object sender, EventArgs e)
    {
        try
        {
            await Task.Run(async () =>
            {
                var button = (Button)sender;
                var context = button.BindingContext as SupabaseProjectModel;

                await _supabaseClient.ResumeProjectById(context!.Id);
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Resuming project failed!: " + ex.Message);
        }
    }
}