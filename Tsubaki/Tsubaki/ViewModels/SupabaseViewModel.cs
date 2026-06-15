using Tsubaki.Models.SupabaseModels;

namespace Tsubaki.ViewModels;

public partial class SupabaseViewModel(SupabaseClients supabaseServ) : BaseViewModel
{
    readonly SupabaseClients _supabaseServ = supabaseServ;

    [ObservableProperty]
    ObservableRangeCollection<SupabaseProjectModel> projects = [];

    [ObservableProperty]
    bool isPageLoading;


    async partial void OnIsPageLoadingChanged(bool value)
    {
        if (value is false) return;

        try
        {
            if (Projects.Count is not 0) return;

            IsBusy = true;

            await Task.Run(async () =>
            {
                var dtos = await _supabaseServ.GetAllProjects();
                List<SupabaseProjectModel> models = [];

                foreach (var dto in dtos)
                {
                    SupabaseProjectModel model = new SupabaseProjectModel()
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        OrganizationId = dto.OrganizationId,
                        OrganizationSlug = dto.OrganizationSlug,
                        Region = dto.Region,
                        CreatedAt = dto.CreatedAt,
                        Status = dto.Status,
                        DBHost = dto.Database!.Host ?? string.Empty
                    };
                    models.Add(model);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Projects.AddRange(models);
                    IsBusy = false;
                });
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine("PageLoad: SupabaseView: error: " + ex.Message);
        }
        finally
        {
            IsBusy = false;
            IsPageLoading = false;
        }
    }
}
