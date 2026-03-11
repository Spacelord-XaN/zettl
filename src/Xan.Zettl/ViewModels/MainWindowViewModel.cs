using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Xan.Zettl.Models;

namespace Xan.Zettl.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<TextFragment> Fragments { get; } = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EditCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private TextFragment? _selectedFragment;

    public MainWindowViewModel()
    {
        foreach (var f in TextFragmentStore.Load())
            Fragments.Add(f);
    }

    // Called from the view after the add dialog returns OK
    public void AddFragment(string name, string text)
    {
        var fragment = new TextFragment { Name = name, Text = text };
        Fragments.Add(fragment);
        SelectedFragment = fragment;
        AutoSave();
    }

    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Delete()
    {
        if (SelectedFragment is null) return;
        Fragments.Remove(SelectedFragment);
        SelectedFragment = Fragments.FirstOrDefault();
        AutoSave();
    }

    // Called from the view after the edit dialog returns OK
    public void ApplyEdit(TextFragment target, string newName, string newText)
    {
        target.Name = newName;
        target.Text = newText;
        // Notify the list to refresh the displayed name
        var idx = Fragments.IndexOf(target);
        if (idx >= 0)
        {
            Fragments.RemoveAt(idx);
            Fragments.Insert(idx, target);
            SelectedFragment = target;
        }
        AutoSave();
    }

    private void AutoSave() => TextFragmentStore.Save(Fragments);

    private bool HasSelection => SelectedFragment is not null;

    // Placeholder – actual dialog invocation is done from code-behind
    [RelayCommand(CanExecute = nameof(HasSelection))]
    private void Edit() { }
}
