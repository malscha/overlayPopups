using Overlay.Core;
using Overlay.Interop;
using Overlay.Render;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Overlay.Cli
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Check if user wants to list displays
            if (args.Length == 1 && args[0].ToLower() == "-list-displays")
            {
                ListDisplays();
                return;
            }
            
            // Parse CLI arguments
            var options = ParseArguments(args);
            
            // Create and run the overlay
            var app = new OverlayApplication();
            app.Run(options);
        }
        
        static void ListDisplays()
        {
            var screens = System.Windows.Forms.Screen.AllScreens;
            Console.WriteLine($"Found {screens.Length} display(s):");
            for (int i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                Console.WriteLine($"  Display {i}: {screen.DeviceName}");
                Console.WriteLine($"    Bounds: {screen.Bounds.Width}x{screen.Bounds.Height} at ({screen.Bounds.Left}, {screen.Bounds.Top})");
                Console.WriteLine($"    Working Area: {screen.WorkingArea.Width}x{screen.WorkingArea.Height} at ({screen.WorkingArea.Left}, {screen.WorkingArea.Top})");
                Console.WriteLine($"    Primary: {screen.Primary}");
                Console.WriteLine();
            }
        }
        
        static OverlayOptions ParseArguments(string[] args)
        {
            var options = new OverlayOptions
            {
                X = 100,
                Y = 100,
                Texts = new string[] { "Overlay Text" },
                Duration = 10,
                SwitchInterval = 2,
                ForegroundColor = "#FFFFFF",
                DropShadowColor = "#000000",
                IsStrobing = false,
                DisplayIndex = 0,
                Column = -1,
                Row = -1,
                Columns = 3,
                Rows = 3
            };
            
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i].ToLower())
                {
                    case "-x":
                        if (i + 1 < args.Length && double.TryParse(args[i + 1], out double xVal))
                            options.X = xVal;
                        break;
                    case "-y":
                        if (i + 1 < args.Length && double.TryParse(args[i + 1], out double yVal))
                            options.Y = yVal;
                        break;
                    case "-text":
                        if (i + 1 < args.Length)
                            options.Texts = new string[] { args[i + 1] };
                        break;
                    case "-texts":
                        if (i + 1 < args.Length)
                        {
                            // Parse multiple texts separated by semicolon
                            options.Texts = args[i + 1].Split(';');
                        }
                        break;
                    case "-duration":
                        if (i + 1 < args.Length && double.TryParse(args[i + 1], out double dur))
                            options.Duration = dur;
                        break;
                    case "-switch":
                        if (i + 1 < args.Length && double.TryParse(args[i + 1], out double interval))
                            options.SwitchInterval = interval;
                        break;
                    case "-foreground":
                        if (i + 1 < args.Length)
                            options.ForegroundColor = args[i + 1];
                        break;
                    case "-dropshadow":
                        if (i + 1 < args.Length)
                            options.DropShadowColor = args[i + 1];
                        break;
                    case "-strobing":
                        options.IsStrobing = true;
                        break;
                    case "-display":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int display))
                            options.DisplayIndex = display;
                        break;
                    case "-center":
                        options.Center = true;
                        break;
                    case "-column":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int col))
                            options.Column = col;
                        break;
                    case "-row":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int row))
                            options.Row = row;
                        break;
                    case "-columns":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int cols))
                            options.Columns = cols;
                        break;
                    case "-rows":
                        if (i + 1 < args.Length && int.TryParse(args[i + 1], out int rows))
                            options.Rows = rows;
                        break;
                }
            }
            
            return options;
        }
    }
    
    class OverlayOptions
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string[] Texts { get; set; } = new string[] { "Overlay Text" };
        public double Duration { get; set; } = 10;
        public double SwitchInterval { get; set; } = 2;
        public string ForegroundColor { get; set; } = "#FFFFFF";
        public string DropShadowColor { get; set; } = "#000000";
        public bool IsStrobing { get; set; } = false;
        public int DisplayIndex { get; set; } = 0;
        public bool Center { get; set; } = false;
        public int Column { get; set; } = -1; // -1 means not set
        public int Row { get; set; } = -1;    // -1 means not set
        public int Columns { get; set; } = 3; // Default columns
        public int Rows { get; set; } = 3;    // Default rows
    }
    
    class OverlayApplication
    {
        private Window? _window;
        private Canvas? _canvas;
        private IRenderer? _renderer;
        private DispatcherTimer? _switchTimer;
        private DispatcherTimer? _closeTimer;
        private string[]? _texts;
        private int _currentTextIndex = 0;
        private Card? _currentCard;
        
        public void Run(OverlayOptions options)
        {
            // Create application
            var app = new System.Windows.Application();
            
            // Create window
            _window = new Window
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                Topmost = true,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                // Make window full screen on the target display
                WindowState = WindowState.Normal // We'll set size and position manually
            };
            
            // Position window on specified display
            PositionWindowOnDisplay(options);
            
            // Create canvas
            _canvas = new Canvas();
            _window.Content = _canvas;
            
            // Initialize renderer
            _renderer = new WpfRenderer();
            
            // Store texts for rotation
            _texts = options.Texts;
            
            // Calculate position based on grid if specified
            double x = options.X;
            double y = options.Y;
            
            // Get all screens and the target screen
            var screens = System.Windows.Forms.Screen.AllScreens;
            int displayIndex = Math.Max(0, Math.Min(options.DisplayIndex, screens.Length - 1));
            var screen = screens[displayIndex];
            
            if (options.Column >= 0 && options.Row >= 0)
            {
                // Use grid positioning
                var (gridX, gridY) = CalculateGridPosition(options);
                x = gridX;
                y = gridY;
            }
            else if (options.Center)
            {
                // Center the text (800x200 is the card size)
                x = screen.Bounds.Left + (screen.Bounds.Width - 800) / 2;
                y = screen.Bounds.Top + (screen.Bounds.Height - 200) / 2;
            }
            
            // Adjust coordinates to be relative to the window's position
            // The window is positioned at (screen.Bounds.Left, screen.Bounds.Top)
            // So we need to subtract that offset to make the card position relative to the window
            double relativeX = x - screen.Bounds.Left;
            double relativeY = y - screen.Bounds.Top;
            
            // Create and render initial card
            _currentCard = new Card
            {
                Id = "cli_card",
                Type = CardType.Text,
                X = relativeX,
                Y = relativeY,
                Width = 800,
                Height = 200,
                Opacity = 0.9,
                Text = _texts[0],
                ForegroundColor = options.ForegroundColor,
                DropShadowColor = options.DropShadowColor,
                TextAlignment = Overlay.Core.TextAlignment.Center,
                IsStrobing = options.IsStrobing,
                DisplayIndex = options.DisplayIndex
            };
            
            _renderer.RenderCard(_currentCard, _canvas);
            
            // Set up text switching if multiple texts are provided
            if (_texts.Length > 1)
            {
                _switchTimer = new DispatcherTimer();
                _switchTimer.Interval = TimeSpan.FromSeconds(options.SwitchInterval);
                _switchTimer.Tick += SwitchText;
                _switchTimer.Start();
            }
            
            // Set up window interop for click-through
            _window.Loaded += (sender, e) => {
                var hwnd = new WindowInteropHelper(_window).Handle;
                var extendedStyle = User32.GetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE);
                User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, extendedStyle | (int)(WindowStyles.WS_EX_LAYERED | WindowStyles.WS_EX_TOOLWINDOW | WindowStyles.WS_EX_TRANSPARENT));
            };
            
            // Set up auto-close
            _closeTimer = new DispatcherTimer();
            _closeTimer.Interval = TimeSpan.FromSeconds(options.Duration);
            _closeTimer.Tick += (sender, e) => {
                _closeTimer.Stop();
                _switchTimer?.Stop();
                _window.Close();
                app.Shutdown();
            };
            _closeTimer.Start();
            
            // Show window and run application
            _window.Show();
            app.Run();
        }
        
        private void SwitchText(object? sender, EventArgs e)
        {
            if (_texts == null || _currentCard == null || _canvas == null || _renderer == null)
                return;
            
            // Move to next text
            _currentTextIndex = (_currentTextIndex + 1) % _texts.Length;
            
            // Update card text
            _currentCard.Text = _texts[_currentTextIndex];
            
            // Re-render the card
            _renderer.ClearCanvas(_canvas);
            _renderer.RenderCard(_currentCard, _canvas);
        }
        
        private (double x, double y) CalculateGridPosition(OverlayOptions options)
        {
            // Get all screens
            var screens = System.Windows.Forms.Screen.AllScreens;
            
            // Make sure display index is valid
            int displayIndex = Math.Max(0, Math.Min(options.DisplayIndex, screens.Length - 1));
            
            // Get the selected screen
            var screen = screens[displayIndex];
            
            // Adjust columns/rows for vertical displays
            int columns = options.Columns;
            int rows = options.Rows;
            
            // For vertical displays, use 1 column and 3 rows
            if (screen.Bounds.Width < screen.Bounds.Height)
            {
                columns = 1;
                rows = 3;
            }
            
            // Make sure column/row are within bounds
            int column = Math.Max(0, Math.Min(options.Column, columns - 1));
            int row = Math.Max(0, Math.Min(options.Row, rows - 1));
            
            // Calculate grid cell dimensions
            double cellWidth = (double)screen.Bounds.Width / columns;
            double cellHeight = (double)screen.Bounds.Height / rows;
            
            // Calculate position within the grid cell (centered)
            // Column 0, Row 0 = Top Left
            // Column 2, Row 2 = Bottom Right
            double x = screen.Bounds.Left + (column * cellWidth) + (cellWidth - 800) / 2;
            double y = screen.Bounds.Top + (row * cellHeight) + (cellHeight - 200) / 2;
            
            return (x, y);
        }
        
        private void PositionWindowOnDisplay(OverlayOptions options)
        {
            if (_window == null) return;
            
            // Get all screens
            var screens = System.Windows.Forms.Screen.AllScreens;
            
            // Make sure display index is valid
            int displayIndex = Math.Max(0, Math.Min(options.DisplayIndex, screens.Length - 1));
            
            // Get the selected screen
            var screen = screens[displayIndex];
            
            // Position window on the selected screen
            _window.Left = screen.Bounds.Left;
            _window.Top = screen.Bounds.Top;
            _window.Width = screen.Bounds.Width;
            _window.Height = screen.Bounds.Height;
        }
    }
}
