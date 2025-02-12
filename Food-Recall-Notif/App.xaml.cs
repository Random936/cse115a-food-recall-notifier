namespace Food_Recall_Notif;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
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