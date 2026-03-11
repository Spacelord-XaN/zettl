using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xan.TextBoard.Models;

namespace Xan.TextBoard.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<TextFragment> Fragments { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private TextFragment? _selectedFragment;

    [ObservableProperty]
    private bool _isDirty;

    public MainWindowViewModel()
    {
        foreach (var f in TextFragmentStore.Load())
            Fragments.Add(f);
    }

    [RelayCommand]
    private void Add()
    {
        var fragment = new TextFragment { Name = "New Fragment", Text = string.Empty };
        Fragments.Add(fragment);
        SelectedFragment = fragment;
        IsDirty = true;
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Delete()
    {
        if (SelectedFragment is null) return;
        Fragments.Remove(SelectedFragment);
        SelectedFragment = Fragments.FirstOrDefault();
        IsDirty = true;
    }

    // Called from the view after the edit dialog returns OK
    public void ApplyEdit(TextFragment target, string newName, string newText)
    {
        target.Name = newName;
        target.Text = newText;
        IsDirty = true;
        // Notify the list to refresh the displayed name
        var idx = Fragments.IndexOf(target);
        if (idx >= 0)
        {
            Fragments.RemoveAt(idx);
            Fragments.Insert(idx, target);
            SelectedFragment = target;
        }
    }

    [RelayCommand]
    private void Save()
    {
        TextFragmentStore.Save(Fragments);
        IsDirty = false;
    }

    private bool HasSelection => SelectedFragment is not null;

    // Placeholder – actual dialog invocation is done from code-behind
    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Edit() { }
}
