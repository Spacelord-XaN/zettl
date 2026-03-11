using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Xan.Zettl.Models;
using Xan.Zettl.ViewModels;

namespace Xan.Zettl.Views;

public partial class PastePickerWindow : Window
{
    public TextFragment? SelectedFragment { get; private set; }

    public PastePickerWindow()
    {
        InitializeComponent();
    }

    public PastePickerWindow(PastePickerViewModel vm) : this()
    {
        DataContext = vm;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        SearchBox.Focus();
        AddHandler(KeyDownEvent, OnWindowKeyDown, RoutingStrategies.Tunnel);
        ResultList.DoubleTapped += (_, _) =>
        {
            if (DataContext is PastePickerViewModel vm) Confirm(vm);
        };
    }

    private void OnWindowKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not PastePickerViewModel vm) return;

        switch (e.Key)
        {
            case Key.Down:
                MoveSelection(vm, 1);
                e.Handled = true;
                break;
            case Key.Up:
                MoveSelection(vm, -1);
                e.Handled = true;
                break;
            case Key.Enter:
                Confirm(vm);
                e.Handled = true;
                break;
            case Key.Escape:
                Close();
                e.Handled = true;
                break;
        }
    }

    private void MoveSelection(PastePickerViewModel vm, int delta)
    {
        var items = vm.FilteredFragments;
        if (items.Count == 0) return;

        var current = vm.SelectedFragment;
        int currentIdx = -1;
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == current) { currentIdx = i; break; }
        }

        var newIdx = Math.Clamp(currentIdx + delta, 0, items.Count - 1);
        vm.SelectedFragment = items[newIdx];
        ResultList.ScrollIntoView(newIdx);
    }

    private void Confirm(PastePickerViewModel vm)
    {
        if (vm.SelectedFragment is null) return;
        SelectedFragment = vm.SelectedFragment;
        Close();
    }
}
