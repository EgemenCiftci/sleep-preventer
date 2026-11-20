# Sleep Preventer

Sleep Preventer is a lightweight Windows tray application that keeps the system awake and prevents the display from turning off.

## Features

- Prevents sleep and display shutdown through the Windows `SetThreadExecutionState` API.
- Simulates F15 key-down and key-up events every minute.
- Runs from the system tray.
- Prevents a second copy of the application from starting.
- Restores normal sleep behavior when the application exits.

## Requirements

- Windows 10 or later
- [.NET 11 SDK](https://dotnet.microsoft.com/download/dotnet/11.0)
- Visual Studio 2026 or another IDE that supports .NET 11 Windows Forms projects

## Build and Run

### Visual Studio

1. Clone or download this repository.
2. Open `SleepPreventer.sln`.
3. Build the `SleepPreventer` project.
4. Start the application. Its icon will appear in the system tray.

### Command Line

From the repository root:

```powershell
dotnet restore
dotnet run --project .\SleepPreventer\SleepPreventer.csproj
```

To create a release build:

```powershell
dotnet build .\SleepPreventer\SleepPreventer.csproj --configuration Release
```

## Usage

- The application starts minimized in the system tray.
- Right-click the tray icon and select **Exit** to stop it.
- Starting a second instance opens an information message and closes the duplicate instance.

## How It Works

Sleep Preventer enables continuous system and display activity with `SetThreadExecutionState`. It also runs a timer that sends an F15 key-down/key-up event every minute.

F15 is typically an unassigned key and does not perform an application action. If the Windows API call is unavailable or unsuccessful, the app notifies the user and continues with keypress simulation.

On exit, Sleep Preventer stops the timer, disposes the tray resources, and resets the execution state to normal sleep behavior.

## Project Structure

- `SleepPreventer.sln` - Visual Studio solution
- `SleepPreventer/SleepPreventer.csproj` - .NET 11 Windows Forms project
- `SleepPreventer/Program.cs` - sleep-prevention, keypress, and tray icon implementation

## License

This project is licensed under the MIT License.
