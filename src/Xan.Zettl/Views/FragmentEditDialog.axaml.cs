using Avalonia.Controls;
using Avalonia.Interactivity;
using Xan.Zettl.ViewModels;

namespace Xan.Zettl.Views;

public partial class FragmentEditDialog : Window
{
    public bool Confirmed { get; private set; }

    public FragmentEditDialog()
    {
        InitializeComponent();
    }

    public FragmentEditDialog(FragmentEditViewModel vm) : this()
    {
        DataContext = vm;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        OkButton.Click += (_, _) => { Confirmed = true; Close(); };
        CancelButton.Click += (_, _) => { Confirmed = false; Close(); };
        KeyDown += (_, ke) =>
        {
            if (ke.Key == Avalonia.Input.Key.Escape)
            {
                Confirmed = false;
                Close();
            }
        };
    }
}
