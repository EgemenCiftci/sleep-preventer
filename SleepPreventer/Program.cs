using System.Runtime.InteropServices;
using Timer = System.Threading.Timer;

namespace SleepPreventer;

internal static partial class Program
{
    [LibraryImport("kernel32.dll")]
    private static partial uint SetThreadExecutionState(uint esFlags);

    [LibraryImport("user32.dll")]
    private static partial void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;
    private const byte VK_F15 = 0x7E;
    private const uint KEYEVENTF_KEYDOWN = 0x0000;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private static NotifyIcon? trayIcon;
    private static ContextMenuStrip? trayMenu;
    private static Timer? keypressTimer;

    [STAThread]
    private static void Main()
    {
        using Mutex mutex = new(true, "SleepPreventerAppMutex", out bool isNewInstance);

        if (!isNewInstance)
        {
            _ = MessageBox.Show("Sleep Preventer is already running.", "Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ApplicationConfiguration.Initialize();

        if (!EnableSleepPrevention())
        {
            _ = MessageBox.Show("Failed to prevent sleep. Keypress simulation will start instead.",
                "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        StartKeypressSimulation();
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
        uint result = SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED);
        return result != 0;
    }

    private static void StartKeypressSimulation()
    {
        keypressTimer = new Timer(SimulateKeyPress, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
    }

    private static void SimulateKeyPress(object? state)
    {
        try
        {
            keybd_event(VK_F15, 0, KEYEVENTF_KEYDOWN, 0);
            keybd_event(VK_F15, 0, KEYEVENTF_KEYUP, 0);
        }
        catch (Exception ex)
        {
            _ = MessageBox.Show($"Failed to simulate key press: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
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
        keypressTimer?.Dispose();
        _ = SetThreadExecutionState(ES_CONTINUOUS);

        trayMenu?.Dispose();
        trayMenu = null;

        if (trayIcon != null)
        {
            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayIcon = null;
        }
    }
}
