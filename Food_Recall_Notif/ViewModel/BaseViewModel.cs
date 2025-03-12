using CommunityToolkit.Mvvm.ComponentModel;

namespace Food_Recall_Notif.ViewModel;

public partial class BaseViewModel : ObservableObject
{

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;

    public bool IsNotBusy => !IsBusy;
}
