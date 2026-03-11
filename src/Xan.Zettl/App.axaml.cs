using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using Xan.Zettl.Models;
using Xan.Zettl.ViewModels;
using Xan.Zettl.Views;

namespace Xan.Zettl;

public partial class App : Application
{
    internal static bool PastePickerMode { get; set; }
    internal static IReadOnlyList<TextFragment> PastePickerFragments { get; set; } = [];
    internal static PastePickerWindow? PastePickerWindowInstance { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DisableAvaloniaDataAnnotationValidation();

            if (PastePickerMode)
            {
                var vm = new PastePickerViewModel(PastePickerFragments);
                var window = new PastePickerWindow(vm);
                PastePickerWindowInstance = window;
                desktop.MainWindow = window;
            }
            else
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(),
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}