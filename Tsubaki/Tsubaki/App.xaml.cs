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
        window.Width = 1000;
        window.Height = 700;

        var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        window.X = displayInfo.Width / displayInfo.Density - 1000;
        window.Y = displayInfo.Height / displayInfo.Density - 700;

        return window;
    }
}
