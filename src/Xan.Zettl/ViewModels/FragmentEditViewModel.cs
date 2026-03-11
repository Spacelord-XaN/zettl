using CommunityToolkit.Mvvm.ComponentModel;

namespace Xan.Zettl.ViewModels;

public partial class FragmentEditViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsValid))]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _text = string.Empty;

    public bool IsValid => !string.IsNullOrWhiteSpace(Name);
}
