using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Avalonia.Controls;

namespace Xan.Zettl.Helpers;

internal static class WindowFocusHelper
{
    public static void BringToForeground(Window window)
    {
        window.Activate(); // handles macOS / Linux natively

        if (!OperatingSystem.IsWindows())
            return;

        var hwnd = window.TryGetPlatformHandle()?.Handle;
        if (hwnd is null or 0)
            return;

        ForceToForegroundWindows(hwnd.Value);
    }

    [SupportedOSPlatform("windows")]
    private static void ForceToForegroundWindows(nint hwnd)
    {
        uint ourThread = GetCurrentThreadId();
        uint fgThread  = GetWindowThreadProcessId(GetForegroundWindow(), out _);

        bool attached = fgThread != 0
                     && fgThread != ourThread
                     && AttachThreadInput(ourThread, fgThread, true);

        ShowWindow(hwnd, SW_RESTORE);
        BringWindowToTop(hwnd);
        SetForegroundWindow(hwnd);

        if (attached)
            AttachThreadInput(ourThread, fgThread, false);
    }

    private const int SW_RESTORE = 9;

    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetForegroundWindow(nint hWnd);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool BringWindowToTop(nint hWnd);
    [DllImport("user32.dll")]
    private static extern nint GetForegroundWindow();
    [DllImport("user32.dll")]
    private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool attach);
    [DllImport("user32.dll")] [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ShowWindow(nint hWnd, int nCmdShow);
}
