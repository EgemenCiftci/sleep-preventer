using System.Runtime.InteropServices;

namespace SleepPreventer;

internal static class Program
{
    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;

    private static NotifyIcon? trayIcon;
    private static ContextMenuStrip? trayMenu;

    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (!EnableSleepPrevention())
        {
            _ = MessageBox.Show("Failed to enable sleep prevention.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        InitializeTrayIcon();

        try
        {
            Application.Run();
        }
        finally
        {
            CleanupResources();
        }
    }

    private static bool EnableSleepPrevention()
    {
        return SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED) != 0;
    }

    private static void InitializeTrayIcon()
    {
        trayIcon = new NotifyIcon
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Text = "Sleep Preventer",
            Visible = true
        };

        trayMenu = new ContextMenuStrip();
        _ = trayMenu.Items.Add("Exit", null, ExitApplication);

        trayIcon.ContextMenuStrip = trayMenu;
    }

    private static void ExitApplication(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    private static void CleanupResources()
    {
        _ = SetThreadExecutionState(ES_CONTINUOUS);

        if (trayMenu != null)
        {
            trayMenu.Dispose();
            trayMenu = null;
        }

        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayIcon = null;
        }
    }
}
