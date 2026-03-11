using Avalonia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xan.Zettl.Models;

namespace Xan.Zettl;

sealed class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length >= 1 && args[0] == "--paste")
        {
            if (args.Length >= 2)
                Paste(args[1]);
            else
                RunPicker();
            return;
        }

        if (args.Length == 0 || args[0] == "--edit")
        {
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            return;
        }

        Console.WriteLine("Usage:");
        Console.WriteLine("  zettl --edit             Open the fragment editor");
        Console.WriteLine("  zettl --paste [name]     Copy a fragment to the clipboard");
    }

    private static void Paste(string name)
    {
        var fragments = TextFragmentStore.Load();
        var fragment = fragments.Find(f =>
            string.Equals(f.Name, name, StringComparison.OrdinalIgnoreCase));

        if (fragment is null)
        {
            Console.Error.WriteLine($"Fragment '{name}' not found.");
            Environment.Exit(1);
            return;
        }

        PasteResult(fragment);
    }

    private static void RunPicker()
    {
        var fragments = TextFragmentStore.Load();
        if (fragments.Count == 0)
        {
            Console.Error.WriteLine("No fragments saved yet.");
            return;
        }

        App.PastePickerMode = true;
        App.PastePickerFragments = fragments;
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(Array.Empty<string>());

        var selected = App.PastePickerWindowInstance?.SelectedFragment;
        if (selected is not null)
            PasteResult(selected);
    }

    private static void PasteResult(TextFragment fragment)
    {
        if (!Console.IsOutputRedirected && TrySetClipboard(fragment.Text))
            Console.WriteLine($"Fragment '{fragment.Name}' copied to clipboard.");
        else
            Console.Write(fragment.Text);
    }

    private static bool TrySetClipboard(string text)
    {
        try
        {
            ProcessStartInfo psi;

            if (OperatingSystem.IsWindows())
            {
                psi = new ProcessStartInfo("clip")
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                return RunWithStdin(psi, text);
            }

            if (OperatingSystem.IsMacOS())
            {
                psi = new ProcessStartInfo("pbcopy")
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                return RunWithStdin(psi, text);
            }

            // Linux - try Wayland then X11
            foreach (var (cmd, argStr) in new (string, string)[]
            {
                ("wl-copy", string.Empty),
                ("xclip", "-selection clipboard"),
                ("xsel", "--clipboard --input"),
            })
            {
                psi = new ProcessStartInfo(cmd, argStr)
                {
                    RedirectStandardInput = true,
                    UseShellExecute = false
                };
                if (RunWithStdin(psi, text))
                    return true;
            }
        }
        catch { /* fall through to stdout */ }

        return false;
    }

    private static bool RunWithStdin(ProcessStartInfo psi, string input)
    {
        try
        {
            using var proc = Process.Start(psi);
            if (proc is null) return false;
            proc.StandardInput.Write(input);
            proc.StandardInput.Close();
            proc.WaitForExit();
            return proc.ExitCode == 0;
        }
        catch { return false; }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
