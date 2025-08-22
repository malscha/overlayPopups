using Overlay.Core;
using Overlay.Interop;
using Overlay.Render;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Overlay.App;

/// <summary>
/// Interaction logic for OverlayWindow.xaml
/// </summary>
public partial class OverlayWindow : Window
{
    private readonly IRenderer _renderer;
    private readonly ISceneManager _sceneManager;
    private readonly ITrayService _trayService;
    private Configuration _config = new Configuration();
    private bool _isClickThrough = false;
    private bool _isCliMode = false;

    public OverlayWindow()
    {
        InitializeComponent();
        
        // Initialize components
        _renderer = new WpfRenderer();
        _sceneManager = new SceneManager();
        _trayService = new TrayService();
        
        // Set up tray icon
        SetupTrayIcon();
        
        // Load configuration
        _config = _sceneManager.LoadConfiguration();
        
        // Render initial scene
        RenderCurrentScene();
        
        // Set up window interop for click-through
        Loaded += OverlayWindow_Loaded;
    }

    public OverlayWindow(string[] args) : this()
    {
        _isCliMode = true;
        
        // Process CLI arguments
        ProcessCliArguments(args);
        
        // In CLI mode, we don't show the tray icon
        // and we automatically close after a period of time
        // or when the user clicks on the overlay
        MouseLeftButtonUp += (sender, e) => Close();
        MouseRightButtonUp += (sender, e) => Close();
        
        // Close after 10 seconds if no interaction
        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(10);
        timer.Tick += (sender, e) => Close();
        timer.Start();
    }

    private void ProcessCliArguments(string[] args)
    {
        // Simple CLI argument parsing
        // Format: -x XPOS -y YPOS -text "TEXT TO DISPLAY" -duration SECONDS
        double x = 100, y = 100;
        string text = "Overlay Text";
        double duration = 10;
        
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-x":
                    if (i + 1 < args.Length && double.TryParse(args[i + 1], out double xVal))
                        x = xVal;
                    break;
                case "-y":
                    if (i + 1 < args.Length && double.TryParse(args[i + 1], out double yVal))
                        y = yVal;
                    break;
                case "-text":
                    if (i + 1 < args.Length)
                        text = args[i + 1];
                    break;
                case "-duration":
                    if (i + 1 < args.Length && double.TryParse(args[i + 1], out double dur))
                        duration = dur;
                    break;
            }
        }
        
        // Create a temporary scene with the specified text
        CreateTemporaryScene(x, y, text);
    }

    private void CreateTemporaryScene(double x, double y, string text)
    {
        // Clear existing configuration
        _config = new Configuration();
        
        // Create a text card with the specified parameters
        var textCard = new Card
        {
            Id = "cli_card",
            Type = CardType.Text,
            X = x,
            Y = y,
            Width = 800,
            Height = 200,
            Opacity = 0.9,
            Text = text,
            ForegroundColor = "#FFFFFF",
            TextAlignment = Overlay.Core.TextAlignment.Center
        };

        _config.Cards.Add(textCard);

        // Create a temporary scene
        var scene = new Scene
        {
            Id = "cli_scene",
            Name = "CLI Scene",
            CardIds = new List<string> { "cli_card" }
        };

        _config.Scenes.Add(scene);
        _sceneManager.SetCurrentScene("cli_scene");
        
        // Render the scene
        RenderCurrentScene();
    }

    private void OverlayWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Get the window handle
        var hwnd = new WindowInteropHelper(this).Handle;
        
        // Set extended window styles
        var extendedStyle = User32.GetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE);
        User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, extendedStyle | (int)(WindowStyles.WS_EX_LAYERED | WindowStyles.WS_EX_TOOLWINDOW));
        
        // Set click-through based on mode
        if (_isCliMode)
        {
            // In CLI mode, we want the overlay to be clickable so user can close it
            SetClickThrough(false);
        }
        else
        {
            // In GUI mode, we want click-through by default
            SetClickThrough(true);
        }
        
        // Ensure the window stays on top
        Topmost = true;
    }

    private void RenderCurrentScene()
    {
        var scene = _sceneManager.GetCurrentScene();
        if (scene != null)
        {
            _renderer.RenderScene(scene, _config, OverlayCanvas);
        }
    }

    private void SetupTrayIcon()
    {
        // Don't show tray icon in CLI mode
        if (_isCliMode) return;
        
        // Set up context menu
        var menuItems = new Dictionary<string, Action>
        {
            { "Show/Hide Overlay", ToggleOverlayVisibility },
            { "Toggle Click-Through", ToggleClickThrough },
            { "Exit", ExitApplication }
        };

        _trayService.SetContextMenu(menuItems);
        _trayService.Show();
    }

    private void ToggleOverlayVisibility()
    {
        Visibility = Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
    }

    private void ToggleClickThrough()
    {
        SetClickThrough(!_isClickThrough);
    }

    private void SetClickThrough(bool clickThrough)
    {
        _isClickThrough = clickThrough;
        
        // Get the window handle
        var hwnd = new WindowInteropHelper(this).Handle;
        
        // Get current extended style
        var extendedStyle = User32.GetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE);
        
        if (clickThrough)
        {
            // Add WS_EX_TRANSPARENT flag
            User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, extendedStyle | (int)WindowStyles.WS_EX_TRANSPARENT);
        }
        else
        {
            // Remove WS_EX_TRANSPARENT flag
            User32.SetWindowLong(hwnd, WindowStyles.GWL_EXSTYLE, extendedStyle & ~(int)WindowStyles.WS_EX_TRANSPARENT);
        }
        
        // Show a brief visual indication of the change
        if (!_isCliMode)
        {
            ShowStatusMessage(clickThrough ? "Click-through ON" : "Click-through OFF");
        }
    }
    
    private void ShowStatusMessage(string message)
    {
        // Create a temporary text card to show status
        var statusCard = new Card
        {
            Id = "status_card",
            Type = CardType.Text,
            X = 100,
            Y = 100,
            Width = 400,
            Height = 100,
            Opacity = 0.8,
            Text = message,
            ForegroundColor = "#FFFFFF",
            BackgroundColor = "#0000FF",
            TextAlignment = Overlay.Core.TextAlignment.Center
        };
        
        // Clear canvas and show status
        _renderer.ClearCanvas(OverlayCanvas);
        _renderer.RenderCard(statusCard, OverlayCanvas);
        
        // Clear status after 1 second
        var timer = new System.Windows.Threading.DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (sender, e) => {
            RenderCurrentScene();
            timer.Stop();
        };
        timer.Start();
    }

    private void ExitApplication()
    {
        System.Windows.Application.Current.Shutdown();
    }
}