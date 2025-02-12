using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Food_Recall_Notif.ViewModel;

public partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
    {

    }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool isBusy;
    [ObservableProperty]
    string title;
    public bool IsNotBusy => !IsBusy;

}