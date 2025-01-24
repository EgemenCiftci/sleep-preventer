using System.Runtime.InteropServices;

namespace SleepPreventer;

internal static class Program
{
    [DllImport("kernel32.dll")]
    private static extern uint SetThreadExecutionState(uint esFlags);
    private const uint ES_CONTINUOUS = 0x80000000;
    private const uint ES_DISPLAY_REQUIRED = 0x00000002;
    private const uint ES_SYSTEM_REQUIRED = 0x00000001;


    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        if (SetThreadExecutionState(ES_CONTINUOUS | ES_DISPLAY_REQUIRED | ES_SYSTEM_REQUIRED) == 0)
        {
            _ = MessageBox.Show("Failed to enable sleep prevention.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        NotifyIcon trayIcon = new()
        {
            Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath),
            Text = "Sleep Preventer",
            Visible = true
        };

        ContextMenuStrip trayMenu = new();

        _ = trayMenu.Items.Add("Exit", null, (sender, e) =>
        {
            _ = SetThreadExecutionState(ES_CONTINUOUS);
            trayIcon.Visible = false;
            trayIcon.Dispose();
            Application.Exit();
        });

        trayIcon.ContextMenuStrip = trayMenu;

        Application.Run();

        _ = SetThreadExecutionState(ES_CONTINUOUS);
    }
}