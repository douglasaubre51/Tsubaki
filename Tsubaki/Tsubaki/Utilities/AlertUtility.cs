namespace Tsubaki.Utilities;

public static class AlertUtility
{
    public static async Task ShowClassicAlertDialog()
    {
        await Shell.Current.DisplayAlertAsync(
            "Page load error",
            "Something went wrong while loading the page!",
            "Ok"
            );
    }

    public static async Task ShowAdvancedAlertDialog(string message)
    {
        await Shell.Current.DisplayAlertAsync(
            "Warning",
            message,
            "Ok"
            );
    }
}
