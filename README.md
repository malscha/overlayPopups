# Overlay Application

A powerful, scriptable overlay tool for Windows that displays customizable text and images on top of other applications. Features include multi-monitor support, grid positioning, text rotation, strobing effects, and customizable colors.

## Features

- **Scriptable CLI Interface**: Control overlays through command-line parameters
- **Multi-Monitor Support**: Display overlays on any connected monitor
- **Grid Positioning**: Position overlays in a 3x3 grid layout (automatically adjusts for vertical displays)
- **Text Rotation**: Display multiple texts that automatically rotate at specified intervals
- **Visual Effects**: 
  - Custom text and drop shadow colors
  - Strobing effects for dynamic text
  - Transparent backgrounds
  - Impact font as default
  - Contrasting outline (black or white)
  - Toggleable glow effect
  - Zoom animation
  - Pulsating text (size and glow variation)
- **Display Options**:
  - Absolute positioning (X, Y coordinates)
  - Grid positioning (column, row)
  - Center positioning
- **Text Customization**:
  - Three sizes: small, medium (default), large
  - Default pink/purple color with glow effect
- **Automatic Timing**: Overlays automatically disappear after specified duration

## Documentation

- **[README.md](README.md)** - User guide and usage instructions
- **[IMPLEMENTATION.md](IMPLEMENTATION.md)** - Technical documentation for developers
- **[CHANGELOG.md](CHANGELOG.md)** - Version history and changes
- **[LICENSE](LICENSE)** - MIT License information

## Installation

### Prerequisites

- .NET 8 SDK
- Windows OS

### Setup

1. Clone or download the repository
2. Open a terminal in the project root directory
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Build the project:
   ```bash
   dotnet build
   ```

## Usage

### Basic Command Structure

```bash
dotnet run --project Overlay.Cli -- [options]
```

### Quick Examples

1. **Display simple text centered on primary display for 5 seconds (using defaults):**
   ```bash
   dotnet run --project Overlay.Cli -- -text "Hello World" -duration 5
   ```

2. **Display strobing purple text with pink drop shadow in top-left corner:**
   ```bash
   dotnet run --project Overlay.Cli -- -text "STROBING" -duration 10 -foreground "#800080" -dropshadow "#FF69B4" -strobing -column 0 -row 0
   ```

3. **Display rotating texts every 2 seconds for 10 seconds:**
   ```bash
   dotnet run --project Overlay.Cli -- -texts "FIRST;SECOND;THIRD" -duration 10 -switch 2 -column 1 -row 1
   ```

4. **Display text on secondary display (index 1) in bottom-right position:**
   ```bash
   dotnet run --project Overlay.Cli -- -text "DISPLAY 2" -duration 5 -display 1 -column 2 -row 2
   ```

### Display Management

**List all available displays:**
```bash
dotnet run --project Overlay.Cli -- -list-displays
```

### Positioning Options

You can position overlays using three different methods:

1. **Absolute Positioning:**
   ```bash
   -x 100 -y 200
   ```

2. **Grid Positioning (recommended):**
   ```bash
   -column 0 -row 0    # Top-left
   -column 1 -row 1    # Center
   -column 2 -row 2    # Bottom-right
   ```

3. **Center Positioning:**
   ```bash
   -center
   ```

### Text Options

1. **Single Text:**
   ```bash
   -text "Your Text Here"
   ```

2. **Multiple Rotating Texts:**
   ```bash
   -texts "Text1;Text2;Text3"
   -switch 2           # Switch every 2 seconds
   ```

3. **Text Size:**
   ```bash
   -size small         # Small text
   -size medium        # Medium text (default)
   -size large         # Large text
   ```

### Visual Customization

- **Text Color:** `-foreground "#RRGGBB"` (default: #FF69B4 - pink/purple)
- **Drop Shadow Color:** `-dropshadow "#RRGGBB"` (default: black)
- **Outline Effect:** `-outline` (adds black or white outline depending on text color)
- **Glow Effect:** `-glow` (adds a subtle glow around text, enabled by default)
- **Zoom Animation:** `-zoom` (text grows from center to full size)
- **Pulsating Effect:** `-pulse [fast|medium|slow]` (text pulses in size and glow)
- **Strobing Effect:** `-strobing`
- **Display Selection:** `-display N` (0-based index)

### Timing Options

- **Duration:** `-duration N` (seconds, default: 10)
- **Text Switching Interval:** `-switch N` (seconds, default: 2)

### Grid System

The overlay uses a 3x3 grid system for positioning:

```
Column 0    Column 1    Column 2
Row 0    │ Top-Left  │ Top-Center │ Top-Right
Row 1    │ Mid-Left  │ Center     │ Mid-Right
Row 2    │ Bot-Left  │ Bot-Center │ Bot-Right
```

For vertical displays, the system automatically adjusts to a 1x3 grid.

## Advanced Examples

1. **Complex overlay with all features:**
   ```bash
   dotnet run --project Overlay.Cli -- -texts "ALERT;WARNING;CRITICAL" -duration 15 -switch 3 -foreground "#FF0000" -dropshadow "#000000" -strobing -display 1 -column 1 -row 0
   ```

2. **Multiple sequential overlays:**
   ```bash
   # First overlay
   dotnet run --project Overlay.Cli -- -text "PREPARING" -duration 3 -column 1 -row 1
   # Second overlay (after first completes)
   dotnet run --project Overlay.Cli -- -text "READY" -duration 2 -foreground "#00FF00" -column 1 -row 1
   ```

## Scripting

Create batch files for complex overlay sequences:

```batch
@echo off
echo System Maintenance
dotnet run --project Overlay.Cli -- -text "MAINTENANCE MODE" -duration 5 -foreground "#FFFF00" -dropshadow "#000000" -column 1 -row 0
timeout /t 1 >nul
dotnet run --project Overlay.Cli -- -text "SYSTEM RESTARTING" -duration 3 -foreground "#FF0000" -dropshadow "#000000" -strobing -column 1 -row 1
```

## Troubleshooting

- **Overlay not visible:** Ensure display index is correct (use `-list-displays`)
- **Text cut off:** Try adjusting position or using different grid positions
- **Performance issues:** Reduce strobing effects or number of simultaneous overlays

## License

This project is licensed under the MIT License - see the LICENSE file for details.