# Overlay Application - Implementation Guide

Technical documentation for developers working with the Overlay Application codebase.

## Project Structure

```
OverlaySolution/
├── Overlay.App/          # Main WPF application (GUI mode)
├── Overlay.Cli/          # Command-line interface
├── Overlay.Core/         # Core models and interfaces
├── Overlay.Interop/      # Win32 interop utilities
├── Overlay.Render/       # Rendering engine
└── Overlay.Setup/        # Installation/setup utilities
```

## Architecture Overview

The application follows a modular architecture with clear separation of concerns:

1. **Core Layer** (`Overlay.Core`): Contains data models, interfaces, and business logic
2. **Interop Layer** (`Overlay.Interop`): Handles Windows API calls for advanced window management
3. **Rendering Layer** (`Overlay.Render`): Responsible for visual rendering of overlays
4. **Presentation Layer** (`Overlay.App`, `Overlay.Cli`): User interfaces (GUI and CLI)
5. **Setup Layer** (`Overlay.Setup`): Packaging and deployment utilities

## Core Components

### Card Model

The `Card` class in `Overlay.Core/Card.cs` represents individual overlay elements:

```csharp
public class Card
{
    public string Id { get; set; }
    public CardType Type { get; set; } // Text or Image
    public double X { get; set; }
    public double Y { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public double Opacity { get; set; }
    public string Text { get; set; }
    public string ForegroundColor { get; set; }
    public string DropShadowColor { get; set; }
    public bool IsStrobing { get; set; }
    // ... other properties
}
```

### Renderer Interface

The `IRenderer` interface in `Overlay.Render/IRenderer.cs` defines the rendering contract:

```csharp
public interface IRenderer
{
    void RenderCard(Card card, Canvas canvas);
    void RenderScene(Scene scene, Configuration config, Canvas canvas);
    void ClearCanvas(Canvas canvas);
}
```

### Win32 Interop

The `Overlay.Interop` project contains utilities for Windows API interaction:

```csharp
public static class WindowStyles
{
    public const uint WS_EX_LAYERED = 0x00080000;
    public const uint WS_EX_TRANSPARENT = 0x00000020;
    public const uint WS_EX_TOOLWINDOW = 0x00000080;
    public const int GWL_EXSTYLE = -20;
}

public static class User32
{
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    
    [DllImport("user32.dll")]
    public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
```

## CLI Implementation

The command-line interface in `Overlay.Cli/Program.cs` handles argument parsing and overlay execution:

### Argument Parsing

```csharp
static OverlayOptions ParseArguments(string[] args)
{
    var options = new OverlayOptions();
    
    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i].ToLower())
        {
            case "-text":
                if (i + 1 < args.Length)
                    options.Texts = new string[] { args[i + 1] };
                break;
            case "-texts":
                if (i + 1 < args.Length)
                    options.Texts = args[i + 1].Split(';');
                break;
            // ... other cases
        }
    }
    
    return options;
}
```

### Overlay Application

The `OverlayApplication` class manages the WPF window lifecycle:

```csharp
public void Run(OverlayOptions options)
{
    // Create WPF application and window
    var app = new System.Windows.Application();
    _window = new Window { /* window configuration */ };
    
    // Position window on specified display
    PositionWindowOnDisplay(options);
    
    // Set up text rotation for multiple texts
    if (options.Texts.Length > 1)
    {
        _switchTimer = new DispatcherTimer();
        _switchTimer.Interval = TimeSpan.FromSeconds(options.SwitchInterval);
        _switchTimer.Tick += SwitchText;
        _switchTimer.Start();
    }
    
    // Auto-close after duration
    _closeTimer = new DispatcherTimer();
    _closeTimer.Interval = TimeSpan.FromSeconds(options.Duration);
    _closeTimer.Tick += (sender, e) => { /* close window */ };
    _closeTimer.Start();
    
    // Show and run
    _window.Show();
    app.Run();
}
```

## Text Rotation Mechanism

The text rotation feature uses two timers:

1. **Switch Timer**: Changes the displayed text at specified intervals
2. **Close Timer**: Automatically closes the overlay after the duration

```csharp
private void SwitchText(object? sender, EventArgs e)
{
    // Move to next text in rotation
    _currentTextIndex = (_currentTextIndex + 1) % _texts.Length;
    
    // Update and re-render
    _currentCard.Text = _texts[_currentTextIndex];
    _renderer.ClearCanvas(_canvas);
    _renderer.RenderCard(_currentCard, _canvas);
}
```

## Grid Positioning System

The grid positioning system automatically adapts to display orientation:

```csharp
private (double x, double y) CalculateGridPosition(OverlayOptions options)
{
    // Get screen dimensions
    var screen = System.Windows.Forms.Screen.AllScreens[options.DisplayIndex];
    
    // Adjust for vertical displays
    int columns = screen.Bounds.Width < screen.Bounds.Height ? 1 : 3;
    int rows = 3;
    
    // Calculate grid cell dimensions
    double cellWidth = (double)screen.Bounds.Width / columns;
    double cellHeight = (double)screen.Bounds.Height / rows;
    
    // Position in center of grid cell
    double x = screen.Bounds.Left + (options.Column * cellWidth) + (cellWidth - 800) / 2;
    double y = screen.Bounds.Top + (options.Row * cellHeight) + (cellHeight - 200) / 2;
    
    return (x, y);
}
```

## Strobing Effect Implementation

The strobing effect uses a `DispatcherTimer` to rapidly switch text colors:

```csharp
private void StartStrobingEffect(TextBlock textBlock, string cardId)
{
    var timer = new DispatcherTimer();
    timer.Interval = TimeSpan.FromMilliseconds(500);
    bool isBright = false;
    
    timer.Tick += (sender, e) => {
        isBright = !isBright;
        textBlock.Foreground = isBright ? 
            new SolidColorBrush(Colors.Purple) : 
            new SolidColorBrush(Colors.White);
    };
    
    timer.Start();
    _strobingTimers[cardId] = timer;
}
```

## Multi-Monitor Support

The application uses `System.Windows.Forms.Screen.AllScreens` to detect and work with multiple displays:

```csharp
private void PositionWindowOnDisplay(OverlayOptions options)
{
    var screens = System.Windows.Forms.Screen.AllScreens;
    int displayIndex = Math.Max(0, Math.Min(options.DisplayIndex, screens.Length - 1));
    var screen = screens[displayIndex];
    
    _window.Left = screen.Bounds.Left;
    _window.Top = screen.Bounds.Top;
    _window.Width = screen.Bounds.Width;
    _window.Height = screen.Bounds.Height;
}
```

## Click-Through Functionality

Click-through is implemented using Win32 extended window styles:

```csharp
// Enable click-through
User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, 
    extendedStyle | (int)WindowStyles.WS_EX_TRANSPARENT);

// Disable click-through
User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, 
    extendedStyle & ~(int)WindowStyles.WS_EX_TRANSPARENT);
```

## Building and Deployment

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or later (optional)
- Windows SDK

### Build Process

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run CLI application
dotnet run --project Overlay.Cli -- [arguments]

# Publish for deployment
dotnet publish -c Release -r win-x64 --self-contained
```

### Project Dependencies

The solution uses the following NuGet packages:

- `System.Drawing.Common` (for color handling)
- `System.Windows.Forms` (for multi-monitor detection)

## Extending the Application

### Adding New Card Types

1. Add new enum value to `CardType` in `Card.cs`
2. Update the renderer to handle the new type
3. Add any new properties to the `Card` class

### Adding New Visual Effects

1. Add new properties to the `Card` model
2. Update the renderer to apply the effects
3. Add new CLI arguments to parse the options

### Adding New Positioning Systems

1. Add new positioning properties to `OverlayOptions`
2. Update the positioning logic in `OverlayApplication`
3. Add new CLI arguments for the positioning system

## Performance Considerations

- Use `Freeze()` on `BitmapImage` objects to improve performance
- Limit the number of simultaneous strobing effects
- Use `DispatcherTimer` instead of `System.Threading.Timer` for UI updates
- Dispose of timers and resources properly

## Testing

The application includes basic functionality testing through:

1. **Manual testing** using the provided batch scripts
2. **Visual verification** of overlay positioning and effects
3. **Multi-monitor testing** on various display configurations

## Future Enhancements

Potential areas for future development:

1. **Configuration Files**: Support for JSON configuration files
2. **Image Support**: Display of custom images with transparency
3. **Animation System**: Smooth transitions between texts
4. **Remote Control**: HTTP API for controlling overlays
5. **Persistence**: Save and load overlay configurations
6. **Scheduling**: Time-based overlay activation
7. **Audio Integration**: Sound effects or text-to-speech