using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using Xan.Zettl.Models;

namespace Xan.Zettl.ViewModels;

public partial class PastePickerViewModel : ViewModelBase
{
    private readonly IReadOnlyList<TextFragment> _allFragments;

    public PastePickerViewModel(IReadOnlyList<TextFragment> fragments)
    {
        _allFragments = fragments;
        _selectedFragment = fragments.Count > 0 ? fragments[0] : null;
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FilteredFragments))]
    private string _query = string.Empty;

    [ObservableProperty]
    private TextFragment? _selectedFragment;

    public IReadOnlyList<TextFragment> FilteredFragments =>
        string.IsNullOrEmpty(Query)
            ? _allFragments
            : _allFragments.Where(f => FuzzyMatch(f.Name, Query)).ToList();

    partial void OnQueryChanged(string value)
    {
        SelectedFragment = FilteredFragments.Count > 0 ? FilteredFragments[0] : null;
    }

    private static bool FuzzyMatch(string text, string query)
    {
        int qi = 0;
        foreach (char c in text)
        {
            if (char.ToLowerInvariant(c) == char.ToLowerInvariant(query[qi]))
                qi++;
            if (qi == query.Length)
                return true;
        }
        return false;
    }
}
