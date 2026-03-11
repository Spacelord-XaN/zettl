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

        EditButton.Click += async (_, _) =>
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
        };

        FragmentList.DoubleTapped += async (_, _) =>
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
        };

        Closing += (_, args) =>
        {
            if (DataContext is MainWindowViewModel vm && vm.IsDirty)
            {
                // Show a simple blocking dialog
                var box = new Window
                {
                    Title = "Unsaved Changes",
                    Width = 380,
                    Height = 150,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                };

                bool discardChosen = false;
                var panel = new StackPanel { Margin = new Avalonia.Thickness(16), Spacing = 16 };
                panel.Children.Add(new TextBlock
                {
                    Text = "You have unsaved changes. Discard them?",
                    TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                });

                var buttons = new StackPanel
                {
                    Orientation = Avalonia.Layout.Orientation.Horizontal,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                    Spacing = 8,
                };

                var saveBtn = new Button { Content = "Save & Close" };
                var discardBtn = new Button { Content = "Discard" };
                var cancelBtn = new Button { Content = "Cancel" };

                saveBtn.Click += (_, _) => { vm.SaveCommand.Execute(null); box.Close(); };
                discardBtn.Click += (_, _) => { discardChosen = true; box.Close(); };
                cancelBtn.Click += (_, _) => box.Close();

                buttons.Children.Add(saveBtn);
                buttons.Children.Add(discardBtn);
                buttons.Children.Add(cancelBtn);
                panel.Children.Add(buttons);
                box.Content = panel;

                args.Cancel = true;

                box.Closed += (_, _) =>
                {
                    if (discardChosen || !vm.IsDirty)
                        Close();
                };

                box.ShowDialog(this);
            }
        };
    }
}
