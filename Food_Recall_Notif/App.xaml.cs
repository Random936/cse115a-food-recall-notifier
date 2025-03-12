namespace Food_Recall_Notif;

public partial class App : Application
{
    // Expose the service provider for later use.
    public static IServiceProvider Services { get; private set; } = null!;

    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        Services = serviceProvider;
        


    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        if (activationState == null)
        {
            throw new ArgumentNullException(nameof(activationState), "Activation state cannot be null.");
        }

        return new Window(new AppShell());
    }
}
