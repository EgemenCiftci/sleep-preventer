# Sleep Preventer

Sleep Preventer is a lightweight Windows utility that prevents your computer from entering sleep mode or turning off the display. It accomplishes this by using Windows API calls to signal system activity, and as a fallback, simulates a harmless keypress if needed. The application runs in the system tray and can be exited at any time.

## Features

- Prevents system sleep and display turn-off using native Windows APIs.
- Simulates a keypress (F15, an unused key) as a fallback if API calls fail.
- Runs silently in the system tray with an easy exit option.
- Single-instance enforcement to avoid multiple running copies.

## Requirements

- Windows 10 or later
- [.NET 9.0 SDK or Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- Windows Forms support

## Installation

1. Clone or download this repository.
2. Open the solution in Visual Studio 2022 or later.
3. Build the project (`SleepPreventer`).
4. Run the application. The icon will appear in the system tray.

## Usage

- The application starts minimized in the system tray.
- To exit, right-click the tray icon and select **Exit**.
- If you try to start a second instance, you will be notified, and the new instance will close.

## How It Works

- Uses `SetThreadExecutionState` to prevent sleep and display turn-off.
- If this fails, it simulates an F15 keypress every minute to keep the system awake.
- Cleans up resources and restores normal sleep behavior on exit.

## License

This project is licensed under the MIT License.
