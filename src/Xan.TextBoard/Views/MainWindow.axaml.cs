using Avalonia.Controls;
using Avalonia.Interactivity;
using Xan.TextBoard.ViewModels;

namespace Xan.TextBoard.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        AddButton.Click += async (_, _) =>
        {
            if (DataContext is not MainWindowViewModel vm)
                return;

            var editVm = new FragmentEditViewModel();
            var dialog = new FragmentEditDialog(editVm);
            await dialog.ShowDialog(this);

            if (dialog.Confirmed)
                vm.AddFragment(editVm.Name, editVm.Text);
        };

        EditButton.Click += async (_, _) => await OpenEditDialog();
        FragmentList.DoubleTapped += async (_, _) => await OpenEditDialog();

        KeyDown += (_, e) =>
        {
            if (e.Key == Avalonia.Input.Key.Escape)
                Close();
        };
    }

    private async System.Threading.Tasks.Task OpenEditDialog()
    {
        if (DataContext is not MainWindowViewModel vm || vm.SelectedFragment is null)
            return;

        var editVm = new FragmentEditViewModel
        {
            Name = vm.SelectedFragment.Name,
            Text = vm.SelectedFragment.Text,
        };

        var dialog = new FragmentEditDialog(editVm);
        await dialog.ShowDialog(this);

        if (dialog.Confirmed)
            vm.ApplyEdit(vm.SelectedFragment, editVm.Name, editVm.Text);
    }
}
