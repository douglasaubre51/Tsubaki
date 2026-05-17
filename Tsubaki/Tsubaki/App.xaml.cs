namespace Tsubaki;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new Window(new AppShell());

        window.Width = 1200;
        window.Height = 700;

        return window;
    }
}
