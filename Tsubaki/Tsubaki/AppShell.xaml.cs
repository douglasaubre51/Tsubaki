
namespace Tsubaki;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("Deploys", typeof(Deploys));
    }
}
